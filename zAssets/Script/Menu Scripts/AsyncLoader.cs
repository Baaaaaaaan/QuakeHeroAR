
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class AsyncLoader : MonoBehaviour
{
    [Header("Loading UI")]
    [SerializeField] private Slider loadingSlider;

    [Header("Startup Scenes")]
    [SerializeField] private string firstTimeSceneName = "Username";
    [SerializeField] private string characterSelectionSceneName = "Character Selection";
    [SerializeField] private string returningPlayerSceneName = "Main Menu";

    [Header("Loading Animation")]
    [SerializeField] private float minimumLoadingTime = 10f;
    [SerializeField] private float completionDelay = 0.5f;

    private bool loadingStarted;

    private void Start()
    {
        Debug.Log("ASYNC LOADER: Started.");

        if (loadingStarted)
            return;

        if (loadingSlider == null)
        {
            Debug.LogError("ASYNC LOADER: Loading Slider is missing.");
            return;
        }

        loadingStarted = true;

        loadingSlider.minValue = 0f;
        loadingSlider.maxValue = 1f;
        loadingSlider.wholeNumbers = false;
        loadingSlider.interactable = false;
        loadingSlider.SetValueWithoutNotify(0f);

        // Ensure the first room in each environment is available.
        RoomProgressManager.UnlockStartingRooms();

        // Determine the correct startup scene.
        string targetScene = GetStartupScene();

        Debug.Log("ASYNC LOADER: Target scene = " + targetScene);

        StartCoroutine(LoadNextScene(targetScene));
    }

    private string GetStartupScene()
    {
        // First-time player: ask for a username.
        if (!UsernameManager.HasUsername())
        {
            return firstTimeSceneName;
        }

        // Existing players who completed character selection
        // should go directly to the Main Menu.
        bool onboardingComplete =
            PlayerPrefs.GetInt("QuakeHero_OnboardingComplete", 0) == 1
            || PlayerPrefs.HasKey("SelectedCharacter");

        // Player entered a username but hasn't completed setup yet.
        if (!onboardingComplete)
        {
            return characterSelectionSceneName;
        }

        // Returning player.
        return returningPlayerSceneName;
    }

    private IEnumerator LoadNextScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName) ||
            !Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                "ASYNC LOADER: Cannot load scene: " + sceneName
            );
            yield break;
        }

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(sceneName);

        if (operation == null)
        {
            Debug.LogError("ASYNC LOADER: Loading operation failed.");
            yield break;
        }

        operation.allowSceneActivation = false;

        float elapsedTime = 0f;
        float requiredTime = Mathf.Max(0.1f, minimumLoadingTime);

        while (elapsedTime < requiredTime ||
               operation.progress < 0.9f)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float timeProgress = Mathf.Clamp01(
                elapsedTime / requiredTime
            );

            float sceneProgress = Mathf.Clamp01(
                operation.progress / 0.9f
            );

            loadingSlider.SetValueWithoutNotify(
                Mathf.Min(timeProgress, sceneProgress)
            );

            yield return null;
        }

        loadingSlider.SetValueWithoutNotify(1f);

        Debug.Log("ASYNC LOADER: Loading complete.");

        yield return new WaitForSecondsRealtime(completionDelay);

        operation.allowSceneActivation = true;
    }
}
