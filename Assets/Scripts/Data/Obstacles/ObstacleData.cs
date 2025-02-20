using UnityEngine;

/*
 * Dependencies:
 * . ObstacleId
 * . PoolData
 */
[CreateAssetMenu(menuName = "Data/Obstacle Data", fileName = "ObstacleData")]
public class ObstacleData : ScriptableObject
{
	public ObstacleId id;
	public PoolData pool;
	public float radius = 1;
	public float radiusGravity = 10;
	public float mass = 5;

	private void OnValidate ()
	{
		if (pool != null)
		{
			// prevent incorrect or non debug friendly pool id
			pool.id = $"obstacle_{id}";
		}
	}

}
