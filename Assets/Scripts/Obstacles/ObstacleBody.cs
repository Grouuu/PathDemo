using UnityEngine;

[System.Serializable]
public class ObstacleBody : MonoBehaviour {

	public float radius = 1;
	public float mass = 10;
	public float radiusGravity = 5;

	public Vector3 Position() {
		return transform.position;
	}

	private void OnDrawGizmos() {
		//Gizmos.color = Color.red;
		//Gizmos.DrawSphere(this.transform.position, radiusGravity);
	}
}
