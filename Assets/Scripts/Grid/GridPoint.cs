using UnityEngine;

public delegate void OnPointDestroy (GridPoint point, ObstacleBody body);

public class GridPoint {

	public event OnPointDestroy OnDestroy;

	public ObstacleBody body;
	public Vector2 position;
	public float reservedDistance;
	public float sizeFactor;
	public bool isRender;

	public GridPoint (Vector2 position, float reservedDistance, float sizeFactor, bool isRender = true) {
		this.position = position;
		this.reservedDistance = reservedDistance;
		this.sizeFactor = sizeFactor;
		this.isRender = isRender;
	}

	public void Destroy () {
		if (OnDestroy != null) {
			OnDestroy(this, body);
		}
	}
}
