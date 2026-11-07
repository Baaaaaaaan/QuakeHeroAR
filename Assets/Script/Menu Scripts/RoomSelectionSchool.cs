using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSelectionSchool : MonoBehaviour
{
    public void Backbtn()
    {
        SceneManager.LoadScene("Environment Selection");
    }

    public void Classroom()
    {
        SceneManager.LoadScene("Classroom");
    }

    public void Library()
    {
        SceneManager.LoadScene("Library");
    }

    public void Cafeteria()
    {
        SceneManager.LoadScene("Cafeteria");
    }
}
