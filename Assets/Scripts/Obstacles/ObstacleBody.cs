using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ObstacleBody : MonoBehaviour {

	public float Mass => _data.Mass * _sizeFactor;
	public float Radius => _data.Radius * _sizeFactor;
	public float RadiusGravity => _data.RadiusGravity * _sizeFactor;

	[SerializeField] private ObstacleData _data;

	private SphereCollider _collider;
	private MeshFilter _mesh;
	private float _defaultScale = 1;
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

	private void Init () {
		if (!_collider) {
			_collider = GetComponent<SphereCollider>();
			_mesh = GetComponentInChildren<MeshFilter>();
			UpdateDefaultScale();
			UpdateSize();
		}
	}

	private void UpdateDefaultScale () {
		Vector3 radius = _mesh.mesh.bounds.extents;
		_defaultScale = _data.Radius / Mathf.Max(radius.x, radius.y, radius.z);
	}

	private void UpdateSize() {
		float scale = _defaultScale * _sizeFactor;
		transform.localScale = new Vector3(scale, scale, scale);
		_collider.transform.localScale = new Vector3(scale, scale, scale);
	}
}
