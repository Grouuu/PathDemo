using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lineRenderer;
	[SerializeField]
	private PlayerMovements _playerMovement;
	[SerializeField]
	private int _stepTotal = 50;
	[SerializeField]
	private float _stepLength = 0.5f;

	public Vector3 StartPosition = Vector3.zero;
	public Vector3 StartVelocity = Vector3.zero;

	public void UpdatePath(Vector3 basePosition, Vector3 thrustVelocity, float deltaTime) {

		List<Vector3> points = new List<Vector3>();
		points.Add(basePosition);

		Vector3 position = basePosition;

		for (int step = 1; step < _stepTotal; step++) {
			position += thrustVelocity * deltaTime;
			Vector3 gravityVelocity = GravitySingleton.Instance.GetGravityByPosition(position) * deltaTime;

			RaycastHit hit;

			if (Physics.Raycast(position, gravityVelocity.normalized, out hit, gravityVelocity.magnitude) && hit.collider.GetComponent<ObstacleBody>()) {
				points.Add(hit.point);
				break; // break the path
			}

			position += gravityVelocity;
			points.Add(position);
		}

		_lineRenderer.positionCount = points.Count;
		_lineRenderer.SetPositions(points.ToArray());

		//int nbStep = _stepTotal + 1; // +1 for initial position

		//List<Vector3> points = new List<Vector3>();

		//Vector3 position = player.Position;
		//Vector3 velocity = player.Velocity;

		////points.Add(position);

		//for (int step = 1; step < nbStep * multiplierStep; step++) {
		//	Vector3 oldPosition = position;

		//	velocity = player.UpdateVelocity(velocity, position, obstacles, dt);
		//	position += velocity * dt;

		//	Vector3 direction = position - oldPosition;
		//	RaycastHit hit;

		//	// if hit a GravityBody
		//	if (Physics.Raycast(position, direction.normalized, out hit, direction.magnitude)
		//		&& hit.collider.GetComponent<GravityBody>()) {
		//		points.Add(hit.point);
		//		break; // break the path
		//	}

		//	if (step % multiplierStep == 1)
		//		points.Add(position); // add a step each X simulations
		//}

		//line.positionCount = points.Count;
		//line.SetPositions(points.ToArray());
	}

	private void Update() {
		List<Vector3> points = new List<Vector3>();
		Vector3 position = StartPosition;
		Vector3 velocity = StartVelocity;

		points.Add(position);

		for (int step = 1; step < _stepTotal; step++) {

		}
	}

}
