using System.Collections.Generic;
using UnityEngine;

public delegate void OnObstaclesUpdate ();

[RequireComponent(typeof(ChunkGrid))]
public class ObstacleController : MonoBehaviour {

	public static event OnObstaclesUpdate OnObstaclesUpdate;
	public static List<ObstacleBody> ObstaclesInstances = new List<ObstacleBody>();

	[SerializeField] private Transform _target;
	[SerializeField] private Transform _parent;
	[SerializeField] private bool _ignoreCollision = false;

	private ChunkGrid _gridBiome;

	public void UpdateObstacleField() {

		if (_gridBiome.UpdatePoints(_target.position, out List<ChunkGridPoint> spawnPoints)) {

			foreach (ChunkGridPoint point in spawnPoints) {

				ChunkBody body = Instantiate<ChunkBody>(point.chunkData.prefab);
				body.transform.parent = _parent;
				body.transform.position = point.position;
				body.transform.rotation = Quaternion.identity;

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
		_gridBiome = GetComponent<ChunkGrid>();
	}

	private void Update () {
		UpdateObstacleField();
	}

	private void DestroyObstacle (ChunkGridPoint point, ChunkBody body) {
		point.OnDestroy -= DestroyObstacle;

		if (body != null) {
			Destroy(body.gameObject);
		}
	}

}
