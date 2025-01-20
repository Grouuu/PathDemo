using UnityEngine;

public class GravityController : MonoBehaviour {

	[SerializeField] private float _gravityPower = 0.5f;
	[SerializeField] private bool _ignoreGravity = false;

	public static GravityController Instance { get; private set; }

	public static float GetGravityForceByDistance(float distance, ObstacleBody body) {
		float distanceToSurface = distance - body.Radius;
		float distanceToSurfaceNormalized = distanceToSurface / (body.RadiusGravity - body.Radius);
		float easeCoeff = 1 - Mathf.Pow(distanceToSurfaceNormalized, 3); // f(x) = x³
		return easeCoeff * body.Mass * (Instance?._gravityPower ?? 0.5f);
		//return (1 / Mathf.Max(distanceToSurface, Mathf.Epsilon)) * body.Mass; // f(x) = 1/x * M
		//return 667.4f * body.mass / Mathf.Pow(distance, 2); // F = G(mM/r²) where m is negligeable
	}

	public static float GetGravityMax(ObstacleBody body) {
		return GetGravityForceByDistance(body.Radius, body);
	}

	public Vector3 GetGravityByPosition(Vector3 position, float magnitudeMax = float.MaxValue) {

		if (_ignoreGravity) {
			return Vector3.zero;
		}

		// TODO
		// . the gravity is good at mid distance, but close to a body it increases too much
		// . make the gravity vector staying in xy plan

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

	private void Awake() {
		Instance = this;
	}
}
