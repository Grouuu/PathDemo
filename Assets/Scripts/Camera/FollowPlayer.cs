using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	[SerializeField] private GameObject _player;
	[SerializeField] private Vector3 _offset = new Vector3(1.5f, 0, -10);

	private void Update() {
		Camera.main.transform.position = _player.transform.position + _offset;
	}

}
