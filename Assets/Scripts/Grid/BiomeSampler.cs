using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnBiomeSamplerListUpdate();

public class BiomeSampler : MonoBehaviour {

	/*// EDITOR test
	public BiomeData[] biomeDataList {          // inspector input
		get => biomeDataList;
		set {
			biomeDataList = value;
			OnBiomeSamplerListUpdate();
		}
	}
	[HideInInspector] public int[] biomesGrid;	// tool output
	[HideInInspector] public event OnBiomeSamplerListUpdate OnBiomeSamplerListUpdate;
	// end EDITOR */

	[SerializeField] private int _cellSizeInUnit = 1; // size of grid cells in unit (one obstacle by cell max)
	[SerializeField] private Vector2Int _biomeTextureSize = new Vector2Int(128, 128); // values should be a multiplier of _sampleSize AND power of 2
	[SerializeField] private BiomeData[] _biomesDataList;
	[SerializeField] private ObstacleData[] _obstaclesDataList;

	private readonly int _unitPerPixel = 1; // how many pixels by cell
	private readonly Vector2Int _startBiomeIndex = new Vector2Int(2, 0); // starting biome (x and y inverted)

	private Vector2Int _biomeSizeInUnit;
	private Vector2Int _biomeSizeInCell;

	public int GetCellSize () {
		return _cellSizeInUnit;
	}

	public List<GridPoint> GetCellsPoints (List<Vector2Int> cellsCoords) {
		List<GridPoint> points = new List<GridPoint>();

		foreach (Vector2Int coords in cellsCoords) {
			GridPoint point = GetPoint(coords);
			if (point != null) {
				points.Add(point);
			}
		}

		return points;
	}

	private void Awake () {
		_biomeSizeInUnit = _biomeTextureSize * _unitPerPixel;
		_biomeSizeInCell = _biomeSizeInUnit / _cellSizeInUnit;
	}

	private GridPoint GetPoint (Vector2Int cellCoords) {

		if (cellCoords.x < 0) {
			// do not generate points on the left of the start position (out of biomes grid)
			return null;
		}

		Vector2Int biomeIndex = GetBiomeIndex(cellCoords);

		if (biomeIndex.x < 0 || biomeIndex.x > BiomeSamplerConfig.BiomeGrid.GetLength(0) || biomeIndex.y < 0 || biomeIndex.y > BiomeSamplerConfig.BiomeGrid.GetLength(1)) {
			// out of biomes grid
			return null;
		}

		BiomeId biomeId = BiomeSamplerConfig.BiomeGrid[biomeIndex.x, biomeIndex.y];

		if (biomeId == BiomeId.None) {
			// nothing here
			return null;
		}

		BiomeData biomeData = Array.Find(_biomesDataList, data => data.Id == biomeId);

		if (biomeData == null) {
			Debug.LogWarning($"didn't found biome data for {biomeId}");
			return null;
		}

		Vector2Int biomeAnchorCellCoords = GetBiomeAnchorCellCoords(biomeIndex);
		Vector2Int biomeLocalCellCoords = GetBiomeLocalCellCoords(biomeAnchorCellCoords, cellCoords);
		Vector2Int texturePixelPosition = biomeLocalCellCoords * _cellSizeInUnit * _unitPerPixel;
		int texturePixelIndex = texturePixelPosition.x + texturePixelPosition.y * _biomeTextureSize.x;

		if (texturePixelIndex < 0 || texturePixelIndex > _biomeTextureSize.x * _biomeTextureSize.y) {
			Debug.LogWarning($"attempt to pick outside of the texture bounds (index: {texturePixelIndex}, x: {texturePixelPosition.x}, y: {texturePixelPosition.y}");
			return null;
		}

		var pixels = biomeData.patternData.GetRawTextureData<Color32>();
		Color32 color = pixels[texturePixelIndex];

		if (color.a == 0) {
			// nothing here
			return null;
		}

		Vector2 worldPosition = cellCoords * _cellSizeInUnit + Vector2.one * _unitPerPixel / 2; // center of the cell
		GridPoint point = GetGridPoint(cellCoords, worldPosition, color);

		return point;
	}

	private GridPoint GetGridPoint (Vector2Int cellCoords, Vector2 centerWorldPosition, Color32 data) {

		ObstacleData obstacleData = Array.Find(_obstaclesDataList, entry => entry.mapColor.r == data.r && entry.mapColor.g == data.g && entry.mapColor.b == data.b);

		if (obstacleData == null) {
			return null;
		}

		Vector2 position = centerWorldPosition;
		float sizeFactor = (float) data.a / 255 + 0.5f; // [.5, 1.5]
		return new GridPoint(obstacleData, position, cellCoords, sizeFactor);
	}

	private Vector2Int GetBiomeIndex (Vector2Int cellCoords) {
		int offsetYInBiome = Mathf.FloorToInt((float) cellCoords.y / _biomeSizeInCell.y);
		// convert orientation x to y (biomes grid x is cells grid y, and y is cells grid x)
		// biomes grid y and cells grid y orientation are opposite
		// both make it easier to visualize the biomes grid
		int x = _startBiomeIndex.x - offsetYInBiome;
		int y = _startBiomeIndex.y + Mathf.FloorToInt((float) cellCoords.x / _biomeSizeInCell.x);
		return new Vector2Int(x, y);
	}

	private Vector2Int GetBiomeAnchorCellCoords (Vector2Int biomeIndex) {
		// bottom-left corner
		int x = biomeIndex.y * _biomeSizeInCell.x;
		int y = -biomeIndex.x * _biomeSizeInCell.y + _startBiomeIndex.x * _biomeSizeInCell.y;
		return new Vector2Int(x, y);
	}

	private Vector2Int GetBiomeLocalCellCoords (Vector2Int anchorCellCoords, Vector2Int cellCoords) {
		return cellCoords - anchorCellCoords;
	}

	#if UNITY_EDITOR
	private void OnValidate () {

		bool isMultiplierOfSampleSize = _biomeTextureSize.x % _cellSizeInUnit == 0 || _biomeTextureSize.y % _cellSizeInUnit == 0;
		bool isPowerOf2 = Utils.IsPowerOfTwo(_biomeTextureSize.x) && Utils.IsPowerOfTwo(_biomeTextureSize.y);

		if (!isMultiplierOfSampleSize || !isPowerOf2) {
			Debug.LogError(
				$"Sample size and biome textures size should be a multiplier of the sample size and a power of 2 " +
				$"(is multiplier of sample size: {isMultiplierOfSampleSize}, is power of 2: {isPowerOf2})"
			);
		}
	}
	#endif
}
