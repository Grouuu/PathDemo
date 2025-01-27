using System.Collections.Generic;
using UnityEngine;

public enum GridChunkState { None, New, NewBorder, Border, OldBorder, Inside }

public class GridChunk {

	public GridChunkState state = GridChunkState.None;
	public Vector2Int coords; // can be negative
	public Vector2 centerPosition => _centerPosition;

	private GridPoint[] _grid;
	private int _chunkSize;
	private float _cellSize;
	private Vector2 _centerPosition => new Vector2(coords.x * _chunkSize * _cellSize, coords.y * _chunkSize * _cellSize);

	public GridChunk (Vector2Int coords, int chunkSize, float cellSize) {
		this.coords = coords;
		this._chunkSize = chunkSize;
		this._cellSize = cellSize;
		this._grid = new GridPoint[chunkSize * chunkSize];
	}

	public void AddPoint (GridPoint point) {
		Vector2 topLeftPosition = _centerPosition + new Vector2(-_chunkSize * _cellSize / 2, -_chunkSize * _cellSize / 2);
		Vector2 localPosition = point.position - topLeftPosition;
		int localCoordX = Mathf.FloorToInt(localPosition.x / _cellSize);
		int localCoordY = Mathf.FloorToInt(localPosition.y / _cellSize);
		int gridIndex = Utils.GridCoordsToIndex(localCoordX, localCoordY, _chunkSize);

		_grid[gridIndex] = point;
	}

	public List<GridPoint> GetPoints () {
		List<GridPoint> points = new List<GridPoint>();
		foreach (GridPoint point in _grid) {
			if (point != null) {
				points.Add(point);
			}
		}
		return points;
	}

	public bool isFreePosition (Vector2 position, float minDistance) {

		for (int x = 0; x < _chunkSize; x++) {
			for (int y = 0; y < _chunkSize; y++) {

				int index = Utils.GridCoordsToIndex(x, y, _chunkSize);
				GridPoint point = _grid[index];

				if (point != null) {

					float reservedDistance = Mathf.Max(minDistance, point.reservedDistance);
					float sqrDistance = (position - point.position).sqrMagnitude;

					if (sqrDistance < reservedDistance * reservedDistance) {
						return false;
					}
				}
			}
		}

		return true;
	}

	public void Destroy () {
		foreach (GridPoint point in _grid) {
			if (point != null) {
				point.Destroy();
			}
		}
	}
}