using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	[SerializeField] private PlayerMovement _playerController;
	[SerializeField] private ObstacleController _obstacleController;

	private void OnEnable () {
		PlayerMovement.OnUpdatePlayerPosition += CheckPlayerCrash;
		FirewallBody.OnFirewallCrash += CrashOnFirewall;
	}

	private void OnDisable () {
		PlayerMovement.OnUpdatePlayerPosition -= CheckPlayerCrash;
		FirewallBody.OnFirewallCrash -= CrashOnFirewall;
	}

	private void CheckPlayerCrash (Vector3 position) {
		if (_obstacleController.IsCrashPosition(position)) {
			IsCrashed();
		}
	}

	private void CrashOnFirewall () {
		IsCrashed();
	}

	private void IsCrashed () {
		_playerController.SetIsCrashed(true);
		string currentSceneName = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(currentSceneName);
	}
}
