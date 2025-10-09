
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro; // Thêm dòng này

public class GameManager : MonoBehaviour
{
	public GameObject winScreen;
	private bool isWinning = false;

	// THÊM UI REFERENCE
	[Header("UI")]
	public TextMeshProUGUI levelDisplayText;

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
		
		// THÊM CẬP NHẬT HIỂN THỊ LEVEL
		UpdateLevelDisplay();
	}

	// THÊM FUNCTION CẬP NHẬT LEVEL DISPLAY
	private void UpdateLevelDisplay()
	{
		if (levelDisplayText != null)
		{
			levelDisplayText.text = $"Level {LevelState.SelectedLevel}";
		}
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

			// THÊM: Lưu tiến độ khi thắng
			int currentLevel = LevelState.SelectedLevel;
			int maxUnlocked = PlayerPrefs.GetInt("MaxLevelUnlocked", 1);
			if (currentLevel >= maxUnlocked)
			{
				PlayerPrefs.SetInt("MaxLevelUnlocked", currentLevel + 1);
				PlayerPrefs.Save();
			}

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
			SceneManager.LoadScene("SelecLevel");
			return;
		}

		// có map -> tăng level và nạp lại
		LevelState.SelectedLevel = next;
		var loader = FindObjectOfType<MapLoader>();
		if (loader != null) loader.LoadCurrentLevel();

		// ẩn win + reset cờ
		if (winScreen != null) winScreen.SetActive(false);
		isWinning = false;
		
		// THÊM CẬP NHẬT HIỂN THỊ LEVEL
		UpdateLevelDisplay();
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
		
		// THÊM CẬP NHẬT HIỂN THỊ LEVEL
		UpdateLevelDisplay();
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