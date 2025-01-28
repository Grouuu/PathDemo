using System.Collections.Generic;
using UnityEngine;

public class Grid {

	public Dictionary<string, GridChunk> chunks { get; private set; }
	public float cellSize => _cellSize;

	private GridOptions _options;
	private float _cellSize;
	private GridChunk _currentCenterChunk;

	public Grid(GridOptions options) {
		_options = options;

		_cellSize = _options.minDistance / Utils.SQRT_2;
		chunks = new Dictionary<string, GridChunk>();
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

		foreach (KeyValuePair<string, GridChunk> entry in chunks) {
			if (
				(onlyOldBorderChunks && entry.Value.state == GridChunkState.OldBorder)
				|| !onlyOldBorderChunks
			) {
				points.AddRange(entry.Value.GetPoints());
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
		GridChunk chunk;

		for (int x = centerChunk.coords.x - range; x <= centerChunk.coords.x + range; x++) {
			for (int y = centerChunk.coords.y - range; y <= centerChunk.coords.y + range; y++) {

				chunk = GetChunk(x, y);

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
		string chunkId = GetChunkId(coords);

		if (!chunks.ContainsKey(chunkId)) {
			return null;
		}

		return chunks[chunkId];
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
		chunks.Add(GetChunkId(coords), chunk);
		return chunk;
	}

	private void DestroyChunk(GridChunk chunk) {
		chunk.Destroy();
		chunks.Remove(GetChunkId(chunk.coords));
	}

	private bool HasChunk (Vector2Int coords) {
		return chunks.ContainsKey(GetChunkId(coords));
	}

	private void ClearOutOfRenderChunks (Vector2 position) {
		GridChunk centerChunk = GetChunk(position);

		if (centerChunk == null) {
			return;
		}

		List<GridChunk> removedChunks = new List<GridChunk>();

		foreach (KeyValuePair<string, GridChunk> entry in chunks) {
			bool leftOut = entry.Value.coords.x < centerChunk.coords.x - _options.renderChunkRange;
			bool rightOut = entry.Value.coords.x > centerChunk.coords.x + _options.renderChunkRange;
			bool topOut = entry.Value.coords.y > centerChunk.coords.y + _options.renderChunkRange;
			bool bottomOut = entry.Value.coords.y < centerChunk.coords.y - _options.renderChunkRange;

			if (leftOut || rightOut || topOut || bottomOut) {
				removedChunks.Add(entry.Value);
			}
		}

		foreach (GridChunk chunk in removedChunks) {
			DestroyChunk(chunk);
		}
	}

	private Vector2Int GetChunkCoordsFromPosition (Vector2 position) {
		return Utils.GetCoordsFromPosition(position, _options.chunkSize * _cellSize);
	}

	private string GetChunkId (Vector2 coords) {
		return coords.x + " " + coords.y;
	}

}
