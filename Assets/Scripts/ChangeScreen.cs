using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScreen : MonoBehaviour
{
    public void ChangeScreen1(string name)
    {
        SceneManager.LoadScene(name);
    }
     public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
