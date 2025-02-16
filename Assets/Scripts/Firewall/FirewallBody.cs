using UnityEngine;

public delegate void OnFirewallCrash ();

public class FirewallBody : MonoBehaviour {

	public static OnFirewallCrash OnFirewallCrash;

	private void OnTriggerEnter (Collider other) {

		if (other.gameObject.GetComponent<PlayerBody>()) {
			OnFirewallCrash();
		}
	}

}
