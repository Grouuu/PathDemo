using UnityEngine;

public class PlayerControls : MonoBehaviour {

	public float GetThrustInput() {
		// between -1 and 1
		return Input.GetAxisRaw("Vertical");
	}

	public float GetRotateInput() {
		// between -1 and 1 (inverted)
		return -Input.GetAxisRaw("Horizontal");
	}
}
