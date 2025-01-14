using UnityEngine;

[RequireComponent(typeof(PlayerControls))]
[RequireComponent(typeof(PlayerMovements))]
[RequireComponent(typeof(PlayerGravity))]
[RequireComponent(typeof(PathController))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private PlayerBody _playerBody;
	[SerializeField]
	private PathController _path;

	private PlayerControls _controls;
	private PlayerMovements _movement;
	private PlayerGravity _gravity;

	private void Awake() {
		_controls = GetComponent<PlayerControls>();
		_movement = GetComponent<PlayerMovements>();
		_gravity = GetComponent<PlayerGravity>();

		_movement.PlayerBody = _playerBody;
		_gravity.PlayerBody = _playerBody;
		_gravity.PlayerMovement = _movement;
	}

	private void Update() {
		_movement.Thrust(_controls.GetThrustInput());
		_movement.Rotate(_controls.GetRotateInput());
		_path.UpdatePath(_playerBody.Position, _movement.ThrustVelocity, Time.deltaTime);
	}
}
