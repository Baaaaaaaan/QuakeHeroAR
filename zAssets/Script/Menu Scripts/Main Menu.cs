using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Environment Selection");
    }

    public void OpenAvatar()
    {
        SceneManager.LoadScene("Avatar");
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}