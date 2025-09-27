// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class AudioManager : MonoBehaviour
// {
//     public static AudioManager Instance;

//     [Header("---------- Audio Source ----------")]
//     [SerializeField] AudioSource musicSource;
//     [SerializeField] AudioSource SFXSource;

//     [Header("---------- Music Clips ----------")]
//     public AudioClip menuMusic;   // Nhạc khi chưa vào game (menu, home, title)
//     public AudioClip gameMusic;   // Nhạc trong game

//     [Header("---------- SFX Clips ----------")]
//     public AudioClip push;        // Đẩy hộp
//     public AudioClip buttonClick; // Click button (trừ touchpad)
//     public AudioClip victory;     // Hoàn thành level / thắng

//     void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//         DontDestroyOnLoad(gameObject);
//     }

//     void OnEnable()
//     {
//         SceneManager.sceneLoaded += OnSceneLoaded;
//     }

//     void OnDisable()
//     {
//         SceneManager.sceneLoaded -= OnSceneLoaded;
//     }

//     void Start()
//     {
//         PlayForCurrentScene();
//     }

//     void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         PlayForCurrentScene();
//     }

//  void PlayForCurrentScene()
// {
// 	string name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToLower();

// 	// Bổ sung các từ khóa thuộc nhóm menu
// 	bool isMenu =
// 		name.Contains("menu") ||
// 		name.Contains("title") ||
// 		name.Contains("home")  ||
// 		name.Contains("select");    
//         name.Contains("selecLevel");   // <- thêm dòng này để scene SelectLevel vẫn phát nhạc menu

// 	AudioClip clip = isMenu ? menuMusic : gameMusic;
// 	if (clip == null) return;

// 	if (musicSource.clip != clip || !musicSource.isPlaying)
// 	{
// 		musicSource.clip = clip;
// 		musicSource.loop = true;
// 		musicSource.spatialBlend = 0f;
// 		musicSource.Play();
// 	}
// }

//     // Public APIs để chỗ khác gọi
//     public void PlayPush()            { PlaySfx(push); }
//     public void PlayButtonClick()     { PlaySfx(buttonClick); }
//     public void PlayVictory()         { PlaySfx(victory); }

//     public void PlaySfx(AudioClip clip, float volume = 1f)
//     {
//         if (clip != null && SFXSource != null)
//             SFXSource.PlayOneShot(clip, volume);
//     }
// }


using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

	[Header("---------- Audio Source ----------")]
	[SerializeField] AudioSource musicSource;
	[SerializeField] AudioSource SFXSource;

	[Header("---------- Music Clips ----------")]
	public AudioClip menuMusic;   // Nhạc khi chưa vào game (menu, home, title)
	public AudioClip gameMusic;   // Nhạc trong game

	[Header("---------- SFX Clips ----------")]
	public AudioClip push;        // Đẩy hộp
	public AudioClip buttonClick; // Click button (trừ touchpad)
	public AudioClip victory;     // Hoàn thành level / thắng

	// Master volume cho cả Music & SFX (0..1)
	[Range(0f,1f)] public float masterVolume = 1f;
	const string VolumeKey = "audio.master";

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		if (PlayerPrefs.HasKey(VolumeKey))
			masterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, 1f));
		ApplyVolumes();
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void Start()
	{
		PlayForCurrentScene();
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		PlayForCurrentScene();
	}

	void PlayForCurrentScene()
	{
		string name = SceneManager.GetActiveScene().name.ToLower();

		// Nhóm scene menu
		bool isMenu =
			name.Contains("menu")  ||
			name.Contains("title") ||
			name.Contains("home")  ||
			name.Contains("selec") ||
			name.Contains("select");

		AudioClip clip = isMenu ? menuMusic : gameMusic;
		if (clip == null) return;

		if (musicSource.clip != clip || !musicSource.isPlaying)
		{
			musicSource.clip = clip;
			musicSource.loop = true;
			musicSource.spatialBlend = 0f;
			musicSource.volume = masterVolume;
			musicSource.Play();
		}
		else
		{
			// cập nhật volume nếu đang phát sẵn
			musicSource.volume = masterVolume;
		}
	}

	// Public APIs
	public void PlayPush()            { PlaySfx(push); }
	public void PlayButtonClick()     { PlaySfx(buttonClick); }
	public void PlayVictory()         { PlaySfx(victory); }

	public void PlaySfx(AudioClip clip, float volume = 1f)
	{
		if (clip != null && SFXSource != null)
			SFXSource.PlayOneShot(clip, volume * masterVolume);
	}

	public void ApplyVolumes()
	{
		if (musicSource != null) musicSource.volume = masterVolume;
		if (SFXSource != null) SFXSource.volume = masterVolume;
	}

	public void SetMasterVolume(float v)
	{
		masterVolume = Mathf.Clamp01(v);
		ApplyVolumes();
		PlayerPrefs.SetFloat(VolumeKey, masterVolume);
		PlayerPrefs.Save();
	}

	public float GetMasterVolume() { return masterVolume; }
}