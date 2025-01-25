using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	[SerializeField] private Rigidbody _playerBody;
	[SerializeField] private float _speedMax = 1f;
	[SerializeField] private float _thrustPower = 0.5f;
	[SerializeField] private float _rotatePower = 500f;
	[SerializeField] private bool _isSnap = true;
	[SerializeField] private float _snapPower = 2f;
	[SerializeField] private bool _debug = false;

	private Vector3 _velocity = Vector3.zero;
	private float _thrustForce = 0;
	private float _rotateForce = 0;

	private Vector3 _playerBodyForward => _playerBody.transform.right;
	private Vector3 _playerBodyUp => _playerBody.transform.forward;
	private Vector3 _playerBodyPosition => _playerBody.position;
	private Vector3 _velocityDirection => _velocity == Vector3.zero ? _playerBodyForward : _velocity.normalized;

	private void Update () {
		_thrustForce = Input.GetAxisRaw("Vertical") * _thrustPower;
		_rotateForce = -Input.GetAxisRaw("Horizontal") * _rotatePower;
		UpdatePath();
	}

	private void FixedUpdate () {

		if (ObstacleController.Instance.IsCrashPosition(_playerBodyPosition)) {
			// crash
			return;
		}

		float deltaTime = Time.fixedDeltaTime;

		UpdateThrustVelocity(deltaTime);
		AddGravity(deltaTime);
		UpdateRotation(deltaTime);
		UpdateGravitySnap(deltaTime);
		UpdatePosition(deltaTime);

		if (_debug) {
			Debug.DrawLine(_playerBodyPosition, _playerBodyPosition + _playerBodyForward * 10, Color.blue); // forward
			Debug.DrawLine(_playerBodyPosition, _playerBodyPosition + _velocityDirection * _velocity.magnitude * 20, Color.green); // velocity
		}
	}

	private void UpdateThrustVelocity (float deltaTime) {
		Vector3 acceleration = _playerBodyForward * _thrustForce * deltaTime;
		_velocity += acceleration;
	}

	private void UpdateRotation (float deltaTime) {
		Quaternion rotation = Quaternion.AngleAxis(_rotateForce * deltaTime, _playerBodyUp);
		_playerBody.rotation *= rotation;
	}

	private void UpdateGravitySnap (float deltaTime) {
		Quaternion rotation = GetVelocitySnapRotation(deltaTime);
		_playerBody.rotation *= rotation;
	}

	private void UpdatePosition (float deltaTime) {
		Vector3 translation = GetBodyTranslation(_velocity, deltaTime);
		_playerBody.transform.Translate(translation, Space.World);
	}

	private void UpdatePath () {
		PathController.Instance.UpdatePath(
			_playerBodyPosition,
			_velocity,
			_playerBodyForward,
			(Vector3 position, float deltaTime) => GetGravity(position, deltaTime),
			(Vector3 velocity, float deltaTime) => GetBodyTranslation(velocity, deltaTime)
		);
	}

	private void AddGravity (float deltaTime) {
		Vector3 gravityForce = GetGravity(_playerBodyPosition, deltaTime);
		_velocity += gravityForce;

		if (_debug) {
			Debug.DrawLine(_playerBodyPosition, _playerBodyPosition + gravityForce.normalized * 10, Color.red);
		}
	}

	private Vector3 GetBodyTranslation(Vector3 velocity, float deltaTime) {
		return Vector3.ClampMagnitude(velocity, _speedMax) * deltaTime;
	}

	private Vector3 GetGravity(Vector3 position, float deltaTime) {
		return GravityController.Instance.GetGravityByPosition(position) * deltaTime;
	}

	private Quaternion GetVelocitySnapRotation (float deltaTime) {
		
		if (!_isSnap) {
			return Quaternion.identity;
		}

		Quaternion rotation = Quaternion.identity;

		// force the player body to face the velocity when no input
		// prevent the player to plank and use only thrust to avoid collisions
		if (_thrustForce == 0 && _rotateForce == 0 && _velocity != Vector3.zero) {
			float deltaAngle = Vector3.SignedAngle(_playerBodyForward, _velocityDirection, _playerBodyUp);
			rotation = Quaternion.AngleAxis(deltaAngle * deltaTime, _playerBodyUp);
		}

		return rotation;
	}

}
