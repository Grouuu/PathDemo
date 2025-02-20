using UnityEngine;

/*
 * Dependencies:
 * . ObstacleData
 */
public class RandomSampler : MonoBehaviour
{
	[SerializeField] private ObstacleData[] _obstaclesDataList;

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

		return _obstaclesDataList[0]; // TODO PLACEHOLER
	}

}
