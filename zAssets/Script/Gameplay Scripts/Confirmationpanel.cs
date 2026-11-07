using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmationPanel : MonoBehaviour
{
    [SerializeField] private Button gotItButton;

    [Tooltip("Fires when the player taps Got It. Wire the next step here, e.g. HazardManager.GoToNextRoom.")]
    public UnityEvent onConfirmed;

    private void Awake()
    {
        if (gotItButton != null) gotItButton.onClick.AddListener(OnGotIt);
    }

    private void OnGotIt()
    {
        gameObject.SetActive(false);
        onConfirmed?.Invoke();
    }
}