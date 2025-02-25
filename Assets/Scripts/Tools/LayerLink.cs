using UnityEngine;

public class LayerLink : MonoBehaviour
{
	[SerializeField] private string _name;
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
