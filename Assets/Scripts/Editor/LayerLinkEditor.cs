using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayerLink))]
public class LayerLinkEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("Focus"))
		{
			LayerLink link = target as LayerLink;
			link.Ping();
		}
	}

}
