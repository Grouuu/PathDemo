using UnityEngine;

[CreateAssetMenu(menuName = "Data/Biome Data", fileName = "BiomeData")]
public class BiomeData : ScriptableObject {

	/*
	 * TEXTURE OPTIONS
	 * read/write: yes
	 * mipmap: no
	 * format: RGBA 32 bits
	 * size: 128x128
	 */

	public BiomeId Id;
	public Texture2D patternData;
	public ObstacleBody prefab;

}
