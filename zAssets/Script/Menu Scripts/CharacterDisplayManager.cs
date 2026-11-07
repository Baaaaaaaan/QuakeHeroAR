using UnityEngine;

public class CharacterDisplayManager : MonoBehaviour
{
    [Header("Character Visuals")]
    [SerializeField] private GameObject maleCharacter;
    [SerializeField] private GameObject femaleCharacter;

    private void OnEnable()
    {
        RefreshCharacter();
    }

    public void RefreshCharacter()
    {
        // Retrieve the character saved during selection.
        string selectedCharacter =
            CharacterSelectionManager.GetSelectedCharacter();

        // Display only the selected character.
        bool isMale = selectedCharacter == "Male";
        bool isFemale = selectedCharacter == "Female";

        if (maleCharacter != null)
            maleCharacter.SetActive(isMale);

        if (femaleCharacter != null)
            femaleCharacter.SetActive(isFemale);

        Debug.Log(
            "CharacterDisplayManager: Displaying " +
            selectedCharacter + " character."
        );

        if (maleCharacter == null || femaleCharacter == null)
        {
            Debug.LogWarning(
                "CharacterDisplayManager: Assign both character visuals " +
                "in the Inspector."
            );
        }
    }
}