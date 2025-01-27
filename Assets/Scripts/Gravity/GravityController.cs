using UnityEngine;

public class GravityController : MonoBehaviour {

	public static GravityController Instance { get; private set; }

	[SerializeField] private float _gravityPower = 0.5f;
	[SerializeField] private bool _ignoreGravity = false;

	public Vector3 GetGravityByPosition(Vector3 position, float magnitudeMax = float.MaxValue) {

		if (_ignoreGravity) {
			return Vector3.zero;
		}

		ObstacleBody[] bodies = ObstacleController.Instance.GetObstacles();

		if (bodies == null || bodies.Length == 0) {
			return Vector3.zero;
		}

		Vector3 gravity = Vector3.zero;

		foreach (ObstacleBody body in bodies) {
			Vector3 direction = body.transform.position - position;
			float distance = direction.magnitude;
			bool inRange = distance <= body.RadiusGravity;

			if (inRange) {
				float force = GetGravityForceByDistance(distance, body);
				gravity += direction.normalized * force;
			}
		}

		gravity = Vector3.ClampMagnitude(gravity, magnitudeMax);

		return gravity;
	}

	private static float GetGravityForceByDistance (float distance, ObstacleBody body) {
		float distanceToSurface = distance - body.Radius;
		float distanceToSurfaceNormalized = distanceToSurface / (body.RadiusGravity - body.Radius);
		float easeCoeff = 1 - Mathf.Pow(distanceToSurfaceNormalized, 3); // f(x) = x³
		return easeCoeff * body.Mass * (Instance?._gravityPower ?? 0.5f);
	}

	private void Awake() {
		Instance = this;
	}
}
