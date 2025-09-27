// using UnityEngine;

// public class GameManager : MonoBehaviour
// {
// 	public GameObject winScreen;
// 	private bool isWinning = false;

// 	void Update()
// 	{
// 		if (!isWinning && CheckWinDynamic())
// 		{
// 			isWinning = true;
// 			Debug.Log("YOU WIN!");
// 			if (winScreen != null) winScreen.SetActive(true);
// 		}
// 	}

// 	private bool CheckWinDynamic()
// 	{
// 		// Lấy danh sách Box hiện có trong scene mỗi lần kiểm tra
// 		var boxes = FindObjectsOfType<BoxController>();
// 		if (boxes == null || boxes.Length == 0) return false;

// 		foreach (var box in boxes)
// 		{
// 			if (!box.IsOnGoal())
// 				return false;
// 		}
// 		return true;
// 	}
// }















// using UnityEngine;
// using System.Collections.Generic;
// using UnityEngine.SceneManagement;
// public class GameManager : MonoBehaviour
// {
// 	public GameObject winScreen;
// 	private bool isWinning = false;

// 	private void OnEnable()
// 	{
// 		BoxController.OnAnyBoxGoalStateChanged += TryWin;
// 	}

// 	private void OnDisable()
// 	{
// 		BoxController.OnAnyBoxGoalStateChanged -= TryWin;
// 	}

// 	private void Start()
// 	{
// 		// Kiểm tra ngay khi vào scene (phòng trường hợp box đã ở goal)
// 		TryWin();
// 	}
// 	public List<GameObject> list_obj = new(); 
// 	private void TryWin()
// 	{
// 		if (isWinning) return;
// 		if (CheckWinDynamic())
// 		{
// 			foreach (var item in list_obj)
// 			{
// 				item.SetActive(false);
// 			}
// 			isWinning = true;
// 			Debug.Log("YOU WIN!");
// 			ClearControls();
// 			if(AudioManager.Instance != null)
// 			{
// 				AudioManager.Instance.PlayVictory();
// 			}
// 			if (winScreen != null) winScreen.SetActive(true);

// 		}
// 	}

// 	private bool CheckWinDynamic()
// 	{
// 		var boxes = FindObjectsOfType<BoxController>();
// 		if (boxes == null || boxes.Length == 0) return false;

// 		foreach (var box in boxes)
// 			if (!box.IsOnGoal()) return false;

// 		return true;
// 	}
// public void NextLevel()
// {
// 	int current = Mathf.Max(1, LevelState.SelectedLevel);
// 	int next = current + 1;

// 	// kiểm tra map kế tiếp có tồn tại không (Resources/Maps/level{n}.txt)
// 	string path = $"Maps/level{next}";
// 	var ta = Resources.Load<TextAsset>(path);

// 	if (ta == null)
// 	{
// 		// hết map -> quay về màn chọn level
// 		SceneManager.LoadScene("SelecLevel"); // đổi tên scene nếu bạn dùng tên khác
// 		return;
// 	}

// 	// có map -> tăng level và nạp lại
// 	LevelState.SelectedLevel = next;
// 	var loader = FindObjectOfType<MapLoader>();
// 	if (loader != null) loader.LoadCurrentLevel();

// 	// ẩn win + reset cờ
// 	if (winScreen != null) winScreen.SetActive(false);
// 	isWinning = false;
// }
// public void ResetLevel()
// {
// 	ClearControls();
// 	// nạp lại đúng level hiện tại
// 	var loader = FindObjectOfType<MapLoader>();
// 	if (loader != null) loader.LoadCurrentLevel();

// 	// tắt win (nếu đang bật) và reset trạng thái
// 	if (winScreen != null) winScreen.SetActive(false);
// 	isWinning = false;
// }
// public void ClearControls()
// {
// 	var player = FindObjectOfType<PlayerController>();
// 	if(dpad != null)
// 	{
// 		dpad.ReleaseAll();
// 	}
// 	var player = FindObjectOfType<PlayerController>();
// 	if(player != null)
// 	{
// 		player.ResetInput();
// 	}
// }
// }


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public GameObject winScreen;
	private bool isWinning = false;

	private void OnEnable()
	{
		BoxController.OnAnyBoxGoalStateChanged += TryWin;
	}

	private void OnDisable()
	{
		BoxController.OnAnyBoxGoalStateChanged -= TryWin;
	}

	private void Start()
	{
		// Kiểm tra ngay khi vào scene (phòng trường hợp box đã ở goal)
		TryWin();
	}

	public List<GameObject> list_obj = new();

	private void TryWin()
	{
		if (isWinning) return;
		if (CheckWinDynamic())
		{
			foreach (var item in list_obj)
			{
				item.SetActive(false);
			}
			isWinning = true;
			Debug.Log("YOU WIN!");

			// Nhả toàn bộ input để tránh kẹt nút khi sang màn mới
			ClearControls();

			if (AudioManager.Instance != null)
			{
				AudioManager.Instance.PlayVictory();
			}
			if (winScreen != null) winScreen.SetActive(true);
		}
	}

	private bool CheckWinDynamic()
	{
		var boxes = FindObjectsOfType<BoxController>();
		if (boxes == null || boxes.Length == 0) return false;

		foreach (var box in boxes)
			if (!box.IsOnGoal()) return false;

		return true;
	}

	public void NextLevel()
	{
		// Nhả input trước khi chuyển level
		ClearControls();

		int current = Mathf.Max(1, LevelState.SelectedLevel);
		int next = current + 1;

		// kiểm tra map kế tiếp có tồn tại không (Resources/Maps/level{n}.txt)
		string path = $"Maps/level{next}";
		var ta = Resources.Load<TextAsset>(path);

		if (ta == null)
		{
			// hết map -> quay về màn chọn level
			SceneManager.LoadScene("SelectLevel");
			return;
		}

		// có map -> tăng level và nạp lại
		LevelState.SelectedLevel = next;
		var loader = FindObjectOfType<MapLoader>();
		if (loader != null) loader.LoadCurrentLevel();

		// ẩn win + reset cờ
		if (winScreen != null) winScreen.SetActive(false);
		isWinning = false;
	}

	public void ResetLevel()
	{
		// Nhả input trước khi reset
		ClearControls();

		// nạp lại đúng level hiện tại
		var loader = FindObjectOfType<MapLoader>();
		if (loader != null) loader.LoadCurrentLevel();

		// tắt win (nếu đang bật) và reset trạng thái
		if (winScreen != null) winScreen.SetActive(false);
		isWinning = false;
	}

	// Hàm dùng chung để nhả D-pad và reset input
	private void ClearControls()
{
	// Tìm cả object inactive
	var dpads = Resources.FindObjectsOfTypeAll<MobileDpad>();
	if (dpads != null && dpads.Length > 0)
	{
		// lấy cái đầu tiên đang dùng trong scene (không phải editor preview)
		foreach (var d in dpads)
		{
			if (d == null) continue;
			if (d.gameObject.scene.IsValid()) { d.ReleaseAll(); break; }
		}
	}

	// Nhả tất cả HoldButton (kể cả đang inactive)
	var holds = Resources.FindObjectsOfTypeAll<HoldButton>();
	foreach (var h in holds)
	{
		if (h == null) continue;
		if (h.gameObject.scene.IsValid()) h.ForceRelease();
	}

	// Reset input cho mọi Player hiện có (nếu còn)
	var players = Resources.FindObjectsOfTypeAll<PlayerController>();
	foreach (var p in players)
	{
		if (p == null) continue;
		if (p.gameObject.scene.IsValid()) p.ResetInput();
	}
}
}