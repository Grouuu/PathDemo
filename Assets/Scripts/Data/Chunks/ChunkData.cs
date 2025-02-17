using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Chunk Data", fileName = "ChunkData")]
public class ChunkData : ScriptableObject {

	public ChunkId Id;
	public Color32 mapColor;
	public ChunkBody prefab;

}

[CustomEditor(typeof(ChunkData))]
public class ChunkDataEditor : Editor {

	public override void OnInspectorGUI () {

		ChunkData data = target as ChunkData;

		GUIStyle styles = new GUIStyle(EditorStyles.miniBoldLabel) {
			fontStyle = FontStyle.Italic
		};

		EditorGUILayout.LabelField("Map Color Info", $"{data.mapColor.r} {data.mapColor.g} {data.mapColor.b}", styles);

		base.OnInspectorGUI();

	}
}
