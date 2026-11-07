using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentSelection : MonoBehaviour
{
    public void Backbtn()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void HouseRoomSelection()
    {
        SceneManager.LoadScene("RoomSelectionHouse");
    }

    public void SchoolRoomSelection()
    {
        SceneManager.LoadScene("RoomSelectionSchool");
    }

    public void ParkRoomSelection()
    {
        SceneManager.LoadScene("RoomSelectionPark");
    }
}
