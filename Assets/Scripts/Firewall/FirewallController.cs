using UnityEngine;

public class FirewallController : MonoBehaviour {

	[SerializeField] private Transform _firewall;
	[SerializeField] private FirewallBody _firewallBody;
	[SerializeField] private float _speed = 1;
	[SerializeField] private bool _isDisabled = false;

	private void Update () {

		if (_isDisabled) {
			return;
		}

		_firewall.transform.Translate(Vector3.right * _speed * Time.deltaTime);
	}

}
