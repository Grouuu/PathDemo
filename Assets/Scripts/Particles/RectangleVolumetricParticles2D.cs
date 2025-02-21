using UnityEngine;
using static UnityEngine.ParticleSystem;

/*
 * Particle System overrides:
 * . main.startLifetime
 * . main.maxParticles
 * . main.stopActions
 * . shape.shapeType
 * . shape.scale
 * . emission.rateOverTime
 * . emission.rateOverDistance
 */

public class RectangleVolumetricParticles2D : MonoBehaviour
{
	[SerializeField] private string _id;
	[SerializeField] private ParticleSystem _particleSystem;
	[SerializeField] private Transform _target;
	[SerializeField] private int _totalParticles = 100;
	[SerializeField] private float _scaleRelatedToScene = 2;
	[SerializeField] private bool _isDebug = false;

	private Particle[] _particles;
	private Bounds _bounds;

	private void Start ()
	{
		_particles = new Particle[_totalParticles];

		_bounds = GetBounds(); // TODO update when screen size change

		ConfigParticlesSystem();
	}

	private void Update ()
	{
		int particleCount = _particleSystem.GetParticles(_particles);

		for (int i = 0; i < particleCount; i++)
		{
			Particle particle = _particles[i];
			Vector2 offset = GetOffset(particle.position);

			if (offset != default)	{
				_particles[i].position = WrapParticlePosition(particle.position, offset);
			}
		}

		_particleSystem.SetParticles(_particles, particleCount);
	}

	private void ConfigParticlesSystem ()
	{
		MainModule main = _particleSystem.main;
		main.startLifetime = float.PositiveInfinity;
		main.maxParticles = _totalParticles;
		main.stopAction = ParticleSystemStopAction.None;

		ShapeModule shape = _particleSystem.shape;
		shape.shapeType = ParticleSystemShapeType.Rectangle;
		shape.scale = _bounds.size;

		EmissionModule emission = _particleSystem.emission;
		emission.rateOverTime = 0;
		emission.rateOverDistance = 0;

		_particleSystem.Play();
		_particleSystem.Emit(_totalParticles);
	}

	private Vector2 WrapParticlePosition (Vector2 particlePosition, Vector2 offset)
	{
		if (offset.x == 1) {
			particlePosition.x += _bounds.size.x;
		}
		else if (offset.x == -1) {
			particlePosition.x -= _bounds.size.x;
		}

		if (offset.y == 1) {
			particlePosition.y += _bounds.size.y;
		}
		else if (offset.y == -1) {
			particlePosition.y -= _bounds.size.y;
		}

		return particlePosition;
	}

	private Vector2 GetOffset (Vector2 position)
	{
		Vector2 offset = default;

		if (position.x < _bounds.min.x) {
			offset.x = 1;
		}
		else if (position.x > _bounds.max.x)	{
			offset.x = -1;
		}

		if (position.y < _bounds.min.y) {
			offset.y = 1;
		}
		else if (position.y > _bounds.max.y)	{
			offset.y = -1;
		}

		return offset;
	}

	private Bounds GetBounds ()
	{
		return Utils.GetSceneBounds(_target.position, Vector2.one * _scaleRelatedToScene);
	}

#if UNITY_EDITOR

	private void OnDrawGizmos ()
	{
		if (_isDebug) {
			Debug.DrawCenteredRectangle(_particleSystem.transform.position, Utils.GetSceneSize(), Color.white, 0);
			Debug.DrawCenteredRectangle(_particleSystem.transform.position, Utils.GetSceneSize() * _scaleRelatedToScene, Color.red, 0);
		}
	}

#endif

}
