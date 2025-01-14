using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBody : MonoBehaviour {

	public Vector3 Position => rigidBody3D.transform.position;
	public Vector3 Forward => rigidBody3D.transform.right; // z => x
	public Vector3 Up => rigidBody3D.transform.forward; // y => z

	private Rigidbody rigidBody3D;

	private void Start() {
		rigidBody3D = GetComponent<Rigidbody>();
		rigidBody3D.constraints =
			RigidbodyConstraints.FreezePositionZ |
			RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY
		;
	}

	public void Rotate(Quaternion value) {
		rigidBody3D.rotation = value * rigidBody3D.rotation;
	}

	public void Translate(Vector3 value) {
		rigidBody3D.position += value;
	}

	public bool isCrashed() {
		return Obstacles.isCrashedPosition(Position);
	}

	private void Update() {
		if (isCrashed()) {
			// TODO
			// . trigger an action
		}
	}
}
