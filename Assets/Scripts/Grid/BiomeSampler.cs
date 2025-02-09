using System.Collections.Generic;
using UnityEngine;

public class BiomeSampler : MonoBehaviour {

	[SerializeField] private BiomeData[] _biomeData;

	public List<GridPoint> GetCellsPoints (List<Vector2Int> cellsCoords, int cellSize) {

		List<GridPoint> points = new List<GridPoint>();

		foreach (Vector2Int coords in cellsCoords) {
			points.AddRange(GetPoints(coords, cellSize));
		}

		return points;
	}

	private List<GridPoint> GetPoints (Vector2Int coords, int cellSize) {
		List<GridPoint> points = new List<GridPoint>();

		Vector2 cellCenter = coords * cellSize;

		// DEBUG
		points.Add(new GridPoint(cellCenter, coords, 1, 1));

		return points;
	}

}
