
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private GameObject quizIntroPanel;
    [SerializeField] private GameObject exteriorPanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject passPanel;
    [SerializeField] private GameObject failPanel;

    [Header("Quiz Questions")]
    [SerializeField] private QuizQuestion[] questions;
    [SerializeField] private Button nextButton;

    [Header("Result Stars")]
    [Tooltip("Assign only the 1 Star, 2 Star, and 3 Star objects.")]
    [SerializeField] private GameObject[] passPanelStars;

    [Tooltip("Assign the No Star object from the Fail panel.")]
    [SerializeField] private GameObject[] failPanelStars;

    [Header("Pass Effects")]
    [SerializeField] private GameObject confettiEffect;

    [Header("Scoring")]
    [Tooltip("Minimum correct answers required to show Great Job.")]
    [SerializeField] private int passingScore = 1;

    [Header("Room Progression")]
    [SerializeField] private RoomData currentRoomData;
    [SerializeField] private string roomSelectionSceneName = "RoomSelectionHouse";

    private QuizQuestion[] randomizedQuestions;
    private int currentQuestionIndex;
    private int correctAnswers;

    private bool quizStarted;
    private bool answerSubmitted;

    private void Awake()
    {
        SetupQuiz();
    }

    private void SetupQuiz()
    {
        quizStarted = false;
        currentQuestionIndex = 0;
        correctAnswers = 0;
        answerSubmitted = false;
        randomizedQuestions = null;

        HideAllQuestions();
        HideResults();

        if (quizIntroPanel != null)
            quizIntroPanel.SetActive(false);

        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        if (confettiEffect != null)
            confettiEffect.SetActive(false);

        if (quizPanel != null)
            quizPanel.SetActive(false);
    }

    public void StartQuizFromExterior()
    {
        Debug.Log("QuizManager: Exterior Continue pressed.");

        if (exteriorPanel != null)
            exteriorPanel.SetActive(false);

        if (quizPanel != null)
            quizPanel.SetActive(true);

        StartQuiz();
    }

    public void StartQuiz()
    {
        if (questions == null || questions.Length == 0)
        {
            Debug.LogError("QuizManager: No questions assigned.");
            return;
        }

        for (int i = 0; i < questions.Length; i++)
        {
            if (questions[i] == null)
            {
                Debug.LogError(
                    "QuizManager: Question " + (i + 1) +
                    " is not assigned."
                );
                return;
            }
        }

        quizStarted = true;
        currentQuestionIndex = 0;
        correctAnswers = 0;
        answerSubmitted = false;

        RandomizeQuestions();
        HideAllQuestions();
        HideResults();

        if (confettiEffect != null)
            confettiEffect.SetActive(false);

        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        if (quizPanel != null)
            quizPanel.SetActive(true);

        if (quizIntroPanel != null)
            quizIntroPanel.SetActive(true);
        else
            ContinueQuiz();

        Debug.Log("QuizManager: Quiz started.");
    }

    public void ContinueQuiz()
    {
        if (!quizStarted)
        {
            Debug.LogWarning(
                "QuizManager: StartQuiz must be called before ContinueQuiz."
            );
            return;
        }

        if (quizIntroPanel != null)
            quizIntroPanel.SetActive(false);

        currentQuestionIndex = 0;
        ShowQuestion();
    }

    private void RandomizeQuestions()
    {
        randomizedQuestions = (QuizQuestion[])questions.Clone();

        for (int i = randomizedQuestions.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            QuizQuestion temp = randomizedQuestions[i];
            randomizedQuestions[i] = randomizedQuestions[randomIndex];
            randomizedQuestions[randomIndex] = temp;
        }
    }

    private void ShowQuestion()
    {
        HideAllQuestions();
        answerSubmitted = false;

        if (randomizedQuestions == null ||
            currentQuestionIndex < 0 ||
            currentQuestionIndex >= randomizedQuestions.Length)
        {
            FinishQuiz();
            return;
        }

        QuizQuestion question = randomizedQuestions[currentQuestionIndex];

        if (question == null)
        {
            Debug.LogError(
                "QuizManager: Question is missing at index " +
                currentQuestionIndex
            );

            FinishQuiz();
            return;
        }

        question.gameObject.SetActive(true);
        question.Initialize(this);

        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        Debug.Log(
            "QuizManager: Showing question " +
            (currentQuestionIndex + 1) + " of " +
            randomizedQuestions.Length
        );
    }

    private void HideAllQuestions()
    {
        if (questions == null)
            return;

        foreach (QuizQuestion question in questions)
        {
            if (question != null)
                question.gameObject.SetActive(false);
        }

        if (randomizedQuestions != null)
        {
            foreach (QuizQuestion question in randomizedQuestions)
            {
                if (question != null)
                    question.gameObject.SetActive(false);
            }
        }
    }

    public void RegisterAnswer(bool isCorrect)
    {
        if (!quizStarted || answerSubmitted)
            return;

        answerSubmitted = true;

        if (isCorrect)
            correctAnswers++;

        if (nextButton != null)
            nextButton.gameObject.SetActive(true);

        Debug.Log(
            "QuizManager: Answer registered. Correct = " +
            isCorrect + ", Score = " + correctAnswers
        );
    }

    public void NextQuestion()
    {
        if (!quizStarted)
            return;

        if (!answerSubmitted)
        {
            Debug.LogWarning(
                "QuizManager: Select an answer before continuing."
            );
            return;
        }

        currentQuestionIndex++;

        if (randomizedQuestions == null ||
            currentQuestionIndex >= randomizedQuestions.Length)
        {
            FinishQuiz();
            return;
        }

        ShowQuestion();
    }

    private void FinishQuiz()
    {
        quizStarted = false;
        answerSubmitted = false;

        HideAllQuestions();

        if (quizIntroPanel != null)
            quizIntroPanel.SetActive(false);

        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        HideResults();

        if (quizPanel != null)
            quizPanel.SetActive(true);

        if (resultPanel != null)
            resultPanel.SetActive(true);

        int totalQuestions = randomizedQuestions != null
            ? randomizedQuestions.Length
            : 0;

        // Zero correct answers fails.
        // Reaching the passing score shows Great Job.
        bool passed = totalQuestions > 0 &&
                      correctAnswers >= passingScore;

        // Save room progress and unlock the next room if passed.
        string nextRoomScene = currentRoomData != null
            ? currentRoomData.nextSceneName
            : "";

        RoomProgressManager.RecordRoomResult(
            SceneManager.GetActiveScene().name,
            correctAnswers,
            nextRoomScene,
            passed
        );

        if (passPanel != null)
            passPanel.SetActive(passed);

        if (failPanel != null)
            failPanel.SetActive(!passed);

        if (confettiEffect != null)
            confettiEffect.SetActive(passed);

        UpdateStars(correctAnswers, passed);

        Debug.Log(
            "QuizManager: Quiz finished. Score = " +
            correctAnswers + "/" + totalQuestions +
            ", Passed = " + passed
        );
    }

    private void UpdateStars(int score, bool passed)
    {
        // PassPanelStars must contain only the three awarded-star objects.
        int availableStars = passPanelStars != null
            ? passPanelStars.Length
            : 0;

        int starsToShow = passed
            ? Mathf.Clamp(score, 0, availableStars)
            : 0;

        UpdateStarArray(passPanelStars, starsToShow);

        // FailPanelStars should contain the No Star image.
        // Show it on failure and hide it on success.
        if (failPanelStars != null)
        {
            foreach (GameObject star in failPanelStars)
            {
                if (star != null)
                    star.SetActive(!passed);
            }
        }

        Debug.Log(
            "QuizManager: Updating stars. Score = " +
            score + ", Passed = " + passed +
            ", Stars shown = " + starsToShow
        );
    }

    private void UpdateStarArray(GameObject[] stars, int count)
    {
        if (stars == null)
            return;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].SetActive(i < count);
        }
    }

    private void HideResults()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (passPanel != null)
            passPanel.SetActive(false);

        if (failPanel != null)
            failPanel.SetActive(false);

        HideStarArray(passPanelStars);
        HideStarArray(failPanelStars);
    }

    private void HideStarArray(GameObject[] stars)
    {
        if (stars == null)
            return;

        foreach (GameObject star in stars)
        {
            if (star != null)
                star.SetActive(false);
        }
    }

    public void RestartQuiz()
    {
        if (quizPanel != null)
            quizPanel.SetActive(true);

        if (exteriorPanel != null)
            exteriorPanel.SetActive(false);

        StartQuiz();
    }

    public void ReturnToRoomSelection()
    {
        if (string.IsNullOrWhiteSpace(roomSelectionSceneName))
        {
            Debug.LogError(
                "QuizManager: Room Selection scene name is empty."
            );
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(roomSelectionSceneName))
        {
            Debug.LogError(
                "QuizManager: Scene '" + roomSelectionSceneName +
                "' is not available. Check its name and Build Settings."
            );
            return;
        }

        SceneManager.LoadScene(roomSelectionSceneName);
    }

    public void ContinueToNextRoom()
    {
        if (currentRoomData == null)
        {
            Debug.LogError(
                "QuizManager: Assign the current RoomData asset."
            );
            return;
        }

        string nextScene = currentRoomData.nextSceneName;

        if (string.IsNullOrWhiteSpace(nextScene))
        {
            Debug.LogError(
                "QuizManager: The next scene name is empty in RoomData."
            );
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextScene))
        {
            Debug.LogError(
                "QuizManager: Scene '" + nextScene +
                "' is not available. Check its name and Build Settings."
            );
            return;
        }

        SceneManager.LoadScene(nextScene);
    }

    public int GetScore()
    {
        return correctAnswers;
    }

    public int GetQuestionCount()
    {
        return randomizedQuestions != null
            ? randomizedQuestions.Length
            : 0;
    }
}
