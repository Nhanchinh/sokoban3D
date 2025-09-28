using UnityEngine;

public class ProgressButtons : MonoBehaviour
{
    public void UnlockAll()
    {
        PlayerPrefs.SetInt("MaxLevelUnlocked", 16);
        PlayerPrefs.Save();
        Debug.Log("All levels unlocked!");
    }

    public void ResetProgress()
    {
        PlayerPrefs.SetInt("MaxLevelUnlocked", 1);
        PlayerPrefs.Save();
        Debug.Log("Progress reset!");
    }
}
