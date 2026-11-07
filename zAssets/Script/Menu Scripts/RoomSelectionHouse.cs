using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSelection : MonoBehaviour
{
    public void Backbtn()
    {
        SceneManager.LoadScene("Environment Selection");
    }

    public void Bedroom()
    {
        SceneManager.LoadScene("Bedroom");
    }

    public void Kitchen()
    {
        SceneManager.LoadScene("Kitchen");
    }

    public void Bathroom()
    {
        SceneManager.LoadScene("Bathroom");
    }
}
