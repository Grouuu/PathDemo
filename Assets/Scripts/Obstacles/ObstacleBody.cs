using UnityEngine;

/*
 * Dependencies:
 * . ObstacleData
 * . ObstacleController
 */
[RequireComponent(typeof(SphereCollider))]
public class ObstacleBody : MonoBehaviour
{
	public float Mass => _data.mass * _sizeFactor;
	public float Radius => _data.radius * _sizeFactor;
	public float RadiusGravity => _data.radiusGravity * _sizeFactor;

	[SerializeField] private ObstacleData _data;

	private SphereCollider _collider;
	private float _sizeFactor = 1;

	public void SetSizeFactor (float sizeFactor)
	{
		_sizeFactor = sizeFactor;
		transform.localScale = new Vector3(_sizeFactor, _sizeFactor, _sizeFactor);
	}

	private void Start ()
	{
		Init();
	}

	private void OnEnable ()
	{
		ObstacleController.ObstaclesInstances.Add(this);
	}

	private void OnDisable ()
	{
		ObstacleController.ObstaclesInstances.Remove(this);
	}

	private void Init ()
	{
		_collider = GetComponent<SphereCollider>();
	}

#if UNITY_EDITOR

	private void OnDrawGizmosSelected ()
	{
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, Radius);
		UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, RadiusGravity);
	}

#endif

}
