
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TermsAndConditionsManager : MonoBehaviour
{
    [Header("Checkbox Button")]
    [SerializeField] private Button confirmButton;

    [Tooltip("Sprite displayed when the checkbox is unchecked.")]
    [SerializeField] private Sprite uncheckedSprite;

    [Tooltip("Sprite displayed when the checkbox is checked.")]
    [SerializeField] private Sprite checkedSprite;

    [Header("Action Buttons")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    [Header("Scene Navigation")]
    [SerializeField] private string nextSceneName = "Main Menu";

    private Image checkboxImage;
    private bool hasAgreed;

    private void Awake()
    {
        if (confirmButton != null)
        {
            checkboxImage = confirmButton.GetComponent<Image>();
            confirmButton.onClick.AddListener(ToggleAgreement);
        }

        if (acceptButton != null)
            acceptButton.onClick.AddListener(AcceptTerms);

        if (declineButton != null)
            declineButton.onClick.AddListener(DeclineTerms);

        // Require the player to check the box each time
        // this Terms and Conditions screen opens.
        hasAgreed = false;

        UpdateConsentUI();
    }

    private void UpdateConsentUI()
    {
        // Change the Confirm button's own image.
        if (checkboxImage != null)
        {
            Sprite targetSprite = hasAgreed
                ? checkedSprite
                : uncheckedSprite;

            if (targetSprite != null)
                checkboxImage.sprite = targetSprite;
        }

        // Accept is only interactable after agreeing.
        if (acceptButton != null)
            acceptButton.interactable = hasAgreed;
    }

    public void ToggleAgreement()
    {
        hasAgreed = !hasAgreed;

        UpdateConsentUI();

        Debug.Log("Terms agreement: " + hasAgreed);
    }

    public void AcceptTerms()
    {
        if (!hasAgreed)
        {
            Debug.LogWarning(
                "Please check the agreement before accepting."
            );
            return;
        }

        PlayerPrefs.SetInt("QuakeHero_TermsAccepted", 1);
        PlayerPrefs.Save();

        LoadNextScene();
    }

    public void DeclineTerms()
    {
        PlayerPrefs.SetInt("QuakeHero_TermsAccepted", 0);
        PlayerPrefs.Save();

        Debug.Log("Terms declined.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError("Next scene name is empty.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError(
                "Cannot load scene '" + nextSceneName +
                "'. Check its name and Build Profiles."
            );
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(ToggleAgreement);

        if (acceptButton != null)
            acceptButton.onClick.RemoveListener(AcceptTerms);

        if (declineButton != null)
            declineButton.onClick.RemoveListener(DeclineTerms);
    }
}
