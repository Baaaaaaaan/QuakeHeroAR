
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UsernameSetup : MonoBehaviour
{
    [Header("Username Input")]
    [SerializeField] private TMP_InputField usernameInput;

    [Header("UI Area to Move")]
    [SerializeField] private RectTransform inputAreaToMove;
    [SerializeField] private Canvas parentCanvas;

    [Header("Mobile Keyboard Settings")]
    [Tooltip("How far to move the input area upward, as a percentage of screen height.")]
    [Range(0.05f, 0.35f)]
    [SerializeField] private float liftScreenHeightPercent = 0.16f;

    [Tooltip("How quickly the input area moves.")]
    [SerializeField] private float movementSpeed = 12f;

    [Header("Scene Navigation")]
    [SerializeField] private string nextSceneName = "Character Selection";

    private Vector2 originalPosition;
    private float originalScreenHeight;

    private bool positionInitialized;
    private bool inputSelected;

    private void Awake()
    {
        // Find the Canvas automatically if not assigned.
        if (parentCanvas == null && usernameInput != null)
        {
            parentCanvas = usernameInput.GetComponentInParent<Canvas>();
        }

        // Find the Input Area automatically if not assigned.
        if (inputAreaToMove == null && usernameInput != null)
        {
            inputAreaToMove =
                usernameInput.transform.parent as RectTransform;
        }

        if (usernameInput != null)
        {
            // Keep the typed text visible in the Unity UI.
            usernameInput.shouldHideMobileInput = true;

            // Detect when the player selects the input field.
            usernameInput.onSelect.AddListener(OnInputSelected);

            // Detect when the player finishes editing.
            usernameInput.onEndEdit.AddListener(OnInputEnded);
        }
    }

    private void Start()
    {
        if (usernameInput == null)
        {
            Debug.LogError(
                "UsernameSetup: TMP Input Field is not assigned."
            );
            return;
        }

        if (inputAreaToMove == null)
        {
            Debug.LogError(
                "UsernameSetup: Input Area is not assigned."
            );
            return;
        }

        // Save the original screen position.
        Canvas.ForceUpdateCanvases();

        originalPosition = inputAreaToMove.anchoredPosition;
        originalScreenHeight = Screen.height;

        positionInitialized = true;

        Debug.Log("UsernameSetup: Ready.");
    }

    private void OnInputSelected(string value)
    {
        inputSelected = true;

        Debug.Log(
            "UsernameSetup: Input selected. Moving UI above keyboard."
        );
    }

    private void OnInputEnded(string value)
    {
        inputSelected = false;

        Debug.Log(
            "UsernameSetup: Editing finished. Restoring UI position."
        );
    }

    private void Update()
    {
        if (!positionInitialized ||
            inputAreaToMove == null ||
            usernameInput == null)
        {
            return;
        }

        // Use input focus instead of keyboard area detection.
        bool shouldMoveUp =
            inputSelected || usernameInput.isFocused;

        Vector2 targetPosition = originalPosition;

        if (shouldMoveUp)
        {
            float canvasScale = parentCanvas != null
                ? Mathf.Max(parentCanvas.scaleFactor, 0.01f)
                : 1f;

            // Calculate the amount to move using the original
            // screen height so Android keyboard resizing
            // does not change the movement distance.
            float liftAmount =
                originalScreenHeight *
                liftScreenHeightPercent /
                canvasScale;

            targetPosition.y = originalPosition.y + liftAmount;
        }

        // Smoothly move the Input Area.
        inputAreaToMove.anchoredPosition = Vector2.Lerp(
            inputAreaToMove.anchoredPosition,
            targetPosition,
            1f - Mathf.Exp(-movementSpeed * Time.unscaledDeltaTime)
        );
    }

    public void Continue()
    {
        if (usernameInput == null)
            return;

        string username = usernameInput.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning(
                "Please enter your username before continuing."
            );

            usernameInput.ActivateInputField();
            return;
        }

        // Hide the keyboard and restore the original position.
        inputSelected = false;
        usernameInput.DeactivateInputField();

        if (inputAreaToMove != null)
            inputAreaToMove.anchoredPosition = originalPosition;

        // Save the username.
        UsernameManager.SaveUsername(username);

        Debug.Log("Username saved: " + username);

        // Check the destination scene.
        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError(
                "UsernameSetup: Scene '" + nextSceneName +
                "' cannot be loaded. Check Build Profiles."
            );
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private void OnDisable()
    {
        inputSelected = false;

        if (positionInitialized && inputAreaToMove != null)
        {
            inputAreaToMove.anchoredPosition = originalPosition;
        }
    }

    private void OnDestroy()
    {
        if (usernameInput != null)
        {
            usernameInput.onSelect.RemoveListener(OnInputSelected);
            usernameInput.onEndEdit.RemoveListener(OnInputEnded);
        }
    }
}
