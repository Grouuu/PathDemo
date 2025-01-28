using System.Collections.Generic;
using UnityEngine;

/**
 * Require:
 * . Grid
 */
public class ObstacleController : MonoBehaviour {

	public static ObstacleController Instance { get; private set; }

	[SerializeField] ObstacleBody _bodyPrefab;
	[SerializeField] Rigidbody _playerBody;
	[SerializeField] [Range(2, 10)] int _renderChunkRange = 2;
	[SerializeField] [Range(3, 10)] int _chunkSize = 5;
	[SerializeField] float _minDistance = 5;
	[SerializeField] float _spawnRadius = 10;
	[SerializeField] bool _ignoreCollision = false;
	[SerializeField] bool _debug = false;

	private Sampler _sampler;

	public ObstacleBody[] GetObstacles() {
		return Component.FindObjectsOfType<ObstacleBody>(false);
	}

	public void UpdateObstacleField() {

		List<GridPoint> points = _sampler.GetNewPoints(_playerBody.position); // TODO avoid playerBody reference (allows any position)

		foreach (GridPoint point in points) {
			if (point.isRender) {
				ObstacleBody body = Instantiate(_bodyPrefab, point.position, Quaternion.identity, transform);
				body.SetSizefactor(point.sizeFactor);
				point.body = body;
				point.OnDestroy += DestroyObstacle;
			}
		}
	}

	public bool IsCrashPosition(Vector3 position) {

		if (_ignoreCollision) {
			return false;
		}

		ObstacleBody[] obstacles = GetObstacles();

		if (obstacles == null || obstacles.Length == 0) {
			return false;
		}

		foreach (ObstacleBody body in obstacles) {
			float sqrDistance = (body.transform.position - position).sqrMagnitude;

			if (sqrDistance <= body.Radius * body.Radius) {
				return true;
			}
		}

		return false;
	}

	private void Awake() {
		Instance = this;
	}

	private void Start () {
		Init();
	}

	private void Update () {
		UpdateObstacleField();

		if (_debug) {
			_sampler.UpdateDebug();
		}
	}

	private void Init() {
		GridOptions options = new GridOptions(_renderChunkRange, _chunkSize, _minDistance, _spawnRadius);
		Grid grid = new Grid(options);
		_sampler = new Sampler(grid, options, GetNoiseAt);
	}

	private float GetNoiseAt (Vector2 position) {
		float noiseScale = 1f;
		return Mathf.PerlinNoise(position.x * noiseScale, position.y * noiseScale) * 2 + 0.5f; // [.5, 2.5]
	}

	private void DestroyObstacle (GridPoint point, ObstacleBody body) {
		point.OnDestroy -= DestroyObstacle;

		if (body != null) {
			Destroy(body.gameObject);
		}
	}

}
