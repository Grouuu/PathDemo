using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObstacleGrid))]
public class ObstacleController : MonoBehaviour {

	public static ObstacleController Instance { get; private set; }

	[SerializeField] ObstacleBody _bodyPrefab;
	[SerializeField] Rigidbody _playerBody; // TODO avoid playerBody reference (allows any position)
	[SerializeField] bool _ignoreCollision = false;

	private ObstacleGrid _grid;

	public ObstacleBody[] GetObstacles() {
		return Component.FindObjectsOfType<ObstacleBody>(false);
	}

	public void UpdateObstacleField() {

		_grid.SetCenterPosition(_playerBody.position);

		List<GridPoint> points = _grid.GetSpawnPoints();

		foreach (GridPoint point in points) {
			if (point.isRender) {

				GameObject instance = PoolManager.Instance.GetInstance(PoolId.Obstacle);
				instance.SetActive(true);

				ObstacleBody body = instance.GetComponent<ObstacleBody>();
				body.transform.parent = transform;
				body.transform.position = point.position;
				body.transform.rotation = Quaternion.identity;
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
		_grid = GetComponent<ObstacleGrid>();
	}

	private void Update () {
		UpdateObstacleField();
	}

	private void DestroyObstacle (GridPoint point, ObstacleBody body) {
		point.OnDestroy -= DestroyObstacle;

		if (body != null) {
			PoolManager.Instance.FreeInstance(PoolId.Obstacle, body.gameObject);
		}
	}

}
