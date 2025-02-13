using UnityEngine;

[CreateAssetMenu(menuName = "Data/Obstacle Data", fileName = "ObstacleData")]
public class ObstacleData : ScriptableObject {

	public ObstacleId id;
	public Color32 mapColor;
	public PoolData pool;
	public float Radius = 0.5f;
	public float RadiusGravity = 20f;
	public float Mass = 5f;

}
