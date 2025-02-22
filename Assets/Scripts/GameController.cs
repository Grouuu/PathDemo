using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Dependencies:
 * . PlayerMovement
 * . FirewallBody
 * . ObstacleController
 * . DifficultyController
 */
public class GameController : MonoBehaviour
{
	[SerializeField] private PlayerMovement _playerController;
	[SerializeField] private ObstacleController _obstacleController;
	[SerializeField] private DifficultyController _difficultyController;

	private void Start ()
	{
		DifficultyUpdated();
	}

	private void OnEnable ()
	{
		PlayerMovement.OnUpdatePlayerPosition += OnUpdatePlayerPosition;
		FirewallBody.OnFirewallCrash += CrashOnFirewall;
	}

	private void OnDisable ()
	{
		PlayerMovement.OnUpdatePlayerPosition -= OnUpdatePlayerPosition;
		FirewallBody.OnFirewallCrash -= CrashOnFirewall;
	}

	private void OnUpdatePlayerPosition (Vector3 position)
	{
		_difficultyController.UpdateDifficulty(position);

		DifficultyUpdated();

		if (_obstacleController.IsCrashPosition(position))
		{
			IsCrashed();
		}
	}

	private void CrashOnFirewall ()
	{
		IsCrashed();
	}

	private void IsCrashed ()
	{
		_playerController.SetIsCrashed(true);

		ReloadStage();
	}

	private void DifficultyUpdated ()
	{
		_obstacleController.SetDifficulty(_difficultyController.GetDifficulty());
	}

	private void ReloadStage ()
	{
		string currentSceneName = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(currentSceneName);
	}

}
