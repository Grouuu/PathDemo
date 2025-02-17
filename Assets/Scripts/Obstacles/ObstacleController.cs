using System.Collections.Generic;
using UnityEngine;

public delegate void OnObstaclesUpdate ();
public delegate void OnGridPointDestroy<T> (IGridPoint<T> point, T body);

public interface IGridPoint<T> {
	public OnGridPointDestroy<T> OnDestroy { get; set; }
	public T body { get; set; }
	public ScriptableObject data { get; set; }
	public Vector2 position { get; set; }
	public Vector2Int coords { get; set; }
	public float reservedDistance { get; set; }
	public float sizeFactor { get; set; }
	public void Destroy ();
}

[RequireComponent(typeof(RandomGrid))]
public class ObstacleController : MonoBehaviour {

	public static event OnObstaclesUpdate OnObstaclesUpdate;
	public static List<ObstacleBody> ObstaclesInstances = new List<ObstacleBody>();

	[SerializeField] private Transform _target;
	[SerializeField] private Transform _parent;
	[SerializeField] private bool _ignoreCollision = false;

	private RandomGrid _grid;

	public void UpdateObstacleField() {

		if (_grid.UpdatePoints(_target.position, out List<RandomGridPoint> spawnPoints)) {

			foreach (RandomGridPoint point in spawnPoints) {

				ObstacleData data = point.data as ObstacleData;

				if (!PoolManager.Instance.HasId(data.pool.id)) {
					PoolManager.Instance.AddPool(data.pool);
				}

				ObstacleBody body = PoolManager.Instance.GetInstance<ObstacleBody>(data.pool.id);

				body.transform.parent = _parent;
				body.transform.position = point.position;
				body.transform.rotation = Quaternion.identity;
				body.SetSizeFactor(point.sizeFactor);

				point.body = body;
				point.OnDestroy += DestroyObstacle;
			}

			OnObstaclesUpdate();
		}
	}

	public bool IsCrashPosition(Vector3 position) {

		if (_ignoreCollision) {
			return false;
		}

		if (ObstaclesInstances == null || ObstaclesInstances.Count == 0) {
			return false;
		}

		foreach (ObstacleBody body in ObstaclesInstances) {
			float sqrDistance = (body.transform.position - position).sqrMagnitude;

			if (sqrDistance <= body.Radius * body.Radius) {
				return true;
			}
		}

		return false;
	}

	private void Start () {
		_grid = GetComponent<RandomGrid>();
	}

	private void Update () {
		UpdateObstacleField();
	}

	private void DestroyObstacle (IGridPoint<ObstacleBody> point, ObstacleBody body) {
		point.OnDestroy -= DestroyObstacle;

		if (body != null) {
			PoolManager.Instance.FreeInstance((point.data as ObstacleData).pool.id, body.gameObject);
		}
	}

}
