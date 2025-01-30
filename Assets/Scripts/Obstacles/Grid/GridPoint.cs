using UnityEngine;

public delegate void OnPointDestroy (GridPoint point, ObstacleBody body);

public class GridPoint {

	public event OnPointDestroy OnDestroy;
	public ObstacleBody body;

	public Vector2 position;
	public Vector2Int cellCoords;
	public float reservedDistance;
	public float sizeFactor;
	public bool isRender;
	public bool isFirst;

	public GridPoint (
		Vector2 position,
		Vector2Int cellCoords,
		float reservedDistance,
		float sizeFactor,
		bool isRender = true,
		bool isFirst = false
	) {
		this.position = position;
		this.cellCoords = cellCoords;
		this.reservedDistance = reservedDistance;
		this.sizeFactor = sizeFactor;
		this.isRender = isRender;
		this.isFirst = isFirst;
	}

	public void Destroy () {
		if (OnDestroy != null) {
			OnDestroy(this, body);
		}
	}
}