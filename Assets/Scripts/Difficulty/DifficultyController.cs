using UnityEngine;

public class DifficultyController : MonoBehaviour
{
	[SerializeField] private float _startDifficulty = 1;
	[SerializeField] private float _distanceToIncreaseDifficulty = 50;

	private float _currentDifficulty = 1;

	public float GetDifficulty ()
	{
		return _currentDifficulty;
	}

	public bool UpdateDifficulty (Vector2 targetPosition)
	{
		bool isDirty = false;
		float oldDifficulty = _currentDifficulty;
		_currentDifficulty = _startDifficulty + targetPosition.x / _distanceToIncreaseDifficulty;

		if (oldDifficulty != _currentDifficulty)
		{
			isDirty = true;
		}

		return isDirty;
	}

	private void Start ()
	{
		_currentDifficulty = _startDifficulty;
	}

}
