using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChunkSampler))]
public class ChunkGrid : MonoBehaviour {

	private ChunkSampler _sampler;
	private Vector2Int _gridSizeInChunk;		// number of chunks on xy axis
	private int _chunkSizeInUnit;               // unit size of each grid chunk (provided by the sampler)
	private Int4 _boundsCoords;                 // bottom-left and top-right chunks coords
	private Vector2Int _halfGridSizeInChunk;    // half of the number of chunks on xy axis
	private Vector2 _centerPosition;			// world position of the center of the grid
	private bool _forceSpawnUpdate = true;		// force a spawn check

	private Dictionary<string, ChunkGridPoint> _points = new Dictionary<string, ChunkGridPoint>();

	public void UpdateGridSize () {
		// grid size big enough to cover the screen + at least 2 chunks buffer
		_gridSizeInChunk = Vector2Int.CeilToInt(Utils.GetSceneSize() / _chunkSizeInUnit) + Vector2Int.one * 2;
		_halfGridSizeInChunk = _gridSizeInChunk / 2;
		UpdateBoundsCoords();
		_forceSpawnUpdate = true;
	}

	public bool UpdatePoints (Vector2 targetPosition, out List<ChunkGridPoint> spawnPoints) {

		bool obstaclesChanged = false;
		spawnPoints = new List<ChunkGridPoint>();

		Vector2Int oldCoords = GetCoords(_centerPosition);
		Vector2Int newCoords = GetCoords(targetPosition);
		Vector2Int coordsShift = GetGridCoordsShift(oldCoords, newCoords);
		bool enoughShift = Mathf.Abs(coordsShift.x) >= 1 || Mathf.Abs(coordsShift.y) >= 1;

		if (!enoughShift && !_forceSpawnUpdate) {
			return false;
		}

		_centerPosition = targetPosition;

		UpdateBoundsCoords();

		if (ClearOutOfBoundsPoints()) {
			obstaclesChanged = true;
		}

		List<Vector2Int> newChunks = GetNewChunks(oldCoords, newCoords, _forceSpawnUpdate);
		spawnPoints = _sampler.GetChunksPoints(newChunks);

		foreach (ChunkGridPoint point in spawnPoints) {
			_points.Add(GetChunkId(point.chunkCoords), point);
		}

		if (spawnPoints.Count > 0) {
			obstaclesChanged = true;
		}

		_forceSpawnUpdate = false;

		return obstaclesChanged;
	}

	private void Awake () {
		_sampler = GetComponent<ChunkSampler>();
		_chunkSizeInUnit = ChunkSampler.chunkSizeInUnit;
	}

	private void Start () {
		UpdateGridSize();
	}

	private void UpdateBoundsCoords () {
		_boundsCoords = GetBoundsCoords(_centerPosition);
	}

	private List<Vector2Int> GetNewChunks (Vector2Int oldCoords, Vector2Int newCoords, bool forceUpdate) {
		List<Vector2Int> chunkCoords = new List<Vector2Int>();

		Int4 oldBounds = GetBoundsCoords(oldCoords);
		Int4 newBounds = GetBoundsCoords(newCoords);

		for (int coordX = newBounds.x; coordX <= newBounds.z; coordX++) {
			for (int coordY = newBounds.y; coordY <= newBounds.w; coordY++) {

				if (!forceUpdate && IsCoordsInBounds(coordX, coordY, oldBounds)) {
					// already done
					continue;
				}

				Vector2Int coords = new Vector2Int(coordX, coordY);
				string key = GetChunkId(coords);

				if (!_points.ContainsKey(key)) {
					chunkCoords.Add(coords);
				}
			}
		}

		return chunkCoords;
	}

	private bool ClearOutOfBoundsPoints () {
		bool hasDestroyedPoints = false;
		List<string> deleteKeys = new List<string>();

		foreach (KeyValuePair<string, ChunkGridPoint> entry in _points) {
			if (!IsCoordsInBounds(entry.Value.chunkCoords, _boundsCoords)) {
				deleteKeys.Add(entry.Key);
				hasDestroyedPoints = true;
			}
		}

		foreach (string key in deleteKeys) {
			_points[key].Destroy();
			_points.Remove(key);
		}

		return hasDestroyedPoints;
	}

	private Vector2Int GetGridCoordsShift (Vector2Int oldCoords, Vector2Int newCoords) {
		int horizontalShift = (int) System.Math.Sign(newCoords.x - oldCoords.x) * Mathf.Abs(oldCoords.x - newCoords.x);
		int verticalShift = (int) System.Math.Sign(newCoords.y - oldCoords.y) * Mathf.Abs(oldCoords.y - newCoords.y);
		return new Vector2Int(horizontalShift, verticalShift);
	}

	private Vector2Int GetCoords (Vector2 position) {
		// bottom-left origin
		int x = Mathf.FloorToInt(position.x / _chunkSizeInUnit);
		int y = Mathf.FloorToInt(position.y / _chunkSizeInUnit);
		return new Vector2Int(x, y);
	}

	private Int4 GetBoundsCoords (Vector2 position) {
		return GetBoundsCoords(GetCoords(position));
	}

	private Int4 GetBoundsCoords (Vector2Int coords) {
		Vector2Int bottomLeft = coords - _halfGridSizeInChunk;
		Vector2Int topRight = coords + _halfGridSizeInChunk;
		return new Int4(
			bottomLeft.x,
			bottomLeft.y,
			topRight.x,
			topRight.y
		);
	}

	private string GetChunkId (Vector2Int coords) {
		return $"{coords.x}_{coords.y}";
	}

	private bool IsCoordsInBounds (Vector2Int coords, Int4 bounds) {
		return IsCoordsInBounds(coords.x, coords.y, bounds);
	}

	private bool IsCoordsInBounds (int coordX, int coordY, Int4 bounds) {
		return coordX >= bounds.x
			&& coordX <= bounds.z
			&& coordY >= bounds.y
			&& coordY <= bounds.w
		;
	}
}
