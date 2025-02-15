using UnityEngine;

[CreateAssetMenu(menuName = "Data/Obstacle Data", fileName = "ObstacleData")]
public class ObstacleData : ScriptableObject {

	public ObstacleId id;
	public Color32 mapColor;
	public PoolData pool;
	public float radius = 0.5f;
	public float radiusGravity = 20f;
	public float mass = 5f;

}
