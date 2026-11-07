using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HintManager : MonoBehaviour
{
    [Header("Limits")]
    [Tooltip("Maximum hints per room. 0 = unlimited.")]
    [SerializeField] private int maxHints = 0;

    [Tooltip("What the guide says once the player has used all their hints.")]
    [SerializeField] private string noMoreHintsMessage = "No more hints! You can do it!";

    [Header("Optional UI")]
    [Tooltip("Optional label like \"Hints: 2\" showing hints remaining. Only used when Max Hints is above 0.")]
    [SerializeField] private TMP_Text hintsLeftText;

    private int hintsUsed = 0;
    private Hazard lastHinted;

    private void Start()
    {
        UpdateHintsLeftUI();
    }

    public void ShowHint()
    {
        // Don't give hints while the game is paused
        if (Time.timeScale == 0f) return;

        if (GuideManager.Instance == null) return;

        if (maxHints > 0 && hintsUsed >= maxHints)
        {
            GuideManager.Instance.ShowCustomGuide(GuideEventType.Hint, noMoreHintsMessage);
            return;
        }

        List<Hazard> candidates = new List<Hazard>();
        foreach (var hazard in FindObjectsOfType<Hazard>(true))
        {
            if (hazard.isHazard && !hazard.IsFound && !string.IsNullOrEmpty(hazard.hintText))
            {
                candidates.Add(hazard);
            }
        }

        if (candidates.Count == 0) return; // everything found, or no hints written yet

        // Avoid giving the same hint twice in a row if there's another choice
        if (candidates.Count > 1 && lastHinted != null)
        {
            candidates.Remove(lastHinted);
        }

        Hazard chosen = candidates[Random.Range(0, candidates.Count)];
        lastHinted = chosen;

        GuideManager.Instance.ShowCustomGuide(GuideEventType.Hint, chosen.hintText);

        hintsUsed++;
        UpdateHintsLeftUI();
    }

    private void UpdateHintsLeftUI()
    {
        if (hintsLeftText == null) return;

        if (maxHints > 0)
        {
            hintsLeftText.text = $"Hints: {Mathf.Max(0, maxHints - hintsUsed)}";
        }
        else
        {
            hintsLeftText.text = "";
        }
    }
}