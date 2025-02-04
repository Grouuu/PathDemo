using UnityEngine;

/**
 * Require:
 * . ObstacleController
 */
public class GravityController : MonoBehaviour {

	public static GravityController Instance { get; private set; }

	[SerializeField] private float _gravityPower = 0.5f;
	[SerializeField] private bool _ignoreGravity = false;

	private ObstacleBody[] _bodies;

	public Vector3 GetGravityByPosition(Vector3 position, float magnitudeMax = float.MaxValue) {

		if (_ignoreGravity) {
			return Vector3.zero;
		}

		if (_bodies == null || _bodies.Length == 0) {
			return Vector3.zero;
		}

		Vector3 gravity = Vector3.zero;

		foreach (ObstacleBody body in _bodies) {
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

	private static Vector3 GetNewtonGravityByPosition (Vector3 movingPosition, Vector3 staticPosition, float staticMass) {
		// Newton's law F = G * mM / r² => a = G * M / r²
		// dot(offset, offset) = sqrMagnitude of offset, |offset|²
		float G = 667.4f;
		Vector3 offset = movingPosition - staticPosition;
		Vector3 direction = offset.normalized;
		float sqrDistance = Vector3.Dot(offset, offset);
		float acceleration = G * staticMass / sqrDistance;
		return direction * acceleration;
	}

	private void Awake() {
		Instance = this;
		ObstacleController.OnObstaclesUpdate += UpdateObstacles;
	}

	private void OnDestroy () {
		ObstacleController.OnObstaclesUpdate -= UpdateObstacles;
	}

	private void UpdateObstacles () {
		_bodies = ObstacleController.Instance.GetObstacles();
	}
}
