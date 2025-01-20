using UnityEngine;

[CreateAssetMenu(menuName = "Data/Obstacle Data", fileName = "ObstacleData")]
public class ObstacleData : ScriptableObject {

	public ObstacleBody Prefab;
	public float Radius = 0.5f;
	public float RadiusGravity = 20f;
	public float Mass = 5f;

}
