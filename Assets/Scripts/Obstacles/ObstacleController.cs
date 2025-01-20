using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour {

	public static ObstacleController Instance { get; private set; }

	[SerializeField] ObstacleBody _bodyPrefab;

	public ObstacleBody[] GetObstacles() {
		return Component.FindObjectsOfType<ObstacleBody>(false);
	}

	public void GenerateObstacleField() {

		float offsetX = 5;
		float minDistance = 5;
		float areaWidth = 100;
		float areaHeight = 50;

		List<Vector2> points = Sampler.GeneratePoints(minDistance, new Vector2(areaWidth, areaHeight));
		Vector2 offset = new Vector2(offsetX, -areaHeight / 2);

		foreach (Vector3 position in points) {
			ObstacleBody body = Instantiate(_bodyPrefab, new Vector3(position.x + offset.x, position.y + offset.y, 0), Quaternion.identity, transform);
		}
	}

	public bool IsCrashPosition(Vector3 position) {
		ObstacleBody[] obstacles = GetObstacles();

		if (obstacles == null || obstacles.Length == 0) {
			return false;
		}

		foreach (ObstacleBody body in obstacles) {
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

	private void Start () {
		GenerateObstacleField();
	}

}
