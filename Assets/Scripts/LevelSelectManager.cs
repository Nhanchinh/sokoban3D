using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelUI
    {
        public Button levelButton;
        public GameObject lockIcon;
    }

    public LevelUI[] levels;

    void Start()
    {
        int maxLevelUnlocked = PlayerPrefs.GetInt("MaxLevelUnlocked", 1);

        for (int i = 0; i < levels.Length; i++)
        {
            int level = i + 1;

            if (level <= maxLevelUnlocked)
            {
                // Mở khóa
                levels[i].levelButton.interactable = true;
                levels[i].lockIcon.SetActive(false);
            }
            else
            {
                // Khóa
                levels[i].levelButton.interactable = false;
                levels[i].lockIcon.SetActive(true);
            }
        }
    }
}
