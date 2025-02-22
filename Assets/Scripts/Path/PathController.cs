using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Dependencies:
 * . ObstacleBody
 */
public class PathController : MonoBehaviour
{
	[SerializeField] private LineRenderer _lineRenderer;
	[SerializeField] private int _length = 10;
	[SerializeField] private int _steps = 100;
	[SerializeField] private float _width = 0.2f;
	[SerializeField] private bool _ignorePath = false;

	private float _minLength = 0.01f; // prevent safe break trigger

	// TODO to many points with low velocity (around 1000 points)
	// => create a bezize line
	// => check every distanced step the new line point
	// !> care the line is not wiggling

	public void UpdatePath(
		Vector3 position,
		Vector3 velocity,
		Vector3 forward,
		Func<Vector3, Vector3> getGravity,
		Func<Vector3, Vector3> getTranslation
	)
	{
		if (_ignorePath)
		{
			return;
		}

		float deltaTime = 0.02f;
		float stepLength = (float) _length / _steps;
		List<Vector3> points = new List<Vector3>() { position };

		float currentLength = 0;
		bool isEnd = false;
		int safeBreakCount = 0;

		while (currentLength < _length)
		{
			Vector3 gravityVelocity = getGravity(position) * deltaTime;

			velocity += gravityVelocity;

			Vector3 translation = getTranslation(velocity) * deltaTime;
			float lengthAdded = translation.magnitude;

			if (lengthAdded < _minLength)
			{
				translation = forward * _minLength;
				lengthAdded = _minLength;
			}

			currentLength += lengthAdded;

			// end of the path
			if (currentLength > _length)
			{
				isEnd = true;
				lengthAdded = currentLength - _length;
				translation = translation.normalized * lengthAdded;
				currentLength = _length;
			}

			if (Physics.Raycast(position, translation.normalized, out RaycastHit hit, translation.magnitude) && hit.collider.GetComponent<ObstacleBody>())
			{
				// crash point
				points.Add(hit.point);
				break;
			}

			position += translation;

			float threshold = points.Count * stepLength;

			if (currentLength > threshold || isEnd)
			{
				points.Add(position);
			}

			safeBreakCount++;
			if (safeBreakCount > 1000)
			{
				Debug.Log("Infinite path loop suspected");
				break;
			}
		}

		float currentProgress = currentLength / _length;

		Gradient gradient = _lineRenderer.colorGradient;
		GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
		alphaKeys[^1].alpha = 1 - currentProgress;
		gradient.alphaKeys = alphaKeys;

		_lineRenderer.startWidth = _width;
		_lineRenderer.endWidth = _width - (_width * currentProgress);
		_lineRenderer.colorGradient = gradient;
		_lineRenderer.positionCount = points.Count;
		_lineRenderer.SetPositions(points.ToArray());
	}

	private void OnEnable ()
	{
		PlayerMovement.OnUpdatePath += UpdatePath;
	}

	private void OnDisable ()
	{
		PlayerMovement.OnUpdatePath -= UpdatePath;
	}

}
