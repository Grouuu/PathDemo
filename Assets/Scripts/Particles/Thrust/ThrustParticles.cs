using UnityEngine;
using static UnityEngine.ParticleSystem;

/**
 * Require:
 * . PlayerMovement
 */
public class ThrustParticles : MonoBehaviour {

	[SerializeField] private ParticleSystem _particleSystem;
	[SerializeField] private PlayerMovement _playerMovement;
	[SerializeField] private float _minSpeed = 1;
	[SerializeField] private float _maxSpeed = 10;

	private MainModule _particleSettings;
	private EmissionModule _particleEmission;

	private void Start () {
		_particleSettings = _particleSystem.main;
		_particleEmission = _particleSystem.emission;

		_particleSystem.Play();
	}

	private void Update () {

		float intensity = _playerMovement.ThrustForce;
		float speed = -intensity * Mathf.Lerp(_minSpeed, _maxSpeed, Mathf.Abs(intensity));

		_particleEmission.enabled = speed != 0;
		_particleSettings.startSpeed = speed;
	}

}
