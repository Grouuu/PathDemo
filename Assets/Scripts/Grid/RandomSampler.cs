using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Dependencies:
 * . ObstacleData
 * . DifficultyController
 */
public class RandomSampler : MonoBehaviour
{
	[SerializeField] private ObstacleData[] _obstaclesDataList;
	[SerializeField] [Range(0, 0.5f)] private float _blendPercent = 0.5f;

	private bool[] _allowedObstacles;

	public void SetDifficulty (float difficulty)
	{
		int currentIndex = (int) difficulty;
		float subProgress = difficulty % 1;

		if (currentIndex >= _allowedObstacles.Length)
		{
			// if we exceed the obstacle data list limit, keep the last one as the only active
			_allowedObstacles = new bool[_obstaclesDataList.Length];
			_allowedObstacles[^1] = true;
			return;
		}

		for (int index = 0; index < _allowedObstacles.Length; index++)
		{
			if (index == currentIndex)
			{
				_allowedObstacles[index] = true;
			}
			else if (index == currentIndex - 1 && subProgress <= _blendPercent)
			{
				// make a progressive transition between difficulties
				_allowedObstacles[index] = true;
			}
			else
			{
				_allowedObstacles[index] = false;
			}
		}
	}

	public float GetFactorAt (Vector2 position)
	{
		float noiseScale = 1f;
		float noiseValue = Mathf.Clamp01(Mathf.PerlinNoise(position.x * noiseScale, position.y * noiseScale)); // [0, 1]

		return noiseValue + 0.5f; // [.5, 1.5]
	}

	public ObstacleData GetDataAt (Vector2 position)
	{
		if (_obstaclesDataList.Length == 0)
		{
			return null;
		}

		List<int> allowedObstacleIndexes = new List<int>();
		
		for (int index = 0; index < _allowedObstacles.Length; index++)
		{
			if (_allowedObstacles[index])
			{
				allowedObstacleIndexes.Add(index);
			}
		}

		int randomIndex = UnityEngine.Random.Range(0, allowedObstacleIndexes.Count);
		int randomObstacleDataIndex = allowedObstacleIndexes[randomIndex];

		return _obstaclesDataList[randomObstacleDataIndex];
	}

	private void Awake ()
	{
		_allowedObstacles = new bool[_obstaclesDataList.Length];
	}

}
