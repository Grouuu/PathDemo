using UnityEngine;

public class LayerLink : MonoBehaviour
{
	public string name;
	public Transform target;

	public void Ping ()
	{
#if UNITY_EDITOR

		if (target != null) {
			UnityEditor.Selection.activeTransform = target;
		}

#endif
	}

}
