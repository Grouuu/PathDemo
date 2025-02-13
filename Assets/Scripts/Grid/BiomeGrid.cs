using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BiomeSampler))]
public class BiomeGrid : MonoBehaviour {

	[SerializeField] private float _gridScaleComparedToTheScene = 2;
	[SerializeField] private float _minCellShiftBeforeUpdate = 1;

	private BiomeSampler _sampler;
	private Vector2Int _gridSizeInCell;         // number of cells on xy axis
	private int _cellSizeInUnit;				// unit size of each grid cell
	private Int4 _coordsBoundsInCell;           // bottom-left and top-right cells coords
	private Vector2Int _halfGridSizeInCell;		// half of the number of cells on xy axis
	private Vector2 _centerPosition;			// world position of the center of the grid
	private bool _forceSpawnUpdate = true;		// force a spawn check

	private Dictionary<string, GridPoint> _points = new Dictionary<string, GridPoint>();

	public void UpdateGridSize () {
		_gridSizeInCell = Vector2Int.CeilToInt(Utils.GetSceneSize() * _gridScaleComparedToTheScene / _cellSizeInUnit);
		_halfGridSizeInCell = _gridSizeInCell / 2;
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
		List<GridPoint> spawnPoints = _sampler.GetCellsPoints(freeCellsCoords);

		foreach (GridPoint point in spawnPoints) {
			_points.Add(GetCellId(point.cellCoords), point);
		}

		return spawnPoints;
	}

	private void Awake () {
		_sampler = GetComponent<BiomeSampler>();
		_cellSizeInUnit = _sampler.GetCellSize();
	}

	private void Start () {
		UpdateGridSize();
	}

	private void UpdateCoordsBounds () {
		_coordsBoundsInCell = GetCoordsBoundsInCell(_centerPosition);
	}

	private List<Vector2Int> GetFreeCellsCoords () {
		List<Vector2Int> cellsCoords = new List<Vector2Int>();

		for (int coordX = _coordsBoundsInCell.x; coordX <= _coordsBoundsInCell.z; coordX++) {
			for (int coordY = _coordsBoundsInCell.y; coordY <= _coordsBoundsInCell.w; coordY++) {

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
			if (!isCoordsInGrid(entry.Value.cellCoords, _coordsBoundsInCell)) {
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
		// bottom-left origin
		int x = Mathf.FloorToInt(position.x / _cellSizeInUnit);
		int y = Mathf.FloorToInt(position.y / _cellSizeInUnit);
		return new Vector2Int(x, y);
	}

	private Int4 GetCoordsBoundsInCell (Vector2 position) {
		Vector2Int coords = GetCoords(position);
		Vector2Int bottomLeft = coords - _halfGridSizeInCell;
		Vector2Int topRight = coords + _halfGridSizeInCell;
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
