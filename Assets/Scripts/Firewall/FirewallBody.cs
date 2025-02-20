using UnityEngine;

public delegate void OnFirewallCrash ();

/*
 * Dependencies:
 * . PlayerBody
 */
[RequireComponent (typeof(Collider))]
public class FirewallBody : MonoBehaviour
{
	public static event OnFirewallCrash OnFirewallCrash;

	private void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.GetComponent<PlayerBody>())
		{
			OnFirewallCrash?.Invoke();
		}
	}

}
