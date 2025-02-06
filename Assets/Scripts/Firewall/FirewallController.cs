using UnityEngine;

public class FirewallController : MonoBehaviour {

	public FirewallController Instance { get; private set; }

	[SerializeField] private Transform _firewall;
	[SerializeField] private FirewallBody _firewallBody;
	[SerializeField] private float _speed = 1;
	[SerializeField] private bool _isDisabled = false;

	private void Awake () {
		Instance = this;
	}

	private void Update () {

		if (_isDisabled) {
			return;
		}

		_firewall.transform.Translate(Vector3.right * _speed * Time.deltaTime);
	}

}
