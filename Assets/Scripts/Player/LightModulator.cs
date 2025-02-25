using UnityEngine;

public class LightModulator : MonoBehaviour
{
	[SerializeField] private Light _light;
	[SerializeField] private float _minRange = 0;
	[SerializeField] private float _maxRange = 50;
	[SerializeField] private float _minIntensity = 0;
	[SerializeField] private float _maxIntensity = 1;

	private void Update ()
	{
		if (_light == null)	{
			return;
		}

		float distance = Vector3.Distance(transform.position, _light.transform.position);
		float intensity = 1 - Mathf.InverseLerp(_minRange, _maxRange, distance);
		intensity = Mathf.Lerp(_minIntensity, _maxIntensity, intensity);

		_light.intensity = intensity;
	}

}
