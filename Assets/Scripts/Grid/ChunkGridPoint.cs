using UnityEngine;

public delegate void OnPointDestroy (ChunkGridPoint point, ChunkBody body);

public class ChunkGridPoint {

	public event OnPointDestroy OnDestroy;
	public ChunkBody body;

	public ChunkData chunkData;
	public Vector2 position;
	public Vector2Int chunkCoords;

	public ChunkGridPoint (
		ChunkData chunkData,
		Vector2 position,
		Vector2Int chunkCoords
	) {
		this.chunkData = chunkData;
		this.position = position;
		this.chunkCoords = chunkCoords;
	}

	public void Destroy () {
		if (OnDestroy != null) {
			OnDestroy(this, body);
		}
	}
}