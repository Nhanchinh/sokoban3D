using UnityEngine;
using UnityEngine.UI;

public class SettingsAudio : MonoBehaviour
{
	[SerializeField] Slider volumeSlider;

	void Start()
	{
		if (AudioManager.Instance != null && volumeSlider != null)
			volumeSlider.value = AudioManager.Instance.GetMasterVolume();

		if (volumeSlider != null)
			volumeSlider.onValueChanged.AddListener(OnSliderChanged);
	}

	void OnDestroy()
	{
		if (volumeSlider != null)
			volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);
	}

	void OnSliderChanged(float v)
	{
		if (AudioManager.Instance != null)
			AudioManager.Instance.SetMasterVolume(v);
	}
}