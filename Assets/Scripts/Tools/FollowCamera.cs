using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	private void Update ()
	{
		transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
	}

}
