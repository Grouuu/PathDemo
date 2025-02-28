using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager: MonoBehaviour
{
	public static PoolManager Instance;

	[SerializeField] private Transform _parent;

	private List<PoolData> _pools = new List<PoolData> ();
	private Dictionary<string, Queue<GameObject>> _poolInstances = new Dictionary<string, Queue<GameObject>>();

	public void AddPool (PoolData data)
	{
		Queue<GameObject> instances = new Queue<GameObject>();

		for (int i = 0; i < data.poolSize; i++)
		{
			GameObject body = CreateInstance(data);
			instances.Enqueue(body);
		}

		if (!HasId(data.id))
		{
			_pools.Add(data);
		}

		_poolInstances.Add(data.id, instances);
	}

	public T GetInstance<T> (string id)
	{
		if (!HasId(id)) {
			throw new Exception($"No ${id} pool id found");
		}

		Queue<GameObject> queue = _poolInstances[id];
		GameObject instance;

		if (queue.Count > 0)
		{
			instance = _poolInstances[id].Dequeue();
		}
		else
		{
			instance = CreateInstance(GetPoolData(id));
		}

		instance.SetActive(true);

		return instance.GetComponent<T>();
	}

	public void FreeInstance (string id, GameObject instance)
	{
		if (!HasId(id))
		{
			Debug.LogError($"No ${id} pool id found");
			Destroy(instance.gameObject);
			return;
		}

		if (instance != null)
		{
			instance.transform.parent = _parent;
			instance.SetActive(false);
			_poolInstances[id].Enqueue(instance);
		}
	}

	public bool HasId (string id)
	{
		return _pools.Exists((PoolData data) => data.id == id);
	}

	private void Awake ()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private GameObject CreateInstance (PoolData data)
	{
		GameObject instance = Instantiate(data.prefab);
		instance.transform.parent = _parent;
		instance.SetActive(false);
		return instance;
	}

	private PoolData GetPoolData (string id)
	{
		return _pools.Find((PoolData data) => data.id == id);
	}

}
