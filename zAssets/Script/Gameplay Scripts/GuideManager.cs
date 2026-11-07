using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The types of moments the guide character reacts to. Add more here
/// as you add more phases (e.g. TakeCover for a future drop-cover-hold mechanic).
/// </summary>
public enum GuideEventType
{
    Intro,
    WrongClick,
    AllFound,
    ExplainIcon,
    TakeCover,          // "Tap the safest place to take cover"
    TakeCoverTip,        // "Tip: Under a sturdy table is a safe place"
    TakeCoverWrongClick, // reaction shown when tapping an unsafe spot
    EarthquakeHold,      // "Stay calm and hold on" (during shaking)
    FollowExit,          // "Follow the arrow and exit safely"
    SafeOutside,         // "You are safe now! Stay calm and remain in the safe area"
    Hint                 // pose/duration used when the Hint button shows a per-hazard hint
}

/// <summary>
/// One guide message + matching mascot pose GameObject. Edit these in the
/// Inspector on GuideManager instead of touching code to change wording or art.
/// </summary>
[System.Serializable]
public class GuideState
{
    public GuideEventType eventType;

    [TextArea]
    public string message;

    [Tooltip("How long this message stays visible before auto-hiding. Leave at 0 to use GuideManager's Default Display Duration instead.")]
    public float displayDuration = 0f;

    [Tooltip("The mascot pose GameObject for this message (e.g. 'Sprite guide 1'). GuideManager will activate this one and deactivate all others in All Mascot Poses.")]
    public GameObject mascotPoseObject;
}

/// <summary>
/// Shows a reactive guide bubble + mascot character that responds to
/// gameplay events (intro, wrong click, all hazards found, etc).
/// Hooked into by RoomIntroManager, Hazard, and HazardManager.
/// </summary>
public class GuideManager : MonoBehaviour
{
    public static GuideManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject guidePanel;
    [SerializeField] private TMP_Text messageText;

    [Tooltip("Every mascot pose GameObject that exists (e.g. Sprite guide 1, 2, 3). GuideManager deactivates all of these, then activates whichever one the current GuideState points to.")]
    [SerializeField] private List<GameObject> allMascotPoses = new List<GameObject>();

    [Header("Guide Content")]
    [Tooltip("One entry per GuideEventType you want to react to.")]
    [SerializeField] private List<GuideState> guideStates = new List<GuideState>();

    [Header("Timing")]
    [Tooltip("Characters typed per second for the typing animation.")]
    [SerializeField] private float typingSpeed = 30f;

    [Tooltip("Default time each guide message stays visible before auto-hiding, if that GuideState's own Display Duration is left at 0.")]
    [SerializeField] private float defaultDisplayDuration = 2.5f;

    [Tooltip("How long to wait after showing 'All Found' before auto-advancing to the 'Explain Icon' hint.")]
    [SerializeField] private float allFoundToExplainDelay = 1.5f;

    private Dictionary<GuideEventType, GuideState> lookup;
    private Coroutine showRoutine;

    private void Awake()
    {
        Instance = this;

        lookup = new Dictionary<GuideEventType, GuideState>();
        foreach (var state in guideStates)
        {
            if (!lookup.ContainsKey(state.eventType))
            {
                lookup.Add(state.eventType, state);
            }
        }
    }

    /// <summary>
    /// Shows the guide message + mascot pose matching this event type.
    /// Safe to call even if that event type has no entry set up yet.
    /// </summary>
    public void ShowGuide(GuideEventType type)
    {
        if (!lookup.TryGetValue(type, out GuideState state))
        {
            Debug.LogWarning($"GuideManager: no GuideState set up for {type}");
            return;
        }

        DisplayMessage(state.message, state.mascotPoseObject, state.displayDuration);
    }

    /// <summary>
    /// Shows dynamic text (e.g. a per-hazard hint) instead of the fixed message,
    /// borrowing the mascot pose and display duration from poseSource's GuideState.
    /// If that state isn't set up, the text still shows with the default duration
    /// and the mascot's current pose.
    /// </summary>
    public void ShowCustomGuide(GuideEventType poseSource, string message)
    {
        if (lookup.TryGetValue(poseSource, out GuideState state))
        {
            DisplayMessage(message, state.mascotPoseObject, state.displayDuration);
        }
        else
        {
            Debug.LogWarning($"GuideManager: no GuideState set up for {poseSource}, showing text without changing the pose.");
            DisplayMessage(message, null, 0f);
        }
    }

    private void DisplayMessage(string message, GameObject poseObject, float displayDuration)
    {
        if (guidePanel != null) guidePanel.SetActive(true);

        // Hide every mascot pose, then show only the requested one (if any)
        if (poseObject != null)
        {
            foreach (var pose in allMascotPoses)
            {
                if (pose != null) pose.SetActive(false);
            }
            poseObject.SetActive(true);
        }

        // Restart the type-then-hold-then-hide sequence for this message
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
        }
        float duration = displayDuration > 0f ? displayDuration : defaultDisplayDuration;
        showRoutine = StartCoroutine(ShowSequence(message, duration));
    }

    /// <summary>
    /// Types the message out character by character, then holds it on screen
    /// for displayDuration before auto-hiding. The hold timer only starts
    /// AFTER typing finishes, so short display durations won't cut off long messages.
    /// </summary>
    private IEnumerator ShowSequence(string fullText, float displayDuration)
    {
        if (messageText != null)
        {
            messageText.text = "";
            float charDelay = 1f / Mathf.Max(typingSpeed, 0.01f);

            foreach (char c in fullText)
            {
                messageText.text += c;
                yield return new WaitForSeconds(charDelay);
            }
        }

        yield return new WaitForSeconds(displayDuration);

        if (guidePanel != null) guidePanel.SetActive(false);
        showRoutine = null;
    }

    public void HideGuide()
    {
        if (guidePanel != null) guidePanel.SetActive(false);

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }
    }

    /// <summary>
    /// Hook this to HazardManager's onAllHazardsFound UnityEvent in the Inspector.
    /// Shows the "great job" message, then auto-advances to the "click the icon"
    /// hint after a short delay.
    /// </summary>
    public void HandleAllHazardsFound()
    {
        ShowGuide(GuideEventType.AllFound);
        StartCoroutine(ShowExplainIconAfterDelay());
    }

    private IEnumerator ShowExplainIconAfterDelay()
    {
        yield return new WaitForSeconds(allFoundToExplainDelay);
        ShowGuide(GuideEventType.ExplainIcon);
    }
}