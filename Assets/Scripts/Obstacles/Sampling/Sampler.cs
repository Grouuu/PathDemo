using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Inspired by https://github.com/SebLague/Poisson-Disc-Sampling/blob/master/Poisson%20Disc%20Sampling%20E01/PoissonDiscSampling.cs
 * Require:
 * . Grid
 */
public class Sampler {

	private GridOptions _options;
	private Grid _grid;
	private Func<Vector2, float> _getNoiseAt;

	public Sampler (Grid grid, GridOptions options, Func<Vector2, float> getNoiseAt) {
		_options = options;
		_grid = grid;
		_getNoiseAt = getNoiseAt;
	}

	public List<GridPoint> GetNewPoints (Vector2 position) {
		List<GridPoint> points = new List<GridPoint>();

		bool isDirty = _grid.UpdateChunks(position);

		if (!isDirty) {
			return points;
		}

		List<GridPoint> borderPoints = _grid.GetPoints(true);
		List<GridPoint> spawnPoints = new List<GridPoint>();

		if (borderPoints.Count == 0) {
			// added to the grid but not to the scene
			// force empty space around the player at start
			GridPoint seedPoint = new GridPoint(position, _options.spawnRadius, 1, false);
			_grid.AddPoint(seedPoint);
			borderPoints.Add(seedPoint);
		}

		spawnPoints.AddRange(borderPoints);

		int safeBreakCount = 0;
		int numSamplesBeforeRejection = 30;
		GridPoint candidate;
		GridPoint spawnCenter;
		int spawnIndex;
		bool candidateAccepted;

		while (spawnPoints.Count > 0) {

			spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
			spawnCenter = spawnPoints[spawnIndex];
			candidateAccepted = false;

			for (int i = 0; i < numSamplesBeforeRejection; i++) {

				candidate = GetRandomNextCandidate(spawnCenter.position, spawnCenter.reservedDistance);

				if (_grid.IsPointValid(candidate)) {
					_grid.AddPoint(candidate);
					points.Add(candidate);
					spawnPoints.Add(candidate);
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

	// DEBUG
	public void UpdateDebug() {
		Dictionary<string, GridChunk> chunks = _grid.chunks;
		
		foreach (KeyValuePair<string, GridChunk> entry in chunks) {
			Utils.DrawDebugRectangle(
				new Rect(
					entry.Value.centerPosition + new Vector2(-_options.chunkSize * _grid.cellSize / 2, -_options.chunkSize * _grid.cellSize / 2),
					new Vector2(_options.chunkSize * _grid.cellSize, _options.chunkSize * _grid.cellSize)
				),
				entry.Value.state == GridChunkState.Border ? Color.red : (entry.Value.state == GridChunkState.OldBorder ? Color.cyan : Color.green),
				0
			);
		}
	}

	private GridPoint GetRandomNextCandidate (Vector2 center, float spawnMinDistance) {
		float minLocalRadius = spawnMinDistance;
		float maxLocalRadius = Mathf.Max(_options.spawnRadius, spawnMinDistance);
		float radius = UnityEngine.Random.Range(minLocalRadius, maxLocalRadius);
		float angle = UnityEngine.Random.value * Mathf.PI * 2;
		Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
		Vector2 position = center + direction * radius;
		float factor = _getNoiseAt(center);
		float reservedDistance = _options.minDistance * factor;

		return new GridPoint(position, reservedDistance, factor);
	}

}
