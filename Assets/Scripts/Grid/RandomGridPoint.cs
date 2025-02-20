using UnityEngine;

public delegate void OnGridPointDestroy (RandomGridPoint point);

/*
 * Dependencies:
 * . ObstacleBody
 * . ObstacleData
 */
public class RandomGridPoint
{
	public event OnGridPointDestroy OnDestroy;

	public ObstacleBody body;

	public ObstacleData data;
	public Vector2 position;
	public Vector2Int coords;
	public float reservedDistance;
	public float sizeFactor;
	public bool isRender;
	public bool isFirst;

	public RandomGridPoint (
		ObstacleData data,
		Vector2 position,
		Vector2Int chunkCoords,
		float reservedDistance,
		float sizeFactor,
		bool isRender = true,
		bool isFirst = false
	)
	{
		this.data = data;
		this.position = position;
		this.coords = chunkCoords;
		this.reservedDistance = reservedDistance;
		this.sizeFactor = sizeFactor;
		this.isRender = isRender;
		this.isFirst = isFirst;
	}

	public void Destroy ()
	{
		OnDestroy?.Invoke(this);
	}
}