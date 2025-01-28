using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Require:
 * . ObstacleBody
 */
[RequireComponent(typeof(LineRenderer))]
public class PathController : MonoBehaviour {

	[SerializeField] private int _length = 10;
	[SerializeField] private int _steps = 100;
	[SerializeField] private bool _ignorePath = false;

	public static PathController Instance { get; private set; }

	private LineRenderer _lineRenderer;
	private float _minLength = 0.1f; // prevent safe break trigger

	public void UpdatePath(
		Vector3 position,
		Vector3 velocity,
		Vector3 forward,
		Func<Vector3, Vector3> getGravity,
		Func<Vector3, Vector3> getTranslation
	) {
		if (_ignorePath) {
			return;
		}

		float deltaTime = 0.02f;
		float stepLength = (float) _length / _steps;
		List<Vector3> points = new List<Vector3>() { position };

		float currentLength = 0f;
		bool isEnd = false;
		int safeBreakCount = 0;

		while (currentLength < _length) {

			Vector3 gravityVelocity = getGravity(position) * deltaTime;
			velocity += gravityVelocity;
			Vector3 translation = getTranslation(velocity) * deltaTime;

			if (translation.magnitude == 0) {
				translation = forward.normalized * _minLength;
			}

			float lengthAdded = translation.magnitude;
			currentLength += lengthAdded;

			// end of the path
			if (currentLength > _length) {
				isEnd = true;
				lengthAdded = currentLength - _length;
				translation = translation.normalized * lengthAdded;
				currentLength = _length;
			}

			RaycastHit hit;

			if (Physics.Raycast(position, translation.normalized, out hit, translation.magnitude) && hit.collider.GetComponent<ObstacleBody>()) {
				// crash
				points.Add(hit.point);
				break;
			}

			position += translation;

			float threshold = points.Count * stepLength;

			if (currentLength > threshold || isEnd) {
				points.Add(position);
			}

			safeBreakCount++;
			if (safeBreakCount > 10000) {
				Debug.Log("Infinite path loop suspected");
				break;
			}
		}

		_lineRenderer.positionCount = points.Count;
		_lineRenderer.SetPositions(points.ToArray());
	}

	private void Awake() {
		Instance = this;
		_lineRenderer = GetComponent<LineRenderer>();
	}

}
