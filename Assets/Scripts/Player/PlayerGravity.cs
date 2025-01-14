using UnityEngine;

public class PlayerGravity : MonoBehaviour {

	[SerializeField]
	private float gravityRotatePower = 0.1f;
	[SerializeField]
	private bool _ignoreGravity = false;
	[SerializeField]
	private bool _debug = false;

	public PlayerBody PlayerBody { get; set; }
	public PlayerMovements PlayerMovement { get; set; }
	public Vector3 GravityVelocity { get; private set; } = Vector3.zero;

	private void FixedUpdate() {
		float deltaTime = Time.fixedDeltaTime;

		if (!_ignoreGravity && !PlayerBody.isCrashed()) {
			GravityVelocity = GravitySingleton.Instance.GetGravityByPosition(PlayerBody.Position) * deltaTime;
			PlayerBody.Translate(GravityVelocity);
		}

		//Quaternion rotationDifference = Quaternion.FromToRotation(PlayerBody.Forward, GravityVelocity);
		//Quaternion rotation = Quaternion.Lerp(PlayerBody.transform.rotation, rotationDifference, 0.1f);
		Quaternion rotation = Quaternion.LookRotation(GravityVelocity);
		//PlayerBody.Rotate(rotation);
		PlayerMovement.RotateVelocity(rotation);

		if (_debug) {
			Debug.DrawLine(PlayerBody.Position, PlayerBody.Position + GravitySingleton.Instance.GetGravityByPosition(PlayerBody.Position).normalized * 50, Color.yellow); // gravity
		}
	}
}
