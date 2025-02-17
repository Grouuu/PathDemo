using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Based on https://github.com/SebLague/Poisson-Disc-Sampling/blob/master/Poisson%20Disc%20Sampling%20E01/PoissonDiscSampling.cs
 */

[RequireComponent(typeof(RandomSampler))]
public class RandomGrid : MonoBehaviour {

	[SerializeField] private int _cellSize = 1;						// unit size for each cell (one obstacle by cell max)
	[SerializeField] private float _startMinDistance = 3;			// min distance between obstacles at start
	[SerializeField] private float _spawnReservedDistance = 15;		// more than max neighbors radius gravity to have a spawn safe from gravity
	[SerializeField] private float _gridScaleFromView = 2;			// how many time the grid is bigger than the screen
	[SerializeField] private float _numSamplesBeforeRejection = 30;	// number of times we try to pick a new point around an old one
	[SerializeField] private bool _isDebug = false;

	private Dictionary<string, RandomGridPoint> _points = new Dictionary<string, RandomGridPoint>();
	private RandomSampler _sampler;
	private Vector2Int _gridSizeInCell;
	private Vector2 _centerPosition;
	private Vector2Int _centerCoords;
	private Vector4 _bounds;
	private float _minDistance;
	private Vector2 _halfGridSize;
	private int _minShiftBeforeUpdate => Mathf.CeilToInt(_minDistance * 2 / _cellSize);
	private float _maxReservedDistance;

	public void SetMinDistance (float minDistance) {
		_minDistance = minDistance;
	}

	public bool UpdatePoints (Vector2 position, out List<RandomGridPoint> spawnPoints) {
		bool obstaclesChanged = false;

		_centerPosition = position;

		if (!IsUpdateAllowed()) {
			// not dirty enough
			spawnPoints = new List<RandomGridPoint>();
			return false;
		}

		Vector4 oldBounds = _bounds;
		Vector2Int oldCoords = _centerCoords;
		_bounds = GetBounds(_centerPosition);
		_centerCoords = GetCoordsFromPosition(_centerPosition);

		if (ClearOutOfBoundsPoints(_bounds)) {
			obstaclesChanged = true;
		}

		spawnPoints = GenerateRandomPoints(oldBounds, oldCoords);

		if (spawnPoints.Count > 0) {
			obstaclesChanged = true;
		}

		return obstaclesChanged;
	}

	private void Awake () {
		_sampler = GetComponent<RandomSampler>();
	}

	private void Start () {
		_gridSizeInCell = Vector2Int.CeilToInt(Utils.GetSceneSize() / _cellSize * _gridScaleFromView);
		_halfGridSize = _gridSizeInCell / 2;
		_minDistance = _startMinDistance;
	}

	private bool IsUpdateAllowed () {

		Vector2Int newCenterCoords = GetCoordsFromPosition(_centerPosition);
		bool firstCall = _points.Count == 0;

		// wait enough cell shifts to update
		int shiftX = Mathf.Abs(newCenterCoords.x - _centerCoords.x);
		int shiftY = Mathf.Abs(newCenterCoords.y - _centerCoords.y);
		bool enoughShift = shiftX >= _minShiftBeforeUpdate || shiftY >= _minShiftBeforeUpdate;

		return enoughShift || firstCall;
	}

	private bool ClearOutOfBoundsPoints (Vector4 bounds) {

		List<string> deleteKeys = new List<string>();

		foreach (KeyValuePair<string, RandomGridPoint> entry in _points) {
			if (!isPositionInBounds(entry.Value.position, bounds)) {
				deleteKeys.Add(entry.Key);
			}
		}

		foreach (string key in deleteKeys) {
			_points[key].Destroy();
			_points.Remove(key);
		}

		return deleteKeys.Count > 0;
	}

	private List<RandomGridPoint> GenerateRandomPoints (Vector4 oldBounds, Vector2Int oldCoords) {

		List<RandomGridPoint> points = new List<RandomGridPoint>();
		Vector2Int gridShiftDirection = GetGridShift(oldCoords, _centerCoords);
		List<RandomGridPoint> spawnPoints = GetSeedPoints(oldBounds, gridShiftDirection);

		UpdateMaxReservedDistance();

		int debugCount = 0;

		int safeBreakCount = 0;
		while (spawnPoints.Count > 0) {

			int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
			RandomGridPoint spawnCentre = spawnPoints[spawnIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < _numSamplesBeforeRejection; i++) {

				debugCount++;

				float angle = UnityEngine.Random.value * Mathf.PI * 2;
				Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				Vector2 candidatePosition = spawnCentre.position + direction * UnityEngine.Random.Range(spawnCentre.reservedDistance, spawnCentre.reservedDistance + _minDistance);
				float candidateFactor = _sampler.GetFactorAt(candidatePosition);

				if (candidateFactor == 0) {
					// position not allowed by the sampler
					continue;
				}

				float reservedDistance = _minDistance * candidateFactor;

				if (IsInNewBounds(candidatePosition, oldBounds, _bounds) && IsValidPoint(candidatePosition, reservedDistance, gridShiftDirection)) {

					ObstacleData candidateData = _sampler.GetDataAt(candidatePosition);
					RandomGridPoint point = AddPoint(candidateData, candidatePosition, reservedDistance, candidateFactor);
					points.Add(point);
					spawnPoints.Add(point);
					candidateAccepted = true;
					break;
				}
			}

			if (!candidateAccepted) {
				spawnPoints.RemoveAt(spawnIndex);
			}

			UpdateMaxReservedDistance();

			safeBreakCount++;
			if (safeBreakCount >= 10000) {
				Debug.Log("Infinite spawn points loop suspected");
				break;
			}
		}

		if (_isDebug) {
			Debug.Log("Sample iterations : " + debugCount + " Points: " + _points.Count);
		}

		return points;
	}

	private RandomGridPoint AddPoint (
		ObstacleData data,
		Vector2 position,
		float reservedDistance,
		float factor,
		bool isRender = true,
		bool isFirst = false
	) {
		Vector2Int coords = GetCoordsFromPosition(position);
		string id = GetCellId(coords);
		RandomGridPoint point = new RandomGridPoint(data, position, coords, reservedDistance, factor, isRender, isFirst);

		// DEBUG
		if (_points.ContainsKey(id)) {
			Debug.LogWarning("ALREADY FILLED");
			Utils.PauseEditor();
		}

		_points.Add(id, point);

		return point;
	}

	private void UpdateMaxReservedDistance () {
		_maxReservedDistance = 0;
		foreach (RandomGridPoint point in _points.Values) {
			_maxReservedDistance = Mathf.Max(_maxReservedDistance, point.reservedDistance);
		}
	}

	private bool IsInNewBounds (Vector2 position, Vector4 oldBounds, Vector4 newBounds) {

		if (isPositionInBounds(position, oldBounds)) {
			return false;
		}

		if (!isPositionInBounds(position, newBounds)) {
			return false;
		}

		return true;
	}

	private bool IsValidPoint (Vector2 candidatePosition, float reservedDistance, Vector2Int shiftDirection) {

		// NOTE: the check against the spawn point reserved distance doesn't take account of the gravity radius, so use margin to preserve spawn position from gravity

		Vector2Int candidateCoords = GetCoordsFromPosition(candidatePosition);

		if (_points.ContainsKey(GetCellId(candidateCoords))) {
			// cell already occupied
			return false;
		}

		int range = Mathf.CeilToInt(_maxReservedDistance / _cellSize);
		int startX = candidateCoords.x - range;
		int endX = candidateCoords.x + range;
		int startY = candidateCoords.y - range;
		int endY = candidateCoords.y + range;

		// first call
		if (shiftDirection == Vector2Int.zero) {
			startX = candidateCoords.x - range;
			endX = candidateCoords.x + range;
			startY = candidateCoords.y - range;
			endY = candidateCoords.y + range;
		}

		for (int coordX = startX; coordX <= endX; coordX++) {
			for (int coordY = startY; coordY <= endY; coordY++) {

				Vector2Int checkCoords = new Vector2Int(coordX, coordY);

				if (_points.TryGetValue(GetCellId(checkCoords), out RandomGridPoint checkPoint)) {

					if (!checkPoint.isRender && !checkPoint.isFirst) {
						// ignore seed points (except the spawn start)
						continue;
					}

					float sqrDistance = (candidatePosition - checkPoint.position).sqrMagnitude;
					float maxReservedDistance = Mathf.Max(reservedDistance, checkPoint.reservedDistance);

					if (sqrDistance < maxReservedDistance * maxReservedDistance) {
						return false;
					}
				}
			}
		}

		return true;
	}

	private bool isPositionInBounds (Vector2 position, Vector4 bounds) {
		return position.x >= bounds.x
			&& position.x <= bounds.z
			&& position.y >= bounds.w
			&& position.y <= bounds.y
		;
	}

	private Vector4 GetBounds (Vector2 centerPosition) {
		// top-left (xy), right-bottom (z, w)
		return new Vector4(
			centerPosition.x - _halfGridSize.x,
			centerPosition.y + _halfGridSize.y,
			centerPosition.x + _halfGridSize.x,
			centerPosition.y - _halfGridSize.y
		);
	}

	private List<RandomGridPoint> GetSeedPoints (Vector4 bounds, Vector2Int shift) {

		// points added to the grid but not to the scene
		List<RandomGridPoint> seedPoints = new List<RandomGridPoint>();

		// at start
		if (bounds == Vector4.zero) {
			// force empty space around the player at start
			seedPoints.Add(AddPoint(null, _centerPosition, _spawnReservedDistance, 1, false, true));
			return seedPoints;
		}

		float shiftWidth = Mathf.Abs(shift.x * _cellSize);
		float shiftHeight = Mathf.Abs(shift.y * _cellSize);

		Action<Vector2> addSeedPoint = seedPosition => {
			Vector2Int seedCoords = GetCoordsFromPosition(seedPosition);
			if (seedPosition != Vector2.zero && !_points.ContainsKey(GetCellId(seedCoords))) {
				// add blank seed points aligned to the center xy axis in the middle of the new areas
				seedPoints.Add(AddPoint(null, seedPosition, 0, 1, false));
			}
		};

		Vector2[] seedsBySide = new Vector2[4] {
			shift.x > 0 ? _centerPosition + new Vector2(_halfGridSize.x - shiftWidth / 2, 0) : Vector2.zero,  // right
			shift.x < 0 ? _centerPosition - new Vector2(_halfGridSize.x - shiftWidth / 2, 0) : Vector2.zero,  // left
			shift.y > 0 ? _centerPosition + new Vector2(0, _halfGridSize.y - shiftHeight / 2) : Vector2.zero, // top
			shift.y < 0 ? _centerPosition - new Vector2(0, _halfGridSize.y - shiftHeight / 2) : Vector2.zero  // bottom
		};

		foreach (Vector2 sidePosition in seedsBySide) {
			if (sidePosition != Vector2.zero) {
				addSeedPoint(sidePosition);
			}
		}

		return seedPoints;
	}

	private Vector2Int GetGridShift (Vector2Int oldCoords, Vector2Int newCoords) {
		int horizontalShift = (int) System.Math.Sign(newCoords.x - oldCoords.x) * Mathf.Abs(oldCoords.x - newCoords.x);
		int verticalShift = (int) System.Math.Sign(newCoords.y - oldCoords.y) * Mathf.Abs(oldCoords.y - newCoords.y);
		return new Vector2Int(horizontalShift, verticalShift);
	}

	private string GetCellId (Vector2Int coords) {
		return coords.x + " " + coords.y;
	}

	private Vector2Int GetCoordsFromPosition (Vector2 position) {
		return new Vector2Int(
			Mathf.RoundToInt(position.x / _cellSize),
			Mathf.RoundToInt(position.y / _cellSize)
		);
	}

}
