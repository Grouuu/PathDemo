
public struct GridOptions {

	public float minDistance;
	public float spawnRadius;
	public int chunkSize;
	public int renderChunkRange;

	public GridOptions (int renderChunkRange, int chunkSize, float minDistance, float spawnRadius) {
		this.renderChunkRange = renderChunkRange;
		this.chunkSize = chunkSize;
		this.minDistance = minDistance;
		this.spawnRadius = spawnRadius;
	}
}
