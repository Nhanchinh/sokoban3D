using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	public bool IsHeld { get; private set; }

	public void OnPointerDown(PointerEventData eventData) { IsHeld = true; }
	public void OnPointerUp(PointerEventData eventData)   { IsHeld = false; }
	public void OnPointerExit(PointerEventData eventData) { IsHeld = false; }

	// Ép nhả (dùng khi chuyển màn/hiện Win)
	public void ForceRelease() { IsHeld = false; }
}