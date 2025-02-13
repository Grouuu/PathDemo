#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(MeshFilter))]
public class ObstacleBody : MonoBehaviour {

	public float Mass => _data.Mass * _sizeFactor;
	public float Radius => _data.Radius * _sizeFactor;
	public float RadiusGravity => _data.RadiusGravity * _sizeFactor;

	[SerializeField] private ObstacleData _data;

	private SphereCollider _collider;
	private MeshFilter _mesh;
	private float _sizeFactor = 1;

	public void SetData(ObstacleData data) {
		_data = data;
		UpdateSize();
	}

	public void SetSizefactor(float sizeFactor) {
		_sizeFactor = sizeFactor;
		UpdateSize();
	}

	private void Awake () {
		Init();
	}

	private void OnEnable () {
		ObstacleController.ObstaclesInstances.Add(this);
	}

	private void OnDisable () {
		ObstacleController.ObstaclesInstances.Remove(this);
	}

	private void Init () {
		if (!_collider) {
			_collider = GetComponent<SphereCollider>();
			_mesh = GetComponentInChildren<MeshFilter>();
			UpdateSize();
		}
	}

	private void UpdateSize() {
		float scale = _sizeFactor;
		transform.localScale = new Vector3(scale, scale, scale);
		_collider.transform.localScale = new Vector3(scale, scale, scale);
	}

	#if UNITY_EDITOR
	private void OnDrawGizmosSelected () {
		Handles.color = Color.white;
		Handles.DrawWireDisc(transform.position, Vector3.forward, Radius);
		Handles.DrawWireDisc(transform.position, Vector3.forward, RadiusGravity);
	}
	#endif
}
