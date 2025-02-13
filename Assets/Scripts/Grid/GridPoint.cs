using UnityEngine;

public delegate void OnPointDestroy (GridPoint point, ObstacleBody body);

public class GridPoint {

	public event OnPointDestroy OnDestroy;
	public ObstacleBody body;

	public ObstacleId obstacleId;
	public Vector2 position;
	public Vector2Int cellCoords;
	public float sizeFactor;

	public GridPoint (
		ObstacleId obstacleId,
		Vector2 position,
		Vector2Int cellCoords,
		float sizeFactor
	) {
		this.obstacleId = obstacleId;
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