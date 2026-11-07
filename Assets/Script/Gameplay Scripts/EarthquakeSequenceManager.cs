using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class EarthquakeSequenceManager : MonoBehaviour
{
    [Header("Phase Panels")]
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private GameObject duringPanel;
    [SerializeField] private GameObject afterPanel;
    [SerializeField] private GameObject safeToMovePanel;
    [SerializeField] private GameObject exteriorPanel;
    [SerializeField] private GameObject gameplayPanel;

    [Header("Warning Safe Spot")]
    [SerializeField] private SafeSpot[] warningSafeSpots;

    private bool warningCompleted = false;

    [Header("Countdown")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float shakeDurationSeconds = 6f;

    [Header("Timing")]
    [SerializeField] private float warningDuration = 3f;
    [SerializeField] private float afterToSafeDelay = 2f;

    [Header("Door")]
    [SerializeField] private DoorExit doorExit;

    [Header("Animation Hooks")]
    public UnityEvent onWarningStart;
    public UnityEvent onShakeStart;
    public UnityEvent onShakeEnd;
    public UnityEvent onSafeToMove;
    public UnityEvent onExitHouse;

    private void Awake()
    {
        if (doorExit != null)
        {
            doorExit.Setup(this);
        }

        SetWarningSafeSpots(false);
        HideAllPanels();
    }

    public void StartSequence()
    {
        if (gameplayPanel != null)
        {
            gameplayPanel.SetActive(false);
        }

        StartCoroutine(RunSequence());
    }

    public void OnWarningSafeSpotButtonPressed()
    {
        Debug.Log("Warning Safe Spot button pressed.");
        SetWarningSafeSpots(true);
    }

    public void OnCorrectSafeSpotSelected()
    {
        if (warningCompleted)
        {
            return;
        }

        Debug.Log("Correct safe spot selected! Moving to During Earthquake phase.");

        warningCompleted = true;
    }

    private void SetWarningSafeSpots(bool active)
    {
        if (warningSafeSpots == null)
        {
            return;
        }

        foreach (SafeSpot safeSpot in warningSafeSpots)
        {
            if (safeSpot == null)
            {
                continue;
            }

            if (active)
            {
                safeSpot.ResetFeedback();
                safeSpot.EnableInteraction();
            }
            else
            {
                safeSpot.DisableInteraction();
            }
        }
    }

    private IEnumerator RunSequence()
    {
        HideAllPanels();

        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
        }

        warningCompleted = false;

        SetWarningSafeSpots(true);

        onWarningStart?.Invoke();

        float elapsed = 0f;

        while (elapsed < warningDuration && !warningCompleted)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetWarningSafeSpots(false);

        HideAllPanels();

        if (duringPanel != null)
        {
            duringPanel.SetActive(true);
        }

        onShakeStart?.Invoke();

        if (GuideManager.Instance != null)
        {
            GuideManager.Instance.ShowGuide(
                GuideEventType.EarthquakeHold
            );
        }

        yield return StartCoroutine(
            RunCountdown(shakeDurationSeconds)
        );

        onShakeEnd?.Invoke();

        HideAllPanels();

        if (afterPanel != null)
        {
            afterPanel.SetActive(true);
        }

        yield return new WaitForSeconds(
            afterToSafeDelay
        );

        HideAllPanels();

        if (safeToMovePanel != null)
        {
            safeToMovePanel.SetActive(true);
        }

        onSafeToMove?.Invoke();

        if (GuideManager.Instance != null)
        {
            GuideManager.Instance.ShowGuide(
                GuideEventType.FollowExit
            );
        }

        if (doorExit != null)
        {
            doorExit.enabled = true;
        }
    }

    private IEnumerator RunCountdown(float seconds)
    {
        float remaining = seconds;

        while (remaining > 0f)
        {
            if (countdownText != null)
            {
                int wholeSeconds = Mathf.CeilToInt(remaining);
                countdownText.text = $"0:{wholeSeconds:00}";
            }

            yield return new WaitForSeconds(1f);

            remaining -= 1f;
        }

        if (countdownText != null)
        {
            countdownText.text = "0:00";
        }
    }

    public void OnDoorTapped()
    {
        if (doorExit != null)
        {
            doorExit.enabled = false;
        }

        onExitHouse?.Invoke();

        HideAllPanels();

        if (exteriorPanel != null)
        {
            exteriorPanel.SetActive(true);
        }

        if (GuideManager.Instance != null)
        {
            GuideManager.Instance.ShowGuide(
                GuideEventType.SafeOutside
            );
        }
    }

    private void HideAllPanels()
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }

        if (duringPanel != null)
        {
            duringPanel.SetActive(false);
        }

        if (afterPanel != null)
        {
            afterPanel.SetActive(false);
        }

        if (safeToMovePanel != null)
        {
            safeToMovePanel.SetActive(false);
        }

        if (exteriorPanel != null)
        {
            exteriorPanel.SetActive(false);
        }
    }
}