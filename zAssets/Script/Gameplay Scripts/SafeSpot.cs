using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SafeSpot : MonoBehaviour, IPointerClickHandler
{
    [Header("Safe Spot Setup")]
    [Tooltip("Check this if tapping this object is the correct safe spot.")]
    public bool isSafe;

    [Header("Icon Prefabs")]
    public GameObject correctIconPrefab;
    public GameObject wrongIconPrefab;

    public float wrongIconLifetime = 0.75f;

    [Header("Confirmation Card")]
    public GameObject confirmationPanel;

    [Header("Earthquake Sequence")]
    [Tooltip("EarthquakeSequenceManager that should be notified when this safe spot is correctly selected.")]
    public EarthquakeSequenceManager earthquakeSequenceManager;

    private bool checkmarkSpawned = false;
    private GameObject spawnedCheckmark;


    private void Awake()
    {
        // Disabled by default.
        // The EarthquakeSequenceManager will enable this during the warning phase.
        enabled = false;
    }


    // =========================================================
    // ENABLE / DISABLE
    // =========================================================

    public void EnableInteraction()
    {
        enabled = true;
    }


    public void DisableInteraction()
    {
        enabled = false;
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetFeedback()
    {
        checkmarkSpawned = false;

        if (spawnedCheckmark != null)
        {
            Destroy(spawnedCheckmark);
            spawnedCheckmark = null;
        }

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }


    // =========================================================
    // CLICK
    // =========================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!enabled)
            return;


        // =====================================================
        // CORRECT SAFE SPOT
        // =====================================================

        if (isSafe)
        {
            Debug.Log("Correct safe spot selected: " + gameObject.name);


            // Show green checkmark only once.
            if (!checkmarkSpawned)
            {
                checkmarkSpawned = true;

                spawnedCheckmark = SpawnIcon(
                    correctIconPrefab,
                    -1f
                );
            }


            // Show confirmation panel if assigned.
            if (confirmationPanel != null)
            {
                confirmationPanel.SetActive(true);
            }


            // Tell EarthquakeSequenceManager.
            if (earthquakeSequenceManager != null)
            {
                earthquakeSequenceManager.OnCorrectSafeSpotSelected();
            }


            return;
        }


        // =====================================================
        // WRONG SAFE SPOT
        // =====================================================

        SpawnIcon(
            wrongIconPrefab,
            wrongIconLifetime
        );


        if (GuideManager.Instance != null)
        {
            GuideManager.Instance.ShowGuide(
                GuideEventType.TakeCoverWrongClick
            );
        }
    }


    // =========================================================
    // SPAWN ICON
    // =========================================================

    private GameObject SpawnIcon(
        GameObject prefab,
        float destroyAfter
    )
    {
        if (prefab == null)
        {
            Debug.LogWarning(
                "SafeSpot: Icon prefab is missing on " +
                gameObject.name
            );

            return null;
        }


        GameObject icon = Instantiate(
            prefab,
            transform.parent
        );


        icon.transform.position = transform.position;


        if (destroyAfter > 0f)
        {
            Destroy(icon, destroyAfter);
        }


        return icon;
    }
}