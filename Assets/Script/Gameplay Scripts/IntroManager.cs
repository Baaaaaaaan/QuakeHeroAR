using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomIntroManager : MonoBehaviour
{
    [Header("Intro UI")]
    [Tooltip("The intro panel GameObject (title image + blurred background + arrow button).")]
    [SerializeField] private GameObject introPanel;

    [Tooltip("The arrow button that dismisses the intro.")]
    [SerializeField] private Button continueButton;

    [Header("Gameplay Input Blocking")]
    [SerializeField] private CanvasGroup hazardsCanvasGroup;
    [SerializeField] private CanvasGroup notHazardCanvasGroup;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.4f;

    private CanvasGroup introCanvasGroup;

    private void Awake()
    {
        introCanvasGroup = introPanel.GetComponent<CanvasGroup>();
        if (introCanvasGroup == null)
        {
            introCanvasGroup = introPanel.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(DismissIntro);
        }

        ShowIntro();
    }

    private void ShowIntro()
    {
        introPanel.SetActive(true);
        introCanvasGroup.alpha = 1f;
        SetGameplayInteractable(false);
    }

    public void DismissIntro()
    {
        StartCoroutine(FadeOutIntro());
    }

    private IEnumerator FadeOutIntro()
    {
        if (continueButton != null) continueButton.interactable = false;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            introCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        introCanvasGroup.alpha = 0f;
        introPanel.SetActive(false);
        SetGameplayInteractable(true);

        if (GuideManager.Instance != null)
        {
            GuideManager.Instance.ShowGuide(GuideEventType.Intro);
        }
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