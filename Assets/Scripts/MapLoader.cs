using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
	[System.Serializable]
	public struct TileDef
	{
		public char id;
		public GameObject prefab;
		public float yOffset; // độ cao bù theo tile
	}

	[Header("Tile size")]
	public float cellSize = 1f;

	[Header("Mappings")]
	public List<TileDef> tiles;

	[Header("Optional parent")]
	public Transform worldParent;

	[Header("Player prefab for 'P'")]
	public GameObject playerPrefab;
	public float playerYOffset = 0.5f;

	private Dictionary<char, GameObject> _dict;
	private Dictionary<char, float> _yOffset;
	public MobileDpad mobileDpad;

	private void Awake()
	{
		_dict = new Dictionary<char, GameObject>();
		_yOffset = new Dictionary<char, float>();
		foreach (var t in tiles)
		{
			if (t.prefab != null && !_dict.ContainsKey(t.id))
			{
				_dict.Add(t.id, t.prefab);
				_yOffset[t.id] = t.yOffset;
			}
		}
	}

	private void Start()
	{
		LoadCurrentLevel();
	}

	public void LoadCurrentLevel()
	{
		int level = Mathf.Max(1, LevelState.SelectedLevel);
		string path = $"Maps/level{level}";
		var ta = Resources.Load<TextAsset>(path);
		if (ta == null)
		{
			Debug.LogError($"Map not found: {path} (.txt in Assets/Resources/Maps)");
			return;
		}

		if (worldParent != null)
		{
			for (int i = worldParent.childCount - 1; i >= 0; i--)
				Destroy(worldParent.GetChild(i).gameObject);
		}

		BuildFromText(ta.text);
	}

	private void BuildFromText(string text)
	{
		string[] lines = text.Replace("\r", "").Split('\n');
		for (int y = 0; y < lines.Length; y++)
		{
			string line = lines[y];
			for (int x = 0; x < line.Length; x++)
			{
				char c = line[x];
				Vector3 basePos = new Vector3(x * cellSize, 0f, -y * cellSize);

				// Ground mặc định nếu có map '.'
				if (_dict.TryGetValue('.', out var ground))
				{
					float gy = _yOffset.TryGetValue('.', out var offG) ? offG : 0f;
					Instantiate(ground, basePos + Vector3.up * gy, Quaternion.identity, worldParent);
				}

				// Player xử lý riêng
				// if (c == 'P')
				// {
				// 	if (playerPrefab != null)
				// 	{
				// 		GameObject player = Instantiate(playerPrefab, basePos, Quaternion.identity, worldParent);
				// 		mobileDpad.player = player.GetComponent<PlayerController>();
				// 	}
					
				// 	continue;
				// }

							if (c == 'P')
					{
						if (playerPrefab != null)
						{
							GameObject player = Instantiate(playerPrefab, basePos, Quaternion.identity, worldParent);
							var pc = player.GetComponent<PlayerController>();
							if (mobileDpad != null) mobileDpad.player = pc;

							// đảm bảo player mới luôn clean input
							if (pc != null) pc.ResetInput();
							if (mobileDpad != null) mobileDpad.ReleaseAll();
						}
						continue;
					}

				// Các tile khác theo ký tự
				if (_dict.TryGetValue(c, out var prefab))
				{
					float oy = _yOffset.TryGetValue(c, out var off) ? off : 0f;
					Instantiate(prefab, basePos + Vector3.up * oy, Quaternion.identity, worldParent);
				}
			}
		}
	}
}