using UnityEngine;

public class Obstacles : MonoBehaviour {

	public static ObstacleBody[] GetObstacles() {
		// ignore inactive bodies
		return Component.FindObjectsOfType<ObstacleBody>(false);
	}

	public static bool isCrashedPosition(Vector3 position) {
		ObstacleBody[] bodies = Obstacles.GetObstacles();

		if (bodies == null || bodies.Length == 0) {
			return false;
		}

		bool isCrashed = false;

		foreach (ObstacleBody body in bodies) {
			float distance = (body.Position() - position).magnitude;

			if (distance <= body.radius) {
				isCrashed = true;
				break;
			}
		}

		return isCrashed;
	}
}
