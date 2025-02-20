using UnityEngine;

public class FollowTarget : MonoBehaviour
{
	[SerializeField] private Transform _target;
	[SerializeField] private Vector3 _offset;

	private void Update()
	{
		transform.position = _target.position + _offset;
	}

}
