using UnityEngine;

public class ObstacleBody : MonoBehaviour {

	[SerializeField] private ObstacleData _data;

	//[field: SerializeField] public ObstacleData Data { get; private set; }
	//[field: SerializeField] public ObstacleData Data { get => _data; private set => _data = value; }
	//public ObstacleData Data { get => _data; private set => _data = value; }

	public float Mass => _data.Mass;
	public float Radius => _data.Radius;
	public float RadiusGravity => _data.RadiusGravity;

	// DEBUG
	[SerializeField] public GridPoint point;

	public void SetData(ObstacleData data) {
		_data = data;
	}

	public void SetSizefactor(float sizeFactor) {
		float scale = Radius * sizeFactor;
		transform.localScale = new Vector3(scale, scale, scale);
	}

	//private void OnDrawGizmos () {
	//	Gizmos.color = Color.green;
	//	Gizmos.DrawWireSphere(transform.position, _data.RadiusGravity);

		//float outsideRadius = _data.RadiusGravity - _data.Radius;
		//int stepTotal = 10;
		//float stepLength = outsideRadius / stepTotal;

		//for (int i = 0; i < stepTotal; i++) {
		//	float distanceToSurface = _data.Radius + stepLength * i;
		//	float gravityForce = GravityController.GetGravityForceByDistance(distanceToSurface, this);
		//	float gravityForceNormalized = Mathf.InverseLerp(0, GravityController.GetGravityMax(this), gravityForce);
		//	Color color = Color.Lerp(Color.blue, Color.red, gravityForceNormalized);
		//	Gizmos.color = color;
		//	Gizmos.DrawWireSphere(transform.position, _data.Radius + distanceToSurface);
		//}
	//}
}
