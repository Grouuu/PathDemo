using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

	public static void Crashed() {
		string currentSceneName = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(currentSceneName);
	}

}
