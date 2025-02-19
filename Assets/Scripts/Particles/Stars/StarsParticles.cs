using System.Collections.Generic;
using UnityEngine;

public class StarsParticles : MonoBehaviour {

	[SerializeField] private ParticleSystem _particleSystem;
	[SerializeField] private PoolData _poolData;
	[SerializeField] Transform _target;
	[SerializeField] private Transform _parent;
	[SerializeField] private Color _color = Color.white;
	[SerializeField] private int _density = 500;
	[SerializeField] private float _minStarSize = 0.05f;
	[SerializeField] private float _maxStarSize = 0.15f;
	[SerializeField] private float _timeBetweeenUpdate = 1;
	[SerializeField] private bool _isDebug = false;

	private readonly string _poolId = "Stars";

	private float _timeBeforeUpdate;
	private Vector2 _sceneSize;
	private Dictionary<string, ParticleSystem> _boxes = new Dictionary<string, ParticleSystem>();
	private Vector2Int _currentCoords;
	private bool _firstCall = true;

	private void Update () {

		if (Time.time > _timeBeforeUpdate) {

			_timeBeforeUpdate = Time.time + _timeBetweeenUpdate;
			_sceneSize = Utils.GetSceneSize();
			Vector2Int targetCoords = GetCoordsByPosition(_target.position);

			if (!_firstCall && targetCoords == _currentCoords) {
				return;
			}

			_currentCoords = targetCoords;

			CleanBoxes(targetCoords);

			AddBox(targetCoords + Vector2Int.left + Vector2Int.up);
			AddBox(targetCoords + Vector2Int.left);
			AddBox(targetCoords + Vector2Int.left + Vector2Int.down);

			AddBox(targetCoords + Vector2Int.up);
			AddBox(targetCoords);
			AddBox(targetCoords + Vector2Int.down);

			AddBox(targetCoords + Vector2Int.right + Vector2Int.up);
			AddBox(targetCoords + Vector2Int.right);
			AddBox(targetCoords + Vector2Int.right + Vector2Int.down);

			_firstCall = false;
		}
	}

	public void AddBox (Vector2Int coords) {

		if (GetBoxByID(coords) != null) {
			return;
		}

		Vector2 boxPosition = coords * _sceneSize;
		ParticleSystem box = PoolManager.Instance.GetInstance<ParticleSystem>(_poolId);
		box.transform.parent = _parent;
		box.transform.position = boxPosition;
		box.transform.rotation = Quaternion.identity;

		_boxes.Add(GetBoxId(coords), box);

		if (_isDebug) {
			Debug.DrawCenteredRectangle(boxPosition, _sceneSize, Color.red);
		}

		SetParticles(box);
	}

	public void SetParticles (ParticleSystem box) {

		ParticleSystem.Particle[] points = new ParticleSystem.Particle[_density];

		for (int i = 0; i < _density; i++) {
			float posX = Random.Range(-_sceneSize.x / 2, _sceneSize.x / 2);
			float posY = Random.Range(-_sceneSize.y / 2, _sceneSize.y / 2);

			points[i].position = new Vector2(posX, posY);
			points[i].startSize = Random.Range(_minStarSize, _maxStarSize);
			points[i].startColor = _color;
		}

		box.SetParticles(points, points.Length);
	}

	private void Start () {
		PoolManager.Instance.AddPool(_poolData);
	}

	private void CleanBoxes (Vector2Int targetCoords) {
		List<string> deleteKeys = new List<string>();

		foreach (KeyValuePair<string, ParticleSystem> entry in _boxes) {

			Vector2 boxCoords = GetCoordsByPosition(entry.Value.transform.position);

			if (Mathf.Abs(boxCoords.x - targetCoords.x) > 1 || Mathf.Abs(boxCoords.y - targetCoords.y) > 1) {
				deleteKeys.Add(entry.Key);
			}
		}

		foreach (string key in deleteKeys) {
			PoolManager.Instance.FreeInstance(_poolId, _boxes[key].gameObject);
			_boxes.Remove(key);
		}
	}

	private Vector2Int GetCoordsByPosition (Vector2 position) {
		return new Vector2Int(
			Mathf.RoundToInt(position.x / _sceneSize.x),
			Mathf.RoundToInt(position.y / _sceneSize.y)
		);
	}

	private ParticleSystem GetBoxByID (Vector2Int coords) {

		if (_boxes.TryGetValue(GetBoxId(coords), out ParticleSystem box)) {
			return box;
		}

		return null;
	}

	private string GetBoxId (Vector2Int coords) {
		return coords.x + " " + coords.y;
	}

}
