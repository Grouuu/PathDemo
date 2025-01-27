using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	[SerializeField] private Rigidbody _playerBody;
	[SerializeField] private float _speedMax = 10f;
	[SerializeField] private float _thrustPower = 5f;
	[SerializeField] private float _rotatePower = 100f;
	[SerializeField] private bool _debug = false;

	private Vector3 _playerBodyForward => _playerBody.transform.right;
	private Vector3 _playerBodyUp => _playerBody.transform.forward;
	private Vector3 _playerBodyPosition => _playerBody.position;
	private Vector3 _velocityDirection => _velocity == Vector3.zero ? _playerBodyForward : _velocity.normalized;

	private Vector3 _velocity = Vector3.zero;
	private float _thrustForce = 0;
	private float _rotateForce = 0;

	private void Update () {
		_thrustForce = Input.GetAxisRaw("Vertical") * _thrustPower;
		_rotateForce = -Input.GetAxisRaw("Horizontal") * _rotatePower;

		UpdatePath();
	}

	private void FixedUpdate () {

		if (ObstacleController.Instance.IsCrashPosition(_playerBodyPosition)) {
			// TODO check a flag instead (crash system for path + collider crash)
			// crash
			SceneController.Crashed();
			return;
		}

		float deltaTime = Time.fixedDeltaTime;

		Quaternion rotationForce = Quaternion.AngleAxis(_rotateForce * deltaTime, _playerBodyUp);
		_playerBody.rotation *= rotationForce;
		_velocity = rotationForce * _velocity;

		Vector3 acceleration = _playerBodyForward * _thrustForce * deltaTime;
		_velocity += acceleration;

		Vector3 gravityForce = GetGravity(_playerBodyPosition) * deltaTime;
		_velocity += gravityForce;

		_velocity = GetClampedVelocity(_velocity);

		Vector3 translation = _velocity * deltaTime;
		_playerBody.position += translation;

		if (_velocity != Vector3.zero) {
			// face the velocity
			float deltaAngle = Vector3.SignedAngle(_playerBodyForward, _velocity, _playerBodyUp);
			Quaternion rotationVelocity = Quaternion.AngleAxis(deltaAngle * deltaTime, _playerBodyUp);
			_playerBody.rotation *= rotationVelocity;
		}

		if (_debug) {
			Debug.DrawLine(_playerBodyPosition, _playerBodyPosition + _playerBodyForward * 10, Color.blue); // forward
			Debug.DrawLine(_playerBodyPosition, _playerBodyPosition + _velocityDirection * _velocity.magnitude * 20, Color.green); // velocity
		}
	}

	private void UpdatePath () {
		PathController.Instance.UpdatePath(
			_playerBodyPosition,
			_velocity,
			_playerBodyForward,
			(Vector3 position) => GetGravity(position),
			(Vector3 velocity) => GetClampedVelocity(velocity)
		);
	}

	private Vector3 GetClampedVelocity(Vector3 velocity) {
		return Vector3.ClampMagnitude(velocity, _speedMax);
	}

	private Vector3 GetGravity(Vector3 position) {
		return GravityController.Instance.GetGravityByPosition(position);
	}

}
