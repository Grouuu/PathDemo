using UnityEngine;

/*
 * Dependencies:
 * . ObstacleController
 * . ObstacleBody
 */
public class GravityController : MonoBehaviour
{
	[SerializeField] private float _gravityPower = 0.5f;
	[SerializeField] private bool _ignoreGravity = false;

	private ObstacleBody[] _bodies;

	public Vector3 GetGravityByPosition(Vector3 position, float magnitudeMax = float.MaxValue)
	{
		if (_ignoreGravity)
		{
			return Vector3.zero;
		}

		if (_bodies == null || _bodies.Length == 0)
		{
			return Vector3.zero;
		}

		Vector3 gravity = Vector3.zero;

		foreach (ObstacleBody body in _bodies)
		{
			Vector3 delta = body.transform.position - position;
			float distance = delta.magnitude;
			bool inRange = distance <= body.RadiusGravity;

			if (inRange) {
				float force = GetGravityForceByDistance(distance, body);
				gravity += delta.normalized * force;
			}
		}

		gravity = Vector3.ClampMagnitude(gravity, magnitudeMax);

		return gravity;
	}

	private float GetGravityForceByDistance (float distance, ObstacleBody body)
	{
		float distanceToSurface = distance - body.Radius;
		float distanceToSurfaceNormalized = distanceToSurface / (body.RadiusGravity - body.Radius);
		float easeCoeff = 1 - Mathf.Pow(distanceToSurfaceNormalized, 3); // f(x) = x³
		return easeCoeff * body.Mass * _gravityPower;
	}

	private void OnEnable ()
	{
		ObstacleController.OnObstaclesUpdate += UpdateObstacles;
	}

	private void OnDisable ()
	{
		ObstacleController.OnObstaclesUpdate -= UpdateObstacles;
	}

	private void UpdateObstacles ()
	{
		_bodies = ObstacleController.ObstaclesInstances.ToArray();
	}

}
