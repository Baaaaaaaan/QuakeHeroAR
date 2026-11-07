using UnityEngine;
using TMPro;

public class ChangeName : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private GameObject editNamePanel;
    [SerializeField] private TMP_InputField nameInput;

    private void Start()
    {
        LoadPlayerName();

        editNamePanel.SetActive(false);
    }

    private void LoadPlayerName()
    {
        playerNameText.text = UsernameManager.GetUsername();
    }

    public void OpenEditName()
    {
        nameInput.text = UsernameManager.GetUsername();
        editNamePanel.SetActive(true);
    }

    public void SaveName()
    {
        string newName = nameInput.text.Trim();

        if (string.IsNullOrEmpty(newName))
            return;

        UsernameManager.SaveUsername(newName);

        playerNameText.text = newName;

        editNamePanel.SetActive(false);
    }

    public void CancelEdit()
    {
        editNamePanel.SetActive(false);
    }
}