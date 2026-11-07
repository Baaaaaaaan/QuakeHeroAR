using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Avatar : MonoBehaviour
{
    private const string CharacterKey = "SelectedCharacter";

    [Header("Character Models")]
    [SerializeField] private GameObject maleModel;
    [SerializeField] private GameObject femaleModel;

    [Header("Character Selection Buttons")]
    [SerializeField] private Button maleCharacterButton;
    [SerializeField] private Button femaleCharacterButton;

    [Header("Action Buttons")]
    [SerializeField] private Button saveAvatarButton;
    [SerializeField] private Button backButton;

    [Header("Selection Appearance")]
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float normalScale = 1f;

    [Header("Scene Navigation")]
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    private string currentSelection;

    private Vector3 maleOriginalScale;
    private Vector3 femaleOriginalScale;

    private void Awake()
    {
        // Connect character selection buttons.
        if (maleCharacterButton != null)
            maleCharacterButton.onClick.AddListener(SelectMale);

        if (femaleCharacterButton != null)
            femaleCharacterButton.onClick.AddListener(SelectFemale);

        // Connect action buttons.
        if (saveAvatarButton != null)
            saveAvatarButton.onClick.AddListener(SaveAvatar);

        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);
    }

    private void Start()
    {
        // Remember the original button sizes.
        if (maleCharacterButton != null)
            maleOriginalScale = maleCharacterButton.transform.localScale;

        if (femaleCharacterButton != null)
            femaleOriginalScale = femaleCharacterButton.transform.localScale;

        // Retrieve the previously saved character.
        currentSelection = PlayerPrefs.GetString(
            CharacterKey,
            "Male"
        );

        // Validate the saved selection.
        if (currentSelection != "Male" &&
            currentSelection != "Female")
        {
            currentSelection = "Male";
        }

        // Display the saved character.
        UpdateCharacterDisplay();

        Debug.Log(
            "Avatar: Loaded saved character: " + currentSelection
        );
    }

    public void SelectMale()
    {
        currentSelection = "Male";

        UpdateCharacterDisplay();

        Debug.Log("Avatar: Male character preview selected.");
    }

    public void SelectFemale()
    {
        currentSelection = "Female";

        UpdateCharacterDisplay();

        Debug.Log("Avatar: Female character preview selected.");
    }

    private void UpdateCharacterDisplay()
    {
        // Show only the currently selected character.
        if (maleModel != null)
            maleModel.SetActive(currentSelection == "Male");

        if (femaleModel != null)
            femaleModel.SetActive(currentSelection == "Female");

        // Highlight the selected character button.
        UpdateButtonScale();
    }

    private void UpdateButtonScale()
    {
        if (maleCharacterButton != null)
        {
            float scale = currentSelection == "Male"
                ? selectedScale
                : normalScale;

            maleCharacterButton.transform.localScale =
                maleOriginalScale * scale;
        }

        if (femaleCharacterButton != null)
        {
            float scale = currentSelection == "Female"
                ? selectedScale
                : normalScale;

            femaleCharacterButton.transform.localScale =
                femaleOriginalScale * scale;
        }
    }

    public void SaveAvatar()
    {
        if (string.IsNullOrEmpty(currentSelection))
        {
            Debug.LogWarning(
                "Avatar: Please select a character first."
            );
            return;
        }

        // Save the selected character.
        PlayerPrefs.SetString(
            CharacterKey,
            currentSelection
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Avatar: Character successfully saved: " +
            currentSelection
        );
    }

    public void BackToMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Avatar: Scene name is empty.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                "Avatar: Scene '" + sceneName +
                "' cannot be loaded. Check Build Profiles."
            );
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public static string GetSavedCharacter()
    {
        return PlayerPrefs.GetString(
            CharacterKey,
            "Male"
        );
    }

    private void OnDestroy()
    {
        if (maleCharacterButton != null)
            maleCharacterButton.onClick.RemoveListener(SelectMale);

        if (femaleCharacterButton != null)
            femaleCharacterButton.onClick.RemoveListener(SelectFemale);

        if (saveAvatarButton != null)
            saveAvatarButton.onClick.RemoveListener(SaveAvatar);

        if (backButton != null)
            backButton.onClick.RemoveListener(BackToMainMenu);
    }
}