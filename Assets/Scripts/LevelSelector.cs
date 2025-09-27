using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
	public void SelectLevel(int level)
	{
		LevelState.SelectedLevel = level;
		SceneManager.LoadScene("Game");
	}
}