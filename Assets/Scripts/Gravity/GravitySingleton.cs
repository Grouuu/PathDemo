using UnityEngine;

public class GravitySingleton : MonoBehaviour {

	[SerializeField]
	private float _gravityPower = 0.5f;

	public static GravitySingleton Instance { get; private set; }

	public Vector3 GetGravityByPosition(Vector3 position, float magnitudeMax = float.MaxValue) {

		// TODO
		// . the gravity is good at mid distance, but close to a body it increases too much

		ObstacleBody[] bodies = Obstacles.GetObstacles();

		if (bodies == null || bodies.Length == 0) {
			return Vector3.zero;
		}

		Vector3 gravity = Vector3.zero;

		foreach (ObstacleBody body in bodies) {
			Vector3 direction = body.transform.position - position;
			float distance = direction.magnitude;
			bool inRange = distance <= body.radiusGravity;

			if (inRange) {
				// Newton's law : F = G(mM/r²)
				// for simplicity, only the obstacle mass is used, the other is considered as negligeable
				float force = (1 / distance) * body.mass; // f(x) = 1/x * M
				//float force = 667.4f * body.mass / Mathf.Pow(distance, 2);
				gravity += direction.normalized * force;
			}
		}

		gravity = Vector3.ClampMagnitude(gravity, magnitudeMax);

		return gravity * _gravityPower;
	}

	private void Awake() {
		Instance = this;
	}
}
