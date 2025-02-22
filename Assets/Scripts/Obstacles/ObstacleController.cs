using System.Collections.Generic;
using UnityEngine;

public delegate void OnObstaclesUpdate ();

/*
 * Dependencies:
 * . ObstacleBody
 * . RandomGrid
 * . RandomGridPoint
 * . PoolManager
 */
[RequireComponent(typeof(RandomGrid))]
public class ObstacleController : MonoBehaviour
{
	public static event OnObstaclesUpdate OnObstaclesUpdate;
	public static List<ObstacleBody> ObstaclesInstances = new List<ObstacleBody>();

	[SerializeField] private Transform _target;
	[SerializeField] private Transform _parent;
	[SerializeField] private bool _ignoreCollision = false;

	private RandomGrid _grid;

	public void UpdateObstacleField()
	{
		if (_grid.UpdatePoints(_target.position, out List<RandomGridPoint> spawnPoints))
		{
			foreach (RandomGridPoint point in spawnPoints)
			{
				PoolData pool = point.data.pool;

				if (!PoolManager.Instance.HasId(pool.id))
				{
					PoolManager.Instance.AddPool(pool);
				}

				ObstacleBody body = PoolManager.Instance.GetInstance<ObstacleBody>(pool.id);

				body.transform.parent = _parent;
				body.transform.position = point.position;
				body.transform.rotation = Quaternion.identity;
				body.SetSizeFactor(point.sizeFactor);

				point.body = body;
				point.OnDestroy += DestroyObstacle;
			}

			OnObstaclesUpdate?.Invoke();
		}
	}

	public void SetDifficulty (float difficulty)
	{
		_grid.sampler.SetDifficulty(difficulty);
	}

	public bool IsCrashPosition(Vector3 position)
	{
		if (_ignoreCollision)
		{
			return false;
		}

		if (ObstaclesInstances == null || ObstaclesInstances.Count == 0)
		{
			return false;
		}

		foreach (ObstacleBody body in ObstaclesInstances)
		{
			float sqrDistance = (body.transform.position - position).sqrMagnitude;

			if (sqrDistance <= body.Radius * body.Radius)
			{
				return true;
			}
		}

		return false;
	}

	private void Start ()
	{
		_grid = GetComponent<RandomGrid>();
	}

	private void Update ()
	{
		UpdateObstacleField();
	}

	private void DestroyObstacle (RandomGridPoint point)
	{
		point.OnDestroy -= DestroyObstacle;

		if (point.body != null)
		{
			PoolManager.Instance.FreeInstance(point.data.pool.id, point.body.gameObject);
		}
	}

}
