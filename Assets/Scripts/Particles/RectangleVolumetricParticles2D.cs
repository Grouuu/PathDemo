using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RectangleVolumetricParticles2D : MonoBehaviour
{
	[SerializeField] private string _id;
	[SerializeField] private ParticleSystem _particleSystem;
	[SerializeField] private Transform _target;
	[SerializeField] private int _totalParticles = 100;
	[SerializeField] private float _scaleRelatedToScene = 2;

	private Particle[] _particles;

	/*
	 * PARTICLES SYSTEM CONFIG
	 * . duration: max (100_000)
	 * . looping: true
	 * . start lifetime: Infinity
	 */

	// TODO
	// . set the mandatory configs to the particles system

	void Start ()
	{
		_particles = new Particle[_particleSystem.main.maxParticles];

		_particleSystem.Play();
		_particleSystem.Emit(_totalParticles);
	}

	void Update ()
	{
		int particleCount = _particleSystem.GetParticles(_particles);

		// scale the size of the scene
		Bounds killBounds = Utils.GetSceneBounds(_target.position, Vector2.one * _scaleRelatedToScene);

		for (int i = 0; i < particleCount; i++)
		{
			Particle particle = _particles[i];
			Vector2 offset = GetOffset(particle.position, killBounds);

			if (offset != Vector2.zero)	{
				_particles[i].position = WrapParticlePosition(particle.position, offset, killBounds);
			}
		}

		_particleSystem.SetParticles(_particles, particleCount);
	}

	private Vector2 WrapParticlePosition (Vector2 particlePosition, Vector2 offset, Bounds bounds)
	{
		if (offset.x == 1) {
			particlePosition.x += bounds.size.x;
		}
		else if (offset.x == -1) {
			particlePosition.x -= bounds.size.x;
		}

		if (offset.y == 1) {
			particlePosition.y += bounds.size.y;
		}
		else if (offset.y == -1) {
			particlePosition.y -= bounds.size.y;
		}

		return particlePosition;
	}

	private Vector2 GetOffset (Vector2 position, Bounds bounds)
	{
		Vector2 offset = Vector2.zero;

		if (position.x < bounds.min.x) {
			offset.x = 1;
		}
		else if (position.x > bounds.max.x)	{
			offset.x = -1;
		}

		if (position.y < bounds.min.y) {
			offset.y = 1;
		}
		else if (position.y > bounds.max.y)	{
			offset.y = -1;
		}

		return offset;
	}

}
