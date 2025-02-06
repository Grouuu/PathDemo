using UnityEngine;

public class FirewallBody : MonoBehaviour {

	private void OnTriggerEnter (Collider other) {

		if (other.gameObject.GetComponent<PlayerBody>()) {
			SceneController.Crashed();
		}
	}

}
