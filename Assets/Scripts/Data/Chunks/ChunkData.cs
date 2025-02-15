using UnityEngine;

[CreateAssetMenu(menuName = "Data/Chunk Data", fileName = "ChunkData")]
public class ChunkData : ScriptableObject {

	public ChunkId Id;
	public Color32 mapColor;
	public ChunkBody prefab;

}
