using System;
using UnityEngine;

public delegate void OnUpdatePath (
	Vector3 startPosition,
	Vector3 velocity,
	Vector3 forward,
	Func<Vector3, Vector3> getGravity,
	Func<Vector3, Vector3> getVelocity
);
public delegate void OnUpdatePlayerPosition (Vector3 position);
public delegate void OnUpdatePlayerVelocity (Vector3 velocity);
public delegate void OnUpdatePlayerThrust (float thrustForce);

/*
 * Dependencies:
 * . GravityController
 * . PlayerBody
 */
public class PlayerMovement : MonoBehaviour
{
	public static event OnUpdatePath OnUpdatePath;
	public static event OnUpdatePlayerPosition OnUpdatePlayerPosition;
	public static event OnUpdatePlayerVelocity OnUpdatePlayerVelocity;
	public static event OnUpdatePlayerThrust OnUpdatePlayerThrust;

	public float thrustForce => _thrustForce / _thrustPower; // [-1, 1]
	public Vector3 velocity => _velocity;

	[SerializeField] private PlayerBody _playerBody;
	[SerializeField] private GravityController _gravity;
	[SerializeField] private float _speedMax = 10f;
	[SerializeField] private float _thrustPower = 5f;
	[SerializeField] private float _rotatePower = 100f;
	[SerializeField] private Vector3 _velocity = Vector3.zero;
	[SerializeField] private bool _isDebug = false;

	private Vector3 _velocityDirection => _velocity == Vector3.zero ? _playerBody.forward : _velocity.normalized;

	private float _thrustForce = 0;
	private float _rotateForce = 0;
	private bool _isCrashed = false;

	public void SetIsCrashed (bool isCrashed)
	{
		_isCrashed = isCrashed;
	}

	private void Update ()
	{
		_thrustForce = Input.GetAxisRaw("Vertical") * _thrustPower;
		_rotateForce = -Input.GetAxisRaw("Horizontal") * _rotatePower;

		UpdatePath();
	}

	private void FixedUpdate ()
	{
		if (_isCrashed) {
			return;
		}

		float deltaTime = Time.fixedDeltaTime;

		ApplyRotationForce(deltaTime);
		ApplyAccelerationForce(deltaTime);
		ApplyGravityForce(deltaTime);
		ClampVelocity();
		ApplyTranslation(deltaTime);

		OnUpdatePlayerPosition?.Invoke(_playerBody.position);
		OnUpdatePlayerVelocity?.Invoke(_velocity);
		OnUpdatePlayerThrust?.Invoke(thrustForce);

		if (_isDebug) {
			Debug.DrawLine(_playerBody.position, _playerBody.position + _playerBody.forward * 10, Color.blue); // forward
			Debug.DrawLine(_playerBody.position, _playerBody.position + _velocityDirection * _velocity.magnitude * 20, Color.green); // velocity
		}
	}

	private void ApplyRotationForce (float deltaTime)
	{
		Quaternion rotation = Quaternion.AngleAxis(_rotateForce * deltaTime, _playerBody.up);
		_playerBody.Rotate(rotation);

		if (_velocity != Vector3.zero) {
			_velocity = rotation * _velocity;
		}
	}

	private void ApplyAccelerationForce (float deltaTime)
	{
		Vector3 acceleration = _playerBody.forward * _thrustForce * deltaTime;
		_velocity += acceleration;
	}

	private void ApplyGravityForce (float deltaTime)
	{
		Vector3 gravityForce = GetGravity(_playerBody.position) * deltaTime;
		_velocity += gravityForce;
	}

	private void ApplyTranslation (float deltaTime)
	{
		Vector3 translation = _velocity * deltaTime;
		_playerBody.Translate(translation);
	}

	private void UpdatePath ()
	{
		OnUpdatePath(
			_playerBody.position,
			_velocity,
			_playerBody.forward,
			(Vector3 position) => GetGravity(position),
			(Vector3 velocity) => GetClampedVelocity(velocity)
		);
	}

	private void ClampVelocity ()
	{
		_velocity = GetClampedVelocity(_velocity);
	}

	private Vector3 GetClampedVelocity(Vector3 velocity)
	{
		return Vector3.ClampMagnitude(velocity, _speedMax);
	}

	private Vector3 GetGravity(Vector3 position)
	{
		return _gravity.GetGravityByPosition(position);
	}

}
