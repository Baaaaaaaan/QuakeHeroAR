using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class HazardManager : MonoBehaviour
{
    public static HazardManager Instance { get; private set; }

    [Header("UI References")]
    public TMP_Text foundCounterText;

    [Header("Room Setup")]
    public RoomData currentRoom;

    public int totalHazards = 0;

    [Header("Events")]

    public UnityEvent onAllHazardsFound;

    private int foundCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (currentRoom != null && currentRoom.hazards.Count > 0)
        {
            ApplyRoomDataToScene();
        }

        if (totalHazards == 0)
        {
            Hazard[] all = FindObjectsOfType<Hazard>(true);
            foreach (var h in all)
            {
                if (h.isHazard) totalHazards++;
            }
        }
        UpdateCounterUI();
    }

    private void ApplyRoomDataToScene()
    {
        Hazard[] sceneHazards = FindObjectsOfType<Hazard>(true);

        foreach (var entry in currentRoom.hazards)
        {
            foreach (var hazard in sceneHazards)
            {
                bool matches = hazard.hazardId == entry.hazardId || hazard.gameObject.name == entry.hazardId;
                if (matches)
                {
                    hazard.displayName = entry.displayName;
                    hazard.hazardExplanation = entry.explanation;
                    hazard.isHazard = true;
                    break;
                }
            }
        }
    }

    public void GoToNextRoom()
    {
        if (currentRoom != null && !string.IsNullOrEmpty(currentRoom.nextSceneName))
        {
            SceneManager.LoadScene(currentRoom.nextSceneName);
        }
        else
        {
            Debug.LogWarning("HazardManager: No next scene set on currentRoom.");
        }
    }

    public void RegisterHazardFound(Hazard hazard)
    {
        foundCount++;
        UpdateCounterUI();

        if (foundCount >= totalHazards)
        {
            onAllHazardsFound?.Invoke();
        }
    }

    private void UpdateCounterUI()
    {
        if (foundCounterText != null)
        {
            foundCounterText.text = $"{foundCount}/{totalHazards}";
        }
    }

    public void ResetRoom()
    {
        foundCount = 0;
        UpdateCounterUI();
    }
}