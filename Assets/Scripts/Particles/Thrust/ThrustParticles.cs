using UnityEngine;
using static UnityEngine.ParticleSystem;

/*
 * Dependencies:
 * . PlayerMovement
 */
public class ThrustParticles : MonoBehaviour
{
	[SerializeField] private ParticleSystem _particleSystem;
	[SerializeField] private PlayerMovement _playerMovement;
	[SerializeField] private float _minSpeed = 1;
	[SerializeField] private float _maxSpeed = 10;

	private MainModule _particleSettings;
	private EmissionModule _particleEmission;

	private void Start ()
	{
		_particleSettings = _particleSystem.main;
		_particleEmission = _particleSystem.emission;

		_particleSystem.Play();
	}

	private void Update ()
	{
		float intensity = _playerMovement.thrustForce;
		float speed = -intensity * Mathf.Lerp(_minSpeed, _maxSpeed, Mathf.Abs(intensity));
		bool enabled = speed != 0;

		_particleEmission.enabled = enabled;
		_particleSettings.startSpeed = speed;
	}

}
