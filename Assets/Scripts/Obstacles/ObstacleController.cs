using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour {

	public static ObstacleController Instance { get; private set; }

	[SerializeField] ObstacleBody _bodyPrefab;
	[SerializeField] Rigidbody _playerBody;
	[SerializeField] int _renderChunkRange = 2;
	[SerializeField] int _chunkSize = 5;
	[SerializeField] float _minDistance = 5;
	[SerializeField] float _maxDistance = 10;
	[SerializeField] bool _ignoreCollision = false;

	private Sampler _sampler;

	public ObstacleBody[] GetObstacles() {
		return Component.FindObjectsOfType<ObstacleBody>(false);
	}

	public void UpdateObstacleField() {

		List<GridPoint> points = _sampler.GetNewPoints(_playerBody.position);

		foreach (GridPoint point in points) {
			if (point.isRender) {
				ObstacleBody body = Instantiate(_bodyPrefab, point.position, Quaternion.identity, transform);
				body.SetSizefactor(point.sizeFactor);
				point.body = body;

				// DEBUG
				body.point = point;

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
		Init();
	}

	private void Start () {
		UpdateObstacleField();
	}

	private void Update () {
		UpdateObstacleField();
		_sampler.UpdateDebug();
	}

	private void Init() {
		GridOptions options = new GridOptions(_renderChunkRange, _chunkSize, _minDistance, _maxDistance);
		Grid grid = new Grid(options);
		_sampler = new Sampler(grid, options);
	}

	private void DestroyObstacle (GridPoint point, ObstacleBody body) {
		point.OnDestroy -= DestroyObstacle;

		if (body != null) {
			Destroy(body.gameObject);
		}
	}

}
