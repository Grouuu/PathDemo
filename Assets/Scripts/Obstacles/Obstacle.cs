using UnityEngine;

public class Obstacle : MonoBehaviour {

	[SerializeField] private float _radius = 0.5f;
	[SerializeField] private float _radiusGravity = 50f;
	[SerializeField] private float _mass = 50f;

	public float Mass => _mass;
	public float Radius => _radius;
	public float RadiusGravity => _radiusGravity;

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, _radiusGravity);

		float outsideRadius = _radiusGravity - _radius;
		int stepTotal = 10;
		float stepLength = outsideRadius / stepTotal;

		for (int i = 0; i < stepTotal; i++) {
			float distanceToSurface = _radius + stepLength * i;
			float gravityForce = GravityController.GetGravityForceByDistance(distanceToSurface, this);
			float gravityForceNormalized = Mathf.InverseLerp(0, GravityController.GetGravityMax(this), gravityForce);
			Color color = Color.Lerp(Color.blue, Color.red, gravityForceNormalized);
			Gizmos.color = color;
			Gizmos.DrawWireSphere(transform.position, _radius + distanceToSurface);
		}
	}
}
