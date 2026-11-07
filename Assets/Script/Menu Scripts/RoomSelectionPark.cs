using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSelectionPark : MonoBehaviour
{
    public void Backbtn()
    {
        SceneManager.LoadScene("Environment Selection");
    }

    public void Playground()
    {
        SceneManager.LoadScene("Playground");
    }

    public void FoodCourt()
    {
        SceneManager.LoadScene("FoodCourt");
    }

}
