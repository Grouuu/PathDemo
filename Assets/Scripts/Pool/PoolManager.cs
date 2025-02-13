using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager: MonoBehaviour {

	public static PoolManager Instance;

	[SerializeField] private Transform _parent;

	private List<PoolData> _pools = new List<PoolData>();
	private Dictionary<PoolId, Queue<GameObject>> _poolInstances = new Dictionary<PoolId, Queue<GameObject>>();

	public void AddPool (PoolData data) {

		Queue<GameObject> instances = new Queue<GameObject>();

		for (int i = 0; i < data.poolSize; i++) {
			GameObject body = CreateInstance(data);
			instances.Enqueue(body);
		}

		if (!HasId(data.id)) {
			_pools.Add(data);
		}

		_poolInstances.Add(data.id, instances);
	}

	public T GetInstance<T> (PoolId id) {

		if (!HasId(id)) {
			throw new Exception($"No ${id} pool id found");
		}

		Queue<GameObject> queue = _poolInstances[id];
		GameObject instance;

		if (queue.Count > 0) {
			instance = _poolInstances[id].Dequeue();
		} else {
			Debug.LogWarning($"No more ${id} instances available");
			instance = CreateInstance(GetPoolData(id));
		}

		instance.SetActive(true);

		return instance.GetComponent<T>();
	}

	public void FreeInstance (PoolId id, GameObject instance) {

		if (!HasId(id)) {
			throw new Exception($"No ${id} pool id found");
		}

		if (instance != null) {
			instance.transform.parent = _parent;
			instance.SetActive(false);
			_poolInstances[id].Enqueue(instance);
		}
	}

	public bool HasId (PoolId id) {
		return _pools.Exists((PoolData data) => data.id == id);
	}

	private void Awake () {
		Instance = this;
	}

	private GameObject CreateInstance (PoolData data) {
		GameObject instance = Instantiate(data.prefab);
		instance.transform.parent = _parent;
		instance.SetActive(false);
		return instance;
	}

	private PoolData GetPoolData (PoolId id) {
		return _pools.Find((PoolData data) => data.id == id);
	}
}
