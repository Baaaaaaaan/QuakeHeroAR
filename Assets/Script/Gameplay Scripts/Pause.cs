using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject darkenEffect;

    [Header("Gameplay Input Blocking")]
    [Tooltip("Drag the 'Hazards Found' object here. Its CanvasGroup will be disabled while paused so clicks can't reach hazard buttons behind the pause panel.")]
    [SerializeField] private CanvasGroup hazardsCanvasGroup;

    [Tooltip("Drag the 'Not Hazard' object here. Same reasoning as above.")]
    [SerializeField] private CanvasGroup notHazardCanvasGroup;

    private void Start()
    {
        pausePanel.SetActive(false);
        darkenEffect.SetActive(false);

        Time.timeScale = 1f;

        SetGameplayInteractable(true);
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        darkenEffect.SetActive(true);

        Time.timeScale = 0f;

        SetGameplayInteractable(false);
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        darkenEffect.SetActive(false);

        Time.timeScale = 1f;

        SetGameplayInteractable(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoHome()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Main Menu");
    }

    public void ReturnToRoomSelection()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("RoomSelectionHouse");
    }

    private void SetGameplayInteractable(bool value)
    {
        if (hazardsCanvasGroup != null)
        {
            hazardsCanvasGroup.interactable = value;
            hazardsCanvasGroup.blocksRaycasts = value;
        }

        if (notHazardCanvasGroup != null)
        {
            notHazardCanvasGroup.interactable = value;
            notHazardCanvasGroup.blocksRaycasts = value;
        }
    }
}