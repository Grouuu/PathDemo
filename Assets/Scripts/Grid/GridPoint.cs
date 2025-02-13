using UnityEngine;

public delegate void OnPointDestroy (GridPoint point, ObstacleBody body);

public class GridPoint {

	public event OnPointDestroy OnDestroy;
	public ObstacleBody body;

	public ObstacleData obstacleData;
	public Vector2 position;
	public Vector2Int cellCoords;
	public float sizeFactor;

	public GridPoint (
		ObstacleData obstacleData,
		Vector2 position,
		Vector2Int cellCoords,
		float sizeFactor
	) {
		this.obstacleData = obstacleData;
		this.position = position;
		this.cellCoords = cellCoords;
		this.sizeFactor = sizeFactor;
	}

	public void Destroy () {
		if (OnDestroy != null) {
			OnDestroy(this, body);
		}
	}
}