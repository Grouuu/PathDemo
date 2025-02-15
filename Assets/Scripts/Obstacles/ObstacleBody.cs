#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(MeshFilter))]
public class ObstacleBody : MonoBehaviour {

	public float Mass => _data.mass * _sizeFactor;
	public float Radius => _data.radius * _sizeFactor;
	public float RadiusGravity => _data.radiusGravity * _sizeFactor;

	[SerializeField] private ObstacleData _data;

	private SphereCollider _collider;
	private float _sizeFactor;

	private void Start () {
		Init();
	}

	private void OnEnable () {
		ObstacleController.ObstaclesInstances.Add(this);
	}

	private void OnDisable () {
		ObstacleController.ObstaclesInstances.Remove(this);
	}

	private void Init () {
		_collider = GetComponent<SphereCollider>();
		_sizeFactor = transform.localScale.x;
	}

	#if UNITY_EDITOR
	private void OnDrawGizmosSelected () {
		Handles.color = Color.red;
		Handles.DrawWireDisc(transform.position, Vector3.forward, Radius);
		Handles.DrawWireDisc(transform.position, Vector3.forward, RadiusGravity);
	}
	#endif
}
