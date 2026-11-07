using System.Collections;
using UnityEngine;

public class TakeCoverManager : MonoBehaviour
{
    [Header("Phase Switch")]

    [Tooltip("Every Hazard component in the room to disable when this phase starts.")]
    [SerializeField] private Hazard[] hazardComponentsToDisable;


    [Tooltip("Every SafeSpot component in the room to enable when this phase starts.")]
    [SerializeField] private SafeSpot[] safeSpotComponentsToEnable;


    [Header("Hazard-Phase UI to Hide")]

    [Tooltip("The 'Found: X/4' counter that is no longer needed.")]
    [SerializeField] private GameObject hazardCounterUI;


    [Header("Guide Timing")]

    [Tooltip("Delay between the 'Tap the safest place' message and the follow-up tip.")]
    [SerializeField] private float introToTipDelay = 2f;


    private bool phaseStarted = false;

    public bool PhaseActive => phaseStarted;


    public void StartTakeCoverPhase()
    {
        Debug.Log(
            $"StartTakeCoverPhase called. " +
            $"phaseStarted={phaseStarted}, " +
            $"hazards={hazardComponentsToDisable.Length}, " +
            $"safeSpots={safeSpotComponentsToEnable.Length}"
        );


        if (phaseStarted)
            return;


        phaseStarted = true;


        foreach (Hazard hazard in hazardComponentsToDisable)
        {
            if (hazard != null)
            {
                hazard.enabled = false;

                Debug.Log(
                    $"Disabled Hazard on {hazard.gameObject.name}"
                );
            }
            else
            {
                Debug.LogWarning(
                    "Null entry in hazardComponentsToDisable"
                );
            }
        }

        foreach (SafeSpot safeSpot in safeSpotComponentsToEnable)
        {
            if (safeSpot != null)
            {
                safeSpot.EnableInteraction();

                Debug.Log(
                    $"Enabled SafeSpot on {safeSpot.gameObject.name}"
                );
            }
            else
            {
                Debug.LogWarning(
                    "Null entry in safeSpotComponentsToEnable"
                );
            }
        }

        if (hazardCounterUI != null)
        {
            hazardCounterUI.SetActive(false);
        }

        if (GuideManager.Instance != null)
        {
            GuideManager.Instance.ShowGuide(
                GuideEventType.TakeCover
            );

            StartCoroutine(
                ShowTipAfterDelay()
            );
        }
    }

    private IEnumerator ShowTipAfterDelay()
    {
        yield return new WaitForSeconds(
            introToTipDelay
        );


        if (GuideManager.Instance != null)
        {
            GuideManager.Instance.ShowGuide(
                GuideEventType.TakeCoverTip
            );
        }
    }
}