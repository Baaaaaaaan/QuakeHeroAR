
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Character Buttons")]
    [SerializeField] private Button maleButton;
    [SerializeField] private Button femaleButton;

    [Header("Navigation Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button selectButton;

    [Header("Selection Appearance")]
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float normalScale = 1f;

    [Header("Scene Navigation")]
    [SerializeField] private string backSceneName = "Main Menu";
    [SerializeField] private string nextSceneName = "Main Menu";

    private const string CharacterKey = "SelectedCharacter";

    private string selectedCharacter = "Male";

    private Vector3 maleOriginalScale;
    private Vector3 femaleOriginalScale;

    private void Awake()
    {
        // Register button events.
        if (maleButton != null)
            maleButton.onClick.AddListener(SelectMale);

        if (femaleButton != null)
            femaleButton.onClick.AddListener(SelectFemale);

        if (backButton != null)
            backButton.onClick.AddListener(GoBack);

        if (selectButton != null)
            selectButton.onClick.AddListener(ConfirmSelection);
    }

    private void Start()
    {
        // Remember the original character card sizes.
        if (maleButton != null)
            maleOriginalScale = maleButton.transform.localScale;

        if (femaleButton != null)
            femaleOriginalScale = femaleButton.transform.localScale;

        // Load the previously saved character.
        selectedCharacter = PlayerPrefs.GetString(
            CharacterKey,
            "Male"
        );

        // Validate saved selection.
        if (selectedCharacter != "Male" &&
            selectedCharacter != "Female")
        {
            selectedCharacter = "Male";
        }

        UpdateSelectionAppearance();

        Debug.Log(
            "Character Selection: Current character is " +
            selectedCharacter
        );
    }

    public void SelectMale()
    {
        selectedCharacter = "Male";

        UpdateSelectionAppearance();

        Debug.Log("Male character selected.");
    }

    public void SelectFemale()
    {
        selectedCharacter = "Female";

        UpdateSelectionAppearance();

        Debug.Log("Female character selected.");
    }

    private void UpdateSelectionAppearance()
    {
        // Enlarge the selected character card.
        // Return the other card to its original size.

        if (maleButton != null)
        {
            float scale = selectedCharacter == "Male"
                ? selectedScale
                : normalScale;

            maleButton.transform.localScale =
                maleOriginalScale * scale;
        }

        if (femaleButton != null)
        {
            float scale = selectedCharacter == "Female"
                ? selectedScale
                : normalScale;

            femaleButton.transform.localScale =
                femaleOriginalScale * scale;
        }
    }

    public void ConfirmSelection()
    {
        if (string.IsNullOrEmpty(selectedCharacter))
        {
            Debug.LogWarning(
                "Please select a character first."
            );
            return;
        }

        PlayerPrefs.SetString(
            CharacterKey,
            selectedCharacter
        );

        PlayerPrefs.SetInt("QuakeHero_OnboardingComplete", 1);
        PlayerPrefs.Save();

        Debug.Log(
            "Character saved: " + selectedCharacter
        );

        LoadScene(nextSceneName);
    }

    public void GoBack()
    {
        LoadScene(backSceneName);
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "CharacterSelectionManager: Scene name is empty."
            );
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                "CharacterSelectionManager: Scene '" +
                sceneName +
                "' cannot be loaded. Check Build Profiles."
            );
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    // Use this method in other scenes to retrieve the selection.
    public static string GetSelectedCharacter()
    {
        return PlayerPrefs.GetString(
            CharacterKey,
            "Male"
        );
    }

    private void OnDestroy()
    {
        if (maleButton != null)
            maleButton.onClick.RemoveListener(SelectMale);

        if (femaleButton != null)
            femaleButton.onClick.RemoveListener(SelectFemale);

        if (backButton != null)
            backButton.onClick.RemoveListener(GoBack);

        if (selectButton != null)
            selectButton.onClick.RemoveListener(ConfirmSelection);
    }
}
