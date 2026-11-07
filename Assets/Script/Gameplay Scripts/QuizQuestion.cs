
using UnityEngine;
using UnityEngine.UI;

public class QuizQuestion : MonoBehaviour
{
    [System.Serializable]
    public class AnswerOption
    {
        public Button button;
        public bool isCorrect;
        public GameObject revealOverlay;
    }

    [Header("Answer Options")]
    [SerializeField] private AnswerOption[] options;

    private QuizManager quizManager;
    private bool answerSelected;

    public void Initialize(QuizManager manager)
    {
        quizManager = manager;
        answerSelected = false;

        SetupButtons();
        ResetFeedback();
    }

    private void SetupButtons()
    {
        if (options == null)
            return;

        for (int i = 0; i < options.Length; i++)
        {
            AnswerOption option = options[i];

            if (option == null || option.button == null)
                continue;

            option.button.onClick.RemoveListener(
                () => SelectAnswer(i)
            );

            int index = i;

            option.button.onClick.RemoveAllListeners();
            option.button.onClick.AddListener(
                () => SelectAnswer(index)
            );

            option.button.interactable = true;
        }
    }

    private void SelectAnswer(int index)
    {
        if (answerSelected)
            return;

        if (options == null || index < 0 || index >= options.Length)
            return;

        AnswerOption selectedOption = options[index];

        if (selectedOption == null)
            return;

        answerSelected = true;

        bool isCorrect = selectedOption.isCorrect;

        if (selectedOption.revealOverlay != null)
            selectedOption.revealOverlay.SetActive(true);

        if (!isCorrect)
            RevealCorrectAnswer();

        DisableButtons();

        if (quizManager != null)
            quizManager.RegisterAnswer(isCorrect);

        Debug.Log("QUIZ: Answer selected. Correct = " + isCorrect);
    }

    private void RevealCorrectAnswer()
    {
        if (options == null)
            return;

        foreach (AnswerOption option in options)
        {
            if (option == null)
                continue;

            if (option.isCorrect && option.revealOverlay != null)
                option.revealOverlay.SetActive(true);
        }
    }

    private void DisableButtons()
    {
        if (options == null)
            return;

        foreach (AnswerOption option in options)
        {
            if (option != null && option.button != null)
                option.button.interactable = false;
        }
    }

    private void ResetFeedback()
    {
        if (options == null)
            return;

        foreach (AnswerOption option in options)
        {
            if (option == null)
                continue;

            if (option.revealOverlay != null)
                option.revealOverlay.SetActive(false);

            if (option.button != null)
                option.button.interactable = true;
        }
    }
}
