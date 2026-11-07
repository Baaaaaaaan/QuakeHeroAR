
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RoomUnlockButton : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private string roomSceneName;

    [Header("Optional Lock UI")]
    [SerializeField] private GameObject lockOverlay;

    private Button roomButton;

    private void Awake()
    {
        roomButton = GetComponent<Button>();
    }

    private void Start()
    {
        RefreshLockState();
    }

    public void RefreshLockState()
    {
        bool unlocked =
            RoomProgressManager.IsRoomUnlocked(roomSceneName);

        if (roomButton != null)
            roomButton.interactable = unlocked;

        if (lockOverlay != null)
            lockOverlay.SetActive(!unlocked);
    }
}
