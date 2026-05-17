using UnityEngine;
using UnityEngine.SceneManagement;

public class Replay : MonoBehaviour
{

	public void ReplayGame()
	{
	SceneManager.LoadScene("1.FlappyBird");
	FindFirstObjectByType<Score>().ResetScore();
	akoJump.isGameStarted = false;
	GameManager.Instance.AddTokens(-10);
	}

}
