using System;
using UnityEngine;

[Serializable]
public class PoolData
{
	public string id;
	public GameObject prefab;
	public int poolSize;

	public PoolData (string id, GameObject prefab, int poolSize)
	{
		this.id = id;
		this.prefab = prefab;
		this.poolSize = poolSize;
	}

}