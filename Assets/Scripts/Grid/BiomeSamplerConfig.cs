
using System.Collections.Generic;
using UnityEngine;

public static class BiomeSamplerConfig {

	public static BiomeId[,] BiomeGrid = new BiomeId[5, 5] {
		{ BiomeId.None,		BiomeId.None,	BiomeId.None,	BiomeId.None,	BiomeId.None },
		{ BiomeId.None,		BiomeId.None,	BiomeId.None,	BiomeId.None,	BiomeId.None },
		{ BiomeId.Default,	BiomeId.None,	BiomeId.None,	BiomeId.None,	BiomeId.None },
		{ BiomeId.None,		BiomeId.None,	BiomeId.None,	BiomeId.None,	BiomeId.None },
		{ BiomeId.None,		BiomeId.None,	BiomeId.None,	BiomeId.None,	BiomeId.None },
	};

	public static Dictionary<string, ObstacleId> MapColors = new Dictionary<string, ObstacleId>() {
		{  "255_0_0", ObstacleId.Default }
	};

}
