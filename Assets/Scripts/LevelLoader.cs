using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	[Tooltip("Nếu đã là level cuối thì quay về scene này (ví dụ: SelectLevel hoặc Menu)")]
	public string fallbackScene = "SelecLevel";

	public void LoadNextLevel()
	{
		int current = SceneManager.GetActiveScene().buildIndex;
		int next = current + 1;

		if (next < SceneManager.sceneCountInBuildSettings)
		{
			SceneManager.LoadScene(next);
		}
		else
		{
			if (!string.IsNullOrEmpty(fallbackScene))
				SceneManager.LoadScene(fallbackScene);
		}
	}

	// Tuỳ chọn: kiểm tra có level tiếp theo không (để ẩn/disable nút)
	public bool HasNextLevel()
	{
		int current = SceneManager.GetActiveScene().buildIndex;
		return (current + 1) < SceneManager.sceneCountInBuildSettings;
	}
}