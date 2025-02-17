using UnityEngine;

public class ChunkGridPoint : IGridPoint<ChunkBody> {

	public OnGridPointDestroy<ChunkBody> OnDestroy { get; set; }
	public ChunkBody body { get; set; }

	public ScriptableObject data { get; set; }
	public Vector2 position { get; set; }
	public Vector2Int chunkCoords { get; set; }
	public float reservedDistance { get; set; }	// not used
	public float sizeFactor { get; set; }       // not used
	public bool isRender { get; set; }          // not used
	public bool isFirst { get; set; }           // not used

	public ChunkGridPoint (
		ChunkData chunkData,
		Vector2 position,
		Vector2Int chunkCoords
	) {
		this.data = chunkData;
		this.position = position;
		this.chunkCoords = chunkCoords;
	}

	public void Destroy () {
		OnDestroy?.Invoke(this, body);
	}
}