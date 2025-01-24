
using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnPointDestroy (GridPoint point, ObstacleBody body);

public enum GridChunkState { None, New, NewBorder, Border, OldBorder, Inside }

[Serializable] // DEUG
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

	//public bool InBounds (Vector2 position) {
	//	return Utils.IsInBounds(position, GetBounds());
	//}

	//public Vector2 GetBounds () {
	//	return new Vector2(_centerPosition.x - _chunkSize * _cellSize / 2, _centerPosition.y - _chunkSize * _cellSize);
	//}

	//public bool IsOutOfRender (Vector2 position, float range) {
	//	return Array.Exists(_grid, (SamplerPoint data) => data.SqrDistanceTo(position) < range * range);
	//}

	public void AddPoint (GridPoint point) {
		Vector2 topLeftPosition = _centerPosition + new Vector2(-_chunkSize * _cellSize / 2, -_chunkSize * _cellSize / 2);
		Vector2 localPosition = point.position - topLeftPosition;
		int localCoordX = Mathf.FloorToInt(localPosition.x / _cellSize);
		int localCoordY = Mathf.FloorToInt(localPosition.y / _cellSize);
		int gridIndex = Utils.GridCoordsToIndex(localCoordX, localCoordY, _chunkSize);

		// DEBUG
		point.chunk = this;

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

	public bool isFreePosition(Vector2 position, float minDistance) {

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

[Serializable] // DEUG
public class GridPoint { 

	public event OnPointDestroy OnDestroy;

	public ObstacleBody body;
	public Vector2 position;
	public float reservedDistance;
	public float sizeFactor;
	public bool isRender;

	// DEBUG
	public GridChunk chunk;

	public GridPoint (Vector2 position, float reservedDistance, float sizeFactor, bool isRender = true) {
		this.position = position;
		this.reservedDistance = reservedDistance;
		this.sizeFactor = sizeFactor;
		this.isRender = isRender;
	}

	public void Destroy () {
		if (OnDestroy != null) {
			OnDestroy(this, body);
		}
	}
}

public struct GridOptions {

	public float minDistance;
	public float maxDistance;
	public int chunkSize;
	public int renderChunkRange;

	public GridOptions (int renderChunkRange, int chunkSize, float minDistance, float maxDistance) {
		this.renderChunkRange = renderChunkRange;
		this.chunkSize = chunkSize;
		this.minDistance = minDistance;
		this.maxDistance = maxDistance;
	}
}

public class Grid {

	public List<GridChunk> chunks { get; private set; }
	public float cellSize => _cellSize;

	private GridOptions _options;
	private float _cellSize;
	private GridChunk _currentCenterChunk;

	public Grid(GridOptions options) {
		_options = options;

		_cellSize = _options.minDistance / Utils.SQRT_2;
		chunks = new List<GridChunk>();
	}

	public bool UpdateChunks (Vector2 position) {

		if (_currentCenterChunk != null && GetChunkCoordsFromPosition(position) == _currentCenterChunk.coords) {
			return false;
		}

		ClearOutOfRenderChunks(position);

		GridChunk centerChunk = GetChunk(position);

		if (centerChunk == null) {
			centerChunk = AddChunk(position);
		}

		_currentCenterChunk = centerChunk;

		int searchChunkRange = Mathf.FloorToInt(_options.renderChunkRange);
		int minCoordX = centerChunk.coords.x - searchChunkRange;
		int maxCoordX = centerChunk.coords.x + searchChunkRange;
		int minCoordY = centerChunk.coords.y - searchChunkRange;
		int maxCoordY = centerChunk.coords.y + searchChunkRange;

		for (int coordX = minCoordX; coordX <= maxCoordX; coordX++) {
			for (int cooordY = minCoordY; cooordY <= maxCoordY; cooordY++) {
				AddChunk(coordX, cooordY);
			}
		}

		UpdateChunksState(minCoordX, maxCoordX, minCoordY, maxCoordY);

		return true;
	}

	public List<GridPoint> GetPoints (bool onlyOldBorderChunks) {
		List<GridPoint> points = new List<GridPoint>();

		foreach (GridChunk chunk in chunks) {
			if (
				(onlyOldBorderChunks && chunk.state == GridChunkState.OldBorder)
				|| !onlyOldBorderChunks
			) {
				points.AddRange(chunk.GetPoints());
			}
		}

		return points;
	}

	public void AddPoint (GridPoint point) {
		GridChunk chunk = GetChunk(point.position);
		chunk.AddPoint(point);
	}

	public bool IsPointValid (GridPoint point) {

		GridChunk centerChunk = GetChunk(point.position);

		if (centerChunk == null || centerChunk.state == GridChunkState.Inside) {
			return false;
		}

		int range = _options.chunkSize;

		for (int x = centerChunk.coords.x - range; x <= centerChunk.coords.x + range; x++) {
			for (int y = centerChunk.coords.y - range; y <= centerChunk.coords.y + range; y++) {

				GridChunk chunk = GetChunk(x, y);

				if (chunk == null) {
					continue;
				}

				if (!chunk.isFreePosition(point.position, point.reservedDistance)) {
					return false;
				}
			}
		}

		return true;
	}

	private void UpdateChunksState (int minCoordX, int maxCoordX, int minCoordY, int maxCoordY) {
		for (int x = minCoordX; x <= maxCoordX; x++) {
			for (int y = minCoordY; y <= maxCoordY; y++) {

				GridChunk chunk = GetChunk(x, y);
				bool isBorderChunk = (x == minCoordX || x == maxCoordX) || (y == minCoordY || y == maxCoordY);

				if (chunk.state == GridChunkState.None) {
					chunk.state = isBorderChunk ? GridChunkState.NewBorder : GridChunkState.New;
				} else if (chunk == _currentCenterChunk) {
					// prevent to add new points on screen
					chunk.state = GridChunkState.Inside;
				} else if ((chunk.state == GridChunkState.Border || chunk.state == GridChunkState.NewBorder) && !isBorderChunk) {
					// used to fill new chunks
					chunk.state = GridChunkState.OldBorder;
				} else {
					chunk.state = isBorderChunk ? GridChunkState.Border : GridChunkState.Inside;
				}
			}
		}
	}

	private GridChunk GetChunk (Vector2 position) {
		return GetChunk(GetChunkCoordsFromPosition(position));
	}

	private GridChunk GetChunk (int coordX, int coordY) {
		return GetChunk(new Vector2Int(coordX, coordY));
	}

	private GridChunk GetChunk (Vector2Int coords) {
		GridChunk test = chunks.Find((GridChunk chunk) => chunk.coords == coords);
		return chunks.Find((GridChunk chunk) => chunk.coords == coords);
	}

	private GridChunk AddChunk (Vector2 position) {
		return AddChunk(GetChunkCoordsFromPosition(position));
	}

	private GridChunk AddChunk (int coordX, int coordY) {
		return AddChunk(new Vector2Int(coordX, coordY));
	}

	private GridChunk AddChunk (Vector2Int coords) {
		if (HasChunk(coords)) {
			return GetChunk(coords);
		}
		GridChunk chunk = new GridChunk(coords, _options.chunkSize, _cellSize);
		chunks.Add(chunk);
		return chunk;
	}

	private void DestroyChunk(GridChunk chunk) {
		chunk.Destroy();
		chunks.Remove(chunk);
	}

	private bool HasChunk (Vector2Int coords) {
		foreach (GridChunk chunk in chunks) {
			if (chunk.coords == coords) {
				return true;
			}
		}
		return false;
	}

	private void ClearOutOfRenderChunks (Vector2 position) {
		GridChunk centerChunk = GetChunk(position);

		if (centerChunk == null) {
			return;
		}

		for (int i = chunks.Count - 1; i >= 0; i--) {
			GridChunk chunk = chunks[i];

			if (chunk == null) {
				continue;
			}

			bool leftOut = chunk.coords.x < centerChunk.coords.x - _options.renderChunkRange;
			bool rightOut = chunk.coords.x > centerChunk.coords.x + _options.renderChunkRange;
			bool topOut = chunk.coords.y > centerChunk.coords.y + _options.renderChunkRange;
			bool bottomOut = chunk.coords.y < centerChunk.coords.y - _options.renderChunkRange;

			if (leftOut || rightOut || topOut || bottomOut) {
				DestroyChunk(chunk);
			}
		}
	}

	private Vector2Int GetChunkCoordsFromPosition (Vector2 position) {
		return Utils.GetCoordsFromPosition(position, _options.chunkSize * _cellSize);
	}

}
