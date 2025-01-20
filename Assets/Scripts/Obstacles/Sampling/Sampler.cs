using System.Collections.Generic;
using UnityEngine;

public static class Sampler {

	private static readonly float SQRT_2 = Mathf.Sqrt(2);

	public static List<Vector2> GeneratePoints (float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30) {

		// TODO
		// . make the radius dependant of the perlin noise
		// . add the size dependant of the the perlin noise (the more radius, the more size)

		float cellSize = radius / SQRT_2;
		int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
		List<Vector2> points = new List<Vector2>(); // List<[x, y, size]>
		List<Vector2> spawnPoints = new List<Vector2>();

		spawnPoints.Add(sampleRegionSize / 2);

		int safeBreakCount = 0;
		while (spawnPoints.Count > 0) {
			int spawnIndex = Random.Range(0, spawnPoints.Count);
			Vector2 spawnCenter = spawnPoints[spawnIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < numSamplesBeforeRejection; i++) {
				float angle = Random.value * Mathf.PI * 2;
				Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				Vector2 candidate = spawnCenter + direction * Random.Range(radius, radius * 2);

				if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid)) {
					points.Add(candidate);
					spawnPoints.Add(candidate);
					int[] gridPosition = GetGridPosition(candidate, cellSize);
					grid[gridPosition[0], gridPosition[1]] = points.Count;
					candidateAccepted = true;
					break;
				}
			}

			if (!candidateAccepted) {
				// remove the current root point because no solution was found for it
				spawnPoints.RemoveAt(spawnIndex);
			}

			safeBreakCount++;
			if (safeBreakCount >= 10000) {
				Debug.Log("Infinite sample loop suspected");
				break;
			}
		}

		return points;
	}

	private static bool IsValid (Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid) {
		bool isInOfBounds = candidate.x >= 0 && candidate.x <= sampleRegionSize.x && candidate.y >= 0 && candidate.y <= sampleRegionSize.y;

		if (!isInOfBounds) {
			return false;
		}

		int[] gridPosition = GetGridPosition(candidate, cellSize);
		int cellX = gridPosition[0];
		int cellY = gridPosition[1];

		// loop on 5x5 cells centered on the candidate
		int searchStartX = Mathf.Max(0, cellX - 2);
		int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
		int searchStartY = Mathf.Max(0, cellY - 2);
		int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

		for (int x = searchStartX; x <= searchEndX; x++) {
			for (int y = searchStartY; y <= searchEndY; y++) {

				int pointIndex = grid[x, y] - 1;

				if (pointIndex != -1) {
					// sqrMagnitude is faster than magnitude
					float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
					if (sqrDst < radius * radius) {
						return false;
					}
				}
			}
		}

		return true;
	}

	private static int[] GetGridPosition (Vector2 position, float cellSize) {
		return new int[] { (int) (position.x / cellSize), (int) (position.y / cellSize) };
	}

}
