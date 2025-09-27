using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonClickSfx : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
	public bool playOnDown = false; // false: kêu khi Click (nhả chuột); true: kêu khi nhấn xuống
	public float volume = 1f;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (playOnDown) Play();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!playOnDown) Play();
	}

	void Play()
	{
		if (AudioManager.Instance != null)
			AudioManager.Instance.PlayButtonClick();
	}
}