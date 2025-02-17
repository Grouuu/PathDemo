using UnityEngine;

public class RandomGridPoint : IGridPoint<ObstacleBody> {

	public OnGridPointDestroy<ObstacleBody> OnDestroy { get; set; }
	public ObstacleBody body { get; set; }

	public ScriptableObject data { get; set; }
	public Vector2 position { get; set; }
	public Vector2Int coords { get; set; }
	public float reservedDistance { get; set; }
	public float sizeFactor { get; set; }
	public bool isRender { get; set; }
	public bool isFirst { get; set; }

	public RandomGridPoint (
		ScriptableObject data,
		Vector2 position,
		Vector2Int chunkCoords,
		float reservedDistance,
		float sizeFactor,
		bool isRender,
		bool isFirst
	) {
		this.data = data;
		this.position = position;
		this.coords = chunkCoords;
		this.reservedDistance = reservedDistance;
		this.sizeFactor = sizeFactor;
		this.isRender = isRender;
		this.isFirst = isFirst;
	}

	public void Destroy () {
		OnDestroy?.Invoke(this, body);
	}
}