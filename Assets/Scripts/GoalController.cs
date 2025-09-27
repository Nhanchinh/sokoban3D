using UnityEngine;

public class GoalController : MonoBehaviour
{
    private Renderer rend;
    public Color normalColor = Color.yellow;  // Màu bình thường của Goal
    public Color highlightColor = Color.cyan; // Màu khi có Box đè lên

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = normalColor;
        }
    }

    public void Highlight(bool state)
    {
        if (rend != null)
        {
            rend.material.color = state ? highlightColor : normalColor;
        }
    }
}
