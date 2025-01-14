using UnityEngine;

public class PlayerMovements: MonoBehaviour {

	[SerializeField]
	private float _thrustSpeedMax = 0.5f;
	[SerializeField]
	private float _thrustPower = 0.2f;
	[SerializeField]
	private float _rotatePower = 500f;
	[SerializeField]
	private bool _debug = false;

	public PlayerBody PlayerBody { get; set; }
	public Vector3 ThrustVelocity => GetThrustVelocityClamped();

	private Vector3 _thrustVelocity = Vector3.zero;
	private float _thrustForce = 0;
	private float _rotateForce = 0;

	/** intensity between -1 and 1 (-1 is backward full speed, 1 is forward full speed) */
	public void Thrust(float intensity) {
		_thrustForce = intensity * _thrustPower;
	}

	/** intensity between -1 and 1 (-1 is rotate left full speed, 1 is rotate right full speed) */
	public void Rotate(float intensity) {
		_rotateForce = intensity * _rotatePower;
	}

	public void RotateVelocity(Quaternion rotation) {
		// avoid to change the speed
		float magnitude = _thrustVelocity.magnitude;
		_thrustVelocity = rotation * _thrustVelocity;
		_thrustVelocity = _thrustVelocity.normalized * magnitude;
	}

	private void FixedUpdate() {
		float deltaTime = Time.fixedDeltaTime;

		if (!PlayerBody.isCrashed()) {
			UpdateThrustVelocity(deltaTime);
			UpdateBodyRotation(deltaTime);
			UpdateBodyPosition(deltaTime);
		}

		if (_debug) {
			Debug.DrawLine(PlayerBody.Position, PlayerBody.Position + PlayerBody.Forward * 10, Color.blue); // forward
			Debug.DrawLine(PlayerBody.Position, PlayerBody.Position + DirectionVelocity() * _thrustVelocity.magnitude * 20, Color.green); // velocity
		}
	}

	private void UpdateThrustVelocity(float deltaTime) {
		_thrustVelocity += PlayerBody.Forward * _thrustForce * deltaTime;
	}

	private void UpdateBodyRotation(float deltaTime) {
		Quaternion rotation = Quaternion.AngleAxis(_rotateForce * deltaTime, PlayerBody.Up);
		PlayerBody.Rotate(rotation);
		RotateVelocity(rotation);
	}

	private void UpdateBodyPosition(float deltaTime) {
		PlayerBody.Translate(GetThrustVelocityClamped() * deltaTime);
	}

	private Vector3 GetThrustVelocityClamped() {
		return Vector3.ClampMagnitude(_thrustVelocity, _thrustSpeedMax);
	}

	private Vector3 DirectionVelocity() {
		return _thrustVelocity == Vector3.zero ? PlayerBody.Forward : _thrustVelocity.normalized;
	}
}
