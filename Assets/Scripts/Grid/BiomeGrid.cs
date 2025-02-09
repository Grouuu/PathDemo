using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BiomeSampler))]
public class BiomeGrid : MonoBehaviour {

	[SerializeField] private float _gridScaleComparedToTheScene = 2;
	[SerializeField] private int _cellSize = 5;
	[SerializeField] private float _minCellShiftBeforeUpdate = 1;

	private BiomeSampler _sampler;
	private Vector2Int _gridSize;				// world size
	private Int4 _coordsBounds;					// bottom-left and top-right coords
	private Vector2Int _halfCoordsBoundsSize;	// half of the number of cells on xy axis
	private Vector2 _centerPosition;			// world position of the center of the grid
	private bool _forceSpawnUpdate = true;		// force a spawn check

	private Dictionary<string, GridPoint> _points = new Dictionary<string, GridPoint>();

	public void UpdateGridSize () {
		Vector2Int sizeInCell = Vector2Int.CeilToInt(Utils.GetSceneSize() * _gridScaleComparedToTheScene / _cellSize);
		_gridSize = sizeInCell * _cellSize;
		_halfCoordsBoundsSize = _gridSize / _cellSize / 2;
		UpdateCoordsBounds();
		_forceSpawnUpdate = true;
	}

	public List<GridPoint> UpdatePoints (Vector2 centerPosition) {

		Vector2Int oldCoords = GetCoords(_centerPosition);
		Vector2Int newCoords = GetCoords(centerPosition);
		Vector2Int coordsShift = GetGridCoordsShift(oldCoords, newCoords);
		bool enoughShift = Mathf.Abs(coordsShift.x) >= _minCellShiftBeforeUpdate || Mathf.Abs(coordsShift.y) >= _minCellShiftBeforeUpdate;

		if (!enoughShift && !_forceSpawnUpdate) {
			return new List<GridPoint>();
		}

		_forceSpawnUpdate = false;
		_centerPosition = centerPosition;

		UpdateCoordsBounds();
		ClearOutOfBoundsPoints();

		List<Vector2Int> freeCellsCoords = GetFreeCellsCoords();
		List<GridPoint> spawnPoints = _sampler.GetCellsPoints(freeCellsCoords, _cellSize);

		foreach (GridPoint point in spawnPoints) {
			_points.Add(GetCellId(point.cellCoords), point);
		}

		return spawnPoints;
	}

	private void Awake () {
		_sampler = GetComponent<BiomeSampler>();
	}

	private void Start () {
		UpdateGridSize();
	}

	private void UpdateCoordsBounds () {
		_coordsBounds = GetCoordsBounds(_centerPosition);
	}

	private List<Vector2Int> GetFreeCellsCoords () {
		List<Vector2Int> cellsCoords = new List<Vector2Int>();

		for (int coordX = _coordsBounds.x; coordX <= _coordsBounds.z; coordX++) {
			for (int coordY = _coordsBounds.y; coordY <= _coordsBounds.w; coordY++) {

				Vector2Int coords = new Vector2Int(coordX, coordY);
				string key = GetCellId(coords);

				if (!_points.ContainsKey(key)) {
					cellsCoords.Add(coords);
				}
			}
		}

		return cellsCoords;
	}

	private void ClearOutOfBoundsPoints () {

		List<string> deleteKeys = new List<string>();

		foreach (KeyValuePair<string, GridPoint> entry in _points) {
			if (!isCoordsInGrid(entry.Value.cellCoords, _coordsBounds)) {
				deleteKeys.Add(entry.Key);
			}
		}

		foreach (string key in deleteKeys) {
			_points[key].Destroy();
			_points.Remove(key);
		}
	}

	private Vector2Int GetGridCoordsShift (Vector2Int oldCoords, Vector2Int newCoords) {
		int horizontalShift = (int) System.Math.Sign(newCoords.x - oldCoords.x) * Mathf.Abs(oldCoords.x - newCoords.x);
		int verticalShift = (int) System.Math.Sign(newCoords.y - oldCoords.y) * Mathf.Abs(oldCoords.y - newCoords.y);
		return new Vector2Int(horizontalShift, verticalShift);
	}

	private Vector2Int GetCoords (Vector2 position) {
		// centered origin
		return new Vector2Int(
			Mathf.RoundToInt(position.x / _cellSize),
			Mathf.RoundToInt(position.y / _cellSize)
		);
	}

	private Int4 GetCoordsBounds (Vector2 position) {
		Vector2Int coords = GetCoords(position);
		Vector2Int bottomLeft = coords - _halfCoordsBoundsSize;
		Vector2Int topRight = coords + _halfCoordsBoundsSize;
		return new Int4(
			bottomLeft.x,
			bottomLeft.y,
			topRight.x,
			topRight.y
		);
	}

	private bool isCoordsInGrid (Vector2Int coords, Int4 bounds) {
		return isCoordsInGrid(coords.x, coords.y, bounds);
	}

	private string GetCellId (Vector2Int coords) {
		return $"{coords.x}_{coords.y}";
	}

	private bool isCoordsInGrid (int coordX, int coordY, Int4 bounds) {
		return coordX > bounds.x
			&& coordX < bounds.z
			&& coordY > bounds.y
			&& coordY < bounds.w
		;
	}
}
