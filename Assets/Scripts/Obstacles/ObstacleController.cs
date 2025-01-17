using UnityEngine;

public class ObstacleController : MonoBehaviour {

	public static ObstacleController Instance { get; private set; }

	public Obstacle[] GetObstacles() {
		return Component.FindObjectsOfType<Obstacle>(false);
	}

	public bool IsCrashPosition(Vector3 position) {
		Obstacle[] obstacles = GetObstacles();

		if (obstacles == null || obstacles.Length == 0) {
			return false;
		}

		foreach (Obstacle body in obstacles) {
			float distance = (body.transform.position - position).magnitude;

			if (distance <= body.Radius) {
				return true;
			}
		}

		return false;
	}

	private void Awake() {
		Instance = this;
	}

}
