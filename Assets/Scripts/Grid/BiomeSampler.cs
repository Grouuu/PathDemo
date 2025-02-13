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


	// CONFIG
	private BiomeId[,] GRID = new BiomeId[5, 5] {
		{ BiomeId.None, BiomeId.None, BiomeId.None, BiomeId.None, BiomeId.None },
		{ BiomeId.None, BiomeId.None, BiomeId.None, BiomeId.None, BiomeId.None },
		{ BiomeId.Default, BiomeId.None, BiomeId.None, BiomeId.None, BiomeId.None },
		{ BiomeId.None, BiomeId.None, BiomeId.None, BiomeId.None, BiomeId.None },
		{ BiomeId.None, BiomeId.None, BiomeId.None, BiomeId.None, BiomeId.None }
	};
	// end CONFIG

	[SerializeField] private int _sampleUnitSize = 4; // size of grid cells in unit
	[SerializeField] private Vector2Int _biomeTextureSize = new Vector2Int(128, 128); // values should be a multiplier of _sampleSize AND power of 2
	[SerializeField] private BiomeData[] _biomesDataList;

	private readonly int _pixelPerUnit = 1; // ratio pixel/unit
	private readonly Vector2Int _startBiomeIndex = new Vector2Int(2, 0); // starting biome

	private Vector2Int _biomeSizeUnit;
	private Vector2Int _biomeSizeInCell;

	public int GetCellSize () {
		return _sampleUnitSize;
	}

	public List<GridPoint> GetCellsPoints (List<Vector2Int> cellsCoords) {
		List<GridPoint> points = new List<GridPoint>();

		foreach (Vector2Int coords in cellsCoords) {
			points.AddRange(GetCellPoints(coords));
		}

		return points;
	}

	private void Awake () {
		_biomeSizeUnit = _biomeTextureSize * _pixelPerUnit;
		_biomeSizeInCell = _biomeSizeUnit / _sampleUnitSize;
	}

	private List<GridPoint> GetCellPoints (Vector2Int cellCoords) {
		List<GridPoint> points = new List<GridPoint>();

		if (cellCoords.x < 0) {
			// do not generate points on the left of the start position
			return points;
		}

		//Vector2Int cellCenterPosition = cellCoords * _sampleUnitSize;
		Vector2Int biomeIndex = GetBiomeIndex(cellCoords);

		if (biomeIndex.x < 0 || biomeIndex.x > GRID.GetLength(0) || biomeIndex.y < 0 || biomeIndex.y > GRID.GetLength(1)) {
			// out of biomes grid
			return points;
		}

		BiomeId biomeId = GRID[biomeIndex.x, biomeIndex.y];

		if (biomeId == BiomeId.None) {
			return points;
		}

		BiomeData biomeData = Array.Find(_biomesDataList, data => data.Id == biomeId);

		if (biomeData == null) {
			Debug.LogWarning($"didn't found biome data for {biomeId}");
			return points;
		}

		Vector2Int anchorCellCoords = GetBiomeAnchorCellCoords(biomeIndex);
		Vector2Int biomeLocalCellCoords = GetBiomeLocalCellCoords(anchorCellCoords, cellCoords);
		Vector2Int startTexturePosition = biomeLocalCellCoords * _sampleUnitSize * _pixelPerUnit;
		var pixels = biomeData.patternData.GetRawTextureData<Color32>();

		for (int y = 0; y < _sampleUnitSize; y++) {
			for (int x = 0; x < _sampleUnitSize; x++) {

				int pixelIndex = (startTexturePosition.x + x) + (startTexturePosition.y + y) * _biomeTextureSize.x;

				if (pixelIndex < 0 || pixelIndex > _biomeTextureSize.x * _biomeTextureSize.y) {
					Debug.LogWarning($"attempt to pick outside of the texture bounds (index: {pixelIndex}, x: {x}, y: {y}");
					continue;
				}

				Color32 color = pixels[pixelIndex];

				Vector2 worldPosition = cellCoords * _sampleUnitSize + new Vector2(x * _pixelPerUnit, y * _pixelPerUnit);

				//Debug.DrawRectangle(worldPosition + Vector2.up * 0.95f + Vector2.right * 0.05f, Vector2.one * 0.95f, color);
			}
		}

		return points;
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

		bool isMultiplierOfSampleSize = _biomeTextureSize.x % _sampleUnitSize == 0 || _biomeTextureSize.y % _sampleUnitSize == 0;
		bool isPowerOf2 = Utils.IsPowerOfTwo(_biomeTextureSize.x) && Utils.IsPowerOfTwo(_biomeTextureSize.y);

		if (!isMultiplierOfSampleSize || !isPowerOf2) {
			Debug.LogError(
				$"Sample size and biome textures size should be a multiplier of the sample size and a power of 2" +
				$"(is multiplier of sample size: {isMultiplierOfSampleSize}, is power of 2: {isPowerOf2})"
			);
		}
	}
	#endif
}
