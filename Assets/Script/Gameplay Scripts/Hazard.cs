using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to ANY clickable object in a hazard-hunt room (hazard or decoy).
/// Whether it's a real hazard is just a checkbox — no more duplicating
/// "Found" objects or writing separate scripts per item.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class Hazard : MonoBehaviour, IPointerClickHandler
{
    [Header("Hazard Setup")]
    [Tooltip("Check this if clicking this object should count as finding a hazard.")]
    public bool isHazard;

    [Tooltip("Should match this GameObject's name, and the hazardId in this room's RoomData asset, so HazardManager can auto-fill the fields below from data instead of you typing them here.")]
    public string hazardId;

    [Tooltip("Friendly name shown in the popup, e.g. 'Unsecured Vase'. Auto-filled from RoomData if set there.")]
    public string displayName;

    [TextArea]
    [Tooltip("Why this is an earthquake hazard, shown when found. Auto-filled from RoomData if set there.")]
    public string hazardExplanation;

    [Tooltip("This hazard's own hand-made info panel (e.g. VaseInfo, WallDecorInfo), shown when its found marker is tapped. Assign the matching panel directly here.")]
    public GameObject infoPanel;

    [TextArea]
    [Tooltip("A small nudge the guide says when the player taps Hint, WITHOUT giving the answer away (e.g. 'Look for something that could fall off a shelf'). Leave empty to skip this hazard when giving hints.")]
    public string hintText;

    [Header("Icon Prefabs")]
    [Tooltip("The red '!' icon prefab, spawned when this hazard is correctly found.")]
    public GameObject hazardFoundIconPrefab;

    [Tooltip("The red 'X' icon prefab, spawned briefly when the player clicks a non-hazard object.")]
    public GameObject wrongClickIconPrefab;

    [Tooltip("How long the wrong-click X icon stays on screen before disappearing.")]
    public float wrongIconLifetime = 0.75f;

    private bool alreadyFound = false;

    /// <summary>True once the player has correctly found this hazard.</summary>
    public bool IsFound => alreadyFound;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isHazard)
        {
            if (alreadyFound) return; // prevent double-counting the same hazard
            alreadyFound = true;

            GameObject foundIcon = SpawnIcon(hazardFoundIconPrefab, destroyAfter: -1f); // stays visible
            HazardManager.Instance.RegisterHazardFound(this);

            if (foundIcon != null)
            {
                var marker = foundIcon.GetComponent<FoundHazardMarker>();
                if (marker == null) marker = foundIcon.AddComponent<FoundHazardMarker>();
                marker.Setup(infoPanel);
            }

            // Stop this object from reacting to further clicks
            var graphic = GetComponent<UnityEngine.UI.Graphic>();
            if (graphic != null) graphic.raycastTarget = false;
        }
        else
        {
            SpawnIcon(wrongClickIconPrefab, wrongIconLifetime);

            if (GuideManager.Instance != null)
            {
                GuideManager.Instance.ShowGuide(GuideEventType.WrongClick);
            }
        }
    }

    private GameObject SpawnIcon(GameObject prefab, float destroyAfter)
    {
        if (prefab == null) return null;

        GameObject icon = Instantiate(prefab, transform.parent);
        icon.transform.position = transform.position;

        if (destroyAfter > 0f)
        {
            Destroy(icon, destroyAfter);
        }

        return icon;
    }
}