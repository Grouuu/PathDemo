using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	[SerializeField] private GameObject _player;

	private void Update() {
		transform.position = _player.transform.position + new Vector3(0, 0, -10);
	}

}
