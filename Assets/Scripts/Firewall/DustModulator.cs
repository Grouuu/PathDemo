using UnityEngine;

public class DustModulator : MonoBehaviour
{
	[SerializeField] private Transform _target;
	[SerializeField] private MeshRenderer _fogMesh;
	[SerializeField] private float _minRange = 0;
	[SerializeField] private float _maxRange = 50;
	[SerializeField] private float _minIntensity = 0;
	[SerializeField] private float _maxIntensity = 1;


	private void Update ()
	{
		if (_target == null || _fogMesh == null)
		{
			return;
		}

		float distance = Vector3.Distance(_target.position, transform.position);
		float intensity = Utils.Remap(_minRange, _maxRange, _minIntensity, _maxIntensity, distance);

		_fogMesh.sharedMaterial.SetFloat("_Intensity", intensity);
	}

}
