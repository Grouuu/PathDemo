using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBody : MonoBehaviour
{
	public Vector3 position => _rigidBody.position;
	public Vector3 forward => transform.right;
	public Vector3 up => transform.forward;

	private Rigidbody _rigidBody;

	public void Rotate (Quaternion rotation)
	{
		_rigidBody.rotation *= rotation;
	}

	public void Translate (Vector3 translation)
	{
		_rigidBody.position += translation;
	}

	private void Awake ()
	{
		_rigidBody = GetComponent<Rigidbody>();
	}

}
