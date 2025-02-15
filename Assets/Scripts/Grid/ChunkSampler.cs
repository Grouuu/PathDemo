using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSampler : MonoBehaviour {

	public static int chunkSizeInUnit = 10; // size of grid chunks in unit, equal of one _chunksPattern pixel

	/*
	 * TEXTURE OPTIONS
	 * read/write: yes
	 * mipmap: no
	 * format: RGBA 32 bits
	 */
	[SerializeField] private Texture2D _chunksPattern;
	[SerializeField] private ChunkData[] _chunksDataList;
	[SerializeField] private Vector2Int _startChunkCoords = new Vector2Int(1, 1);
	[SerializeField] private bool _isDebug = false;

	// TODO use pooling for obstacles
	// => only put placeholders in the chunks
	// => when a chunk is placed, replace them by pooled instances of obstacles
	// 2nd step
	// => check what chunks are loaded
	// => get what obstacles are used by them
	// => generate listed obstacles
	// => destroy non listed obstacles
	// => instantiations/destroy spread on different frames

	public List<ChunkGridPoint> GetChunksPoints (List<Vector2Int> chunkCoords) {
		List<ChunkGridPoint> points = new List<ChunkGridPoint>();

		foreach (Vector2Int coords in chunkCoords) {
			ChunkGridPoint point = GetPoint(coords);
			if (point != null) {
				points.Add(point);
			}
		}

		return points;
	}

	private ChunkGridPoint GetPoint (Vector2Int chunkCoords) {

		if (chunkCoords.x < 0) {
			// do not generate points on the left of the start position (out of chunks grid)
			return null;
		}

		if (_isDebug) {
			Debug.DrawRectangle((Vector2) chunkCoords * chunkSizeInUnit, new Vector2(chunkSizeInUnit, chunkSizeInUnit), Color.white);
		}

		Vector2Int texturePixelPosition = GetPixelCoords(chunkCoords);
		int texturePixelIndex = texturePixelPosition.x + texturePixelPosition.y * _chunksPattern.width;

		if (texturePixelIndex < 0 || texturePixelIndex > _chunksPattern.width * _chunksPattern.height) {
			Debug.LogWarning($"attempt to pick outside of the texture bounds (index: {texturePixelIndex}, x: {texturePixelPosition.x}, y: {texturePixelPosition.y}");
			return null;
		}

		var pixels = _chunksPattern.GetRawTextureData<Color32>();
		Color32 color = pixels[texturePixelIndex];

		if (color.a == 0) {
			// if transparent pixel, no chunk here
			return null;
		}

		Vector2 worldPosition = chunkCoords * chunkSizeInUnit + Vector2.one * chunkSizeInUnit * 0.5f; // center of the chunk
		ChunkGridPoint point = GetGridPoint(chunkCoords, worldPosition, color);

		return point;
	}

	private Vector2Int GetPixelCoords (Vector2Int chunkCoords) {

		int x = (chunkCoords.x + _startChunkCoords.x) % _chunksPattern.width;
		int y = (chunkCoords.y + _startChunkCoords.y) % _chunksPattern.height;

		if (x < 0) {
			x += _chunksPattern.width;
		}

		if (y < 0) {
			y += _chunksPattern.height;
		}

		return new Vector2Int(x, y);
	}

	private ChunkGridPoint GetGridPoint (Vector2Int chunkCoords, Vector2 centerWorldPosition, Color32 pixelData) {

		ChunkData chunkData = Array.Find(_chunksDataList, entry => entry.mapColor.r == pixelData.r && entry.mapColor.g == pixelData.g && entry.mapColor.b == pixelData.b);

		if (chunkData == null) {
			Debug.LogWarning($"no chunk data found with color {pixelData.r} {pixelData.g} {pixelData.b}");
			return null;
		}

		return new ChunkGridPoint(chunkData, centerWorldPosition, chunkCoords);
	}
}
