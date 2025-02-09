using System.Collections.Generic;
using UnityEngine;

public delegate void OnObstaclesUpdate ();

[RequireComponent(typeof(RandomGrid))]
public class ObstacleController : MonoBehaviour {

	public static ObstacleController Instance { get; private set; }
	public static event OnObstaclesUpdate OnObstaclesUpdate;

	[SerializeField] ObstacleBody _bodyPrefab;
	[SerializeField] Transform _target;
	[SerializeField] Transform _parent;
	[SerializeField] bool _ignoreCollision = false;

	private RandomGrid _grid;
	private BiomeGrid _gridBiome;

	public ObstacleBody[] GetObstacles() {
		return FindObjectsByType<ObstacleBody>(FindObjectsSortMode.None);
	}

	public void UpdateObstacleField() {


		List<GridPoint> spawnPoints = _gridBiome.UpdatePoints(_target.position);

		foreach (GridPoint point in spawnPoints) {
			ObstacleBody body = PoolManager.Instance.GetInstance<ObstacleBody>(PoolId.Obstacle);
			body.transform.parent = _parent;
			body.transform.position = point.position;
			body.transform.rotation = Quaternion.identity;
			body.SetSizefactor(point.sizeFactor);

			point.body = body;
			point.OnDestroy += DestroyObstacle;
		}

		//_grid.SetCenterPosition(_target.position);

		//List<GridPoint> points = _grid.GetSpawnPoints();

		//foreach (GridPoint point in points) {
		//	if (point.isRender) {

		//		ObstacleBody body = PoolManager.Instance.GetInstance<ObstacleBody>(PoolId.Obstacle);
		//		body.transform.parent = transform;
		//		body.transform.position = point.position;
		//		body.transform.rotation = Quaternion.identity;
		//		body.SetSizefactor(point.sizeFactor);

		//		point.body = body;
		//		point.OnDestroy += DestroyObstacle;
		//	}
		//}

		if (spawnPoints.Count != 0) {
			OnObstaclesUpdate();
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
		_grid = GetComponent<RandomGrid>();
		_gridBiome = GetComponent<BiomeGrid>();
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
