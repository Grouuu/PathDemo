using UnityEditor;

[CustomEditor(typeof(BiomeSampler))]
public class BiomeSamplerInspector : Editor {

	public override void OnInspectorGUI () {

		/* Test
		if (GUILayout.Button("Edit biomes grid")) {
			BiomeSamplerWindow.target = (BiomeSampler) target;
			BiomeSamplerWindow.ShowWindow();
		}
		*/

		base.OnInspectorGUI();
	}

}
