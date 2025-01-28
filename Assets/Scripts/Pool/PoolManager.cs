using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager: MonoBehaviour {

	public static PoolManager Instance;
	public static bool IsInitialized { get; private set; } = false;

	[SerializeField] private List<PoolData> _pools;

	private Dictionary<PoolId, Queue<GameObject>> _poolInstances;

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

	public GameObject GetInstance (PoolId id) {

		if (!HasId(id)) {
			throw new Exception($"No ${id} pool id found");
		}

		Queue<GameObject> queue = _poolInstances[id];

		if (queue.Count > 0) {
			return _poolInstances[id].Dequeue();
		}

		Debug.LogWarning($"No more ${id} instances available");
		return CreateInstance(GetPoolData(id));
	}

	public void FreeInstance (PoolId id, GameObject instance) {

		if (!HasId(id)) {
			throw new Exception($"No ${id} pool id found");
		}

		if (instance != null) {
			instance.SetActive(false);
			_poolInstances[id].Enqueue(instance);
		}
	}

	private void Awake () {
		Instance = this;
	}

	private void Start () {
		_poolInstances = new Dictionary<PoolId, Queue<GameObject>>();

		foreach (PoolData data in _pools) {
			AddPool(data);
		}

		IsInitialized = true;
	}

	private GameObject CreateInstance (PoolData data) {
		GameObject instance = Instantiate(data.prefab);
		instance.SetActive(false);
		return instance;
	}

	private bool HasId (PoolId id) {
		return _pools.Exists((PoolData data) => data.id == id);
	}

	private PoolData GetPoolData (PoolId id) {
		return _pools.Find((PoolData data) => data.id == id);
	}
}
