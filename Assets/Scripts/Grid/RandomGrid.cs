using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Based on https://github.com/SebLague/Poisson-Disc-Sampling/blob/master/Poisson%20Disc%20Sampling%20E01/PoissonDiscSampling.cs
 * 
 * Dependencies:
 * . RandomSampler
 * . RandomGridPoint
 * . ObstacleData
 * . Utils
 */
[RequireComponent(typeof(RandomSampler))]
public class RandomGrid : MonoBehaviour
{
	public RandomSampler sampler { get; private set; }

	[SerializeField] private int _cellSizeInUnit = 1;				// unit size for each cell (one obstacle by cell max)
	[SerializeField] private float _startMinDistance = 3;			// min distance between obstacles at start
	[SerializeField] private float _spawnReservedDistance = 15;		// more than max neighbors radius gravity to have a spawn safe from gravity
	[SerializeField] private float _gridScaleRelatedToScene = 2;	// how many time the grid is bigger than the scene
	[SerializeField] private float _numSamplesBeforeRejection = 30;	// number of time we try to pick a new point around an old one
	[SerializeField] private bool _isDebug = false;

	private int _minShiftBeforeUpdate => Mathf.CeilToInt(_minDistance * 2 / _cellSizeInUnit);

	private Dictionary<string, RandomGridPoint> _points = new Dictionary<string, RandomGridPoint>();
	private Vector2 _gridSizeInUnit;
	private Vector2Int _gridSizeInCell;
	private Vector2 _targetPosition;
	private Vector2Int _centerCoords;
	private Vector4 _bounds;
	private float _minDistance;
	private Vector2 _halfGridSizeInCell;
	private float _maxReservedDistance;
	private bool _isFirstCall = true;

	public void UpdateGridSize ()
	{
		_gridSizeInUnit = Utils.GetSceneSize() * _gridScaleRelatedToScene;
		_gridSizeInCell = Vector2Int.CeilToInt(_gridSizeInUnit / _cellSizeInUnit);
		_halfGridSizeInCell = _gridSizeInCell / 2;
		_minDistance = _startMinDistance;
	}

	public void SetMinDistance (float minDistance)
	{
		_minDistance = minDistance;
	}

	public bool UpdatePoints (Vector2 position, out List<RandomGridPoint> spawnPoints)
	{
		bool isDirty = false;

		_targetPosition = position;

		if (!IsUpdateAllowed())
		{
			// didn't moved enough
			spawnPoints = new List<RandomGridPoint>();
			return false;
		}

		Vector4 oldBounds = _bounds;
		Vector2Int oldCoords = _centerCoords;
		_bounds = GetBounds(_targetPosition);
		_centerCoords = GetCoordsFromPosition(_targetPosition);

		// remove out of bounds points
		if (ClearOutOfBoundsPoints(_bounds))
		{
			isDirty = true;
		}

		// add new in bounds points
		spawnPoints = GenerateRandomPoints(oldBounds, oldCoords);

		if (spawnPoints.Count > 0)
		{
			isDirty = true;
		}

		_isFirstCall = false;

		return isDirty;
	}

	private void Awake ()
	{
		sampler = GetComponent<RandomSampler>();
	}

	private void Start ()
	{
		UpdateGridSize();
	}

	private bool IsUpdateAllowed ()
	{
		// wait enough cell shift to update
		Vector2Int targetCenterCoords = GetCoordsFromPosition(_targetPosition);
		int shiftX = Mathf.Abs(targetCenterCoords.x - _centerCoords.x);
		int shiftY = Mathf.Abs(targetCenterCoords.y - _centerCoords.y);
		bool enoughShift = shiftX >= _minShiftBeforeUpdate || shiftY >= _minShiftBeforeUpdate;

		return enoughShift || _isFirstCall;
	}

	private bool ClearOutOfBoundsPoints (Vector4 bounds)
	{
		List<string> deleteKeys = new List<string>();

		foreach (KeyValuePair<string, RandomGridPoint> entry in _points)
		{
			if (!isPositionInBounds(entry.Value.position, bounds))
			{
				deleteKeys.Add(entry.Key);
			}
		}

		foreach (string key in deleteKeys)
		{
			// remove the gameObject (pool)
			_points[key].Destroy();
			_points.Remove(key);
		}

		return deleteKeys.Count > 0;
	}

	private List<RandomGridPoint> GenerateRandomPoints (Vector4 oldBounds, Vector2Int oldCenterCoords)
	{
		List<RandomGridPoint> newPoints = new List<RandomGridPoint>();
		Vector2Int gridShiftDirection = GetGridShift(oldCenterCoords, _centerCoords);
		List<RandomGridPoint> spawnPoints = GetSeedPoints(gridShiftDirection);

		// use correct sample size to prevent any violation of reserved distances
		UpdateMaxReservedDistance();

		int debugIterationCount = 0;
		int safeBreakCount = 0;

		while (spawnPoints.Count > 0)
		{
			int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
			RandomGridPoint spawnCentre = spawnPoints[spawnIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < _numSamplesBeforeRejection; i++)
			{
				debugIterationCount++;

				float randomAngle = UnityEngine.Random.value * Mathf.PI * 2;
				Vector2 randomDirection = new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle));
				Vector2 randomDeltaPosition = randomDirection * UnityEngine.Random.Range(spawnCentre.reservedDistance, spawnCentre.reservedDistance + _minDistance);

				Vector2 candidatePosition = spawnCentre.position + randomDeltaPosition;
				float candidateFactor = sampler.GetFactorAt(candidatePosition);

				if (candidateFactor == -1)
				{
					// position not allowed by the sampler
					continue;
				}

				float reservedDistance = _minDistance * candidateFactor;

				if (IsInNewBounds(candidatePosition, oldBounds, _bounds) && IsValidPoint(candidatePosition, reservedDistance))
				{
					ObstacleData candidateData = sampler.GetDataAt(candidatePosition);
					RandomGridPoint point = AddPoint(candidateData, candidatePosition, reservedDistance, candidateFactor);
					newPoints.Add(point);
					spawnPoints.Add(point);
					candidateAccepted = true;
					break;
				}
			}

			if (!candidateAccepted)
			{
				spawnPoints.RemoveAt(spawnIndex);
			}

			UpdateMaxReservedDistance();

			safeBreakCount++;
			if (safeBreakCount >= 1000)
			{
				Debug.Log("Infinite spawn points loop suspected");
				break;
			}
		}

		if (_isDebug) {
			Debug.Log($"Sample iterations : {debugIterationCount}, Total points: {_points.Count}");
		}

		return newPoints;
	}

	private RandomGridPoint AddPoint (
		ObstacleData data,
		Vector2 position,
		float reservedDistance,
		float factor,
		bool isRender = true,
		bool isFirst = false
	)
	{
		Vector2Int coords = GetCoordsFromPosition(position);
		string id = GetCellId(coords);
		RandomGridPoint point = new RandomGridPoint(data, position, coords, reservedDistance, factor, isRender, isFirst);

		_points.Add(id, point);

		return point;
	}

	private void UpdateMaxReservedDistance ()
	{
		_maxReservedDistance = 0;

		foreach (RandomGridPoint point in _points.Values)
		{
			_maxReservedDistance = Mathf.Max(_maxReservedDistance, point.reservedDistance);
		}
	}

	private bool IsInNewBounds (Vector2 position, Vector4 oldBounds, Vector4 newBounds)
	{
		if (isPositionInBounds(position, oldBounds))
		{
			return false;
		}

		if (!isPositionInBounds(position, newBounds))
		{
			return false;
		}

		return true;
	}

	private bool IsValidPoint (Vector2 candidatePosition, float reservedDistance)
	{
		// NOTE: the check against the spawn point reserved distance doesn't take account of the gravity radius, so use margin to preserve spawn position from gravity

		Vector2Int candidateCoords = GetCoordsFromPosition(candidatePosition);

		if (_points.ContainsKey(GetCellId(candidateCoords)))
		{
			// cell already occupied
			return false;
		}

		// check all cells around, enough to check the biggest reserved distance of the grid
		int range = Mathf.CeilToInt(_maxReservedDistance / _cellSizeInUnit);
		int startX = candidateCoords.x - range;
		int endX = candidateCoords.x + range;
		int startY = candidateCoords.y - range;
		int endY = candidateCoords.y + range;

		for (int coordX = startX; coordX <= endX; coordX++)
		{
			for (int coordY = startY; coordY <= endY; coordY++)
			{
				Vector2Int checkCoords = new Vector2Int(coordX, coordY);

				if (_points.TryGetValue(GetCellId(checkCoords), out RandomGridPoint checkPoint))
				{
					if (!checkPoint.isRender && !checkPoint.isFirst)
					{
						// ignore seed points (except the spawn start)
						continue;
					}

					float sqrDistance = (candidatePosition - checkPoint.position).sqrMagnitude;
					float maxReservedDistance = Mathf.Max(reservedDistance, checkPoint.reservedDistance);

					if (sqrDistance < maxReservedDistance * maxReservedDistance)
					{
						// too close
						return false;
					}
				}
			}
		}

		return true;
	}

	private bool isPositionInBounds (Vector2 position, Vector4 bounds)
	{
		return position.x >= bounds.x
			&& position.x <= bounds.z
			&& position.y >= bounds.w
			&& position.y <= bounds.y
		;
	}

	private Vector4 GetBounds (Vector2 centerPosition)
	{
		// top-left (xy), right-bottom (zw)
		return new Vector4(
			centerPosition.x - _halfGridSizeInCell.x,
			centerPosition.y + _halfGridSizeInCell.y,
			centerPosition.x + _halfGridSizeInCell.x,
			centerPosition.y - _halfGridSizeInCell.y
		);
	}

	private List<RandomGridPoint> GetSeedPoints (Vector2Int shift)
	{
		// points added to the grid but not to the scene
		List<RandomGridPoint> seedPoints = new List<RandomGridPoint>();

		if (_isFirstCall)
		{
			// force empty space around the player at start
			seedPoints.Add(AddPoint(null, _targetPosition, _spawnReservedDistance, 1, false, true));
			return seedPoints;
		}

		float shiftWidth = Mathf.Abs(shift.x * _cellSizeInUnit);
		float shiftHeight = Mathf.Abs(shift.y * _cellSizeInUnit);

		Action<Vector2> addSeedPoint = seedPosition =>
		{
			Vector2Int seedCoords = GetCoordsFromPosition(seedPosition);
			bool isValid = seedPosition != default;
			bool isExist = _points.ContainsKey(GetCellId(seedCoords));

			if (isValid && !isExist)
			{
				// add blank seed points aligned to the center xy axis in the middle of the new areas
				seedPoints.Add(AddPoint(null, seedPosition, 0, 1, false));
			}
		};

		Vector2[] seedsBySide = new Vector2[4] {
			shift.x > 0 ? _targetPosition + new Vector2(_halfGridSizeInCell.x - shiftWidth / 2, 0) : default,  // right
			shift.x < 0 ? _targetPosition - new Vector2(_halfGridSizeInCell.x - shiftWidth / 2, 0) : default,  // left
			shift.y > 0 ? _targetPosition + new Vector2(0, _halfGridSizeInCell.y - shiftHeight / 2) : default, // top
			shift.y < 0 ? _targetPosition - new Vector2(0, _halfGridSizeInCell.y - shiftHeight / 2) : default  // bottom
		};

		foreach (Vector2 sidePosition in seedsBySide)
		{
			addSeedPoint(sidePosition);
		}

		return seedPoints;
	}

	private Vector2Int GetGridShift (Vector2Int oldCoords, Vector2Int newCoords)
	{
		int horizontalShift = Math.Sign(newCoords.x - oldCoords.x) * Mathf.Abs(oldCoords.x - newCoords.x);
		int verticalShift = Math.Sign(newCoords.y - oldCoords.y) * Mathf.Abs(oldCoords.y - newCoords.y);
		return new Vector2Int(horizontalShift, verticalShift);
	}

	private string GetCellId (Vector2Int coords)
	{
		return $"{coords.x}_{coords.y}";
	}

	private Vector2Int GetCoordsFromPosition (Vector2 position)
	{
		return new Vector2Int(
			Mathf.RoundToInt(position.x / _cellSizeInUnit),
			Mathf.RoundToInt(position.y / _cellSizeInUnit)
		);
	}

#if UNITY_EDITOR

	private void OnDrawGizmos ()
	{
		if (_isDebug)
		{
			Debug.DrawCenteredRectangle(_targetPosition, Utils.GetSceneSize() / 2, Color.white, 0);
			Debug.DrawCenteredRectangle(_targetPosition, _gridSizeInUnit * _cellSizeInUnit, Color.red, 0);
		}
	}

#endif

}
