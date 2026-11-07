using UnityEngine;
using UnityEngine.EventSystems;

public class FoundHazardMarker : MonoBehaviour, IPointerClickHandler
{
    private GameObject infoPanel;

    private static GameObject currentlyOpenPanel;

    public void Setup(GameObject infoPanel)
    {
        this.infoPanel = infoPanel;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (infoPanel == null) return;

        bool wasOpen = infoPanel.activeSelf;

        if (currentlyOpenPanel != null && currentlyOpenPanel != infoPanel)
        {
            currentlyOpenPanel.SetActive(false);
        }

        infoPanel.SetActive(!wasOpen);
        currentlyOpenPanel = infoPanel.activeSelf ? infoPanel : null;
    }
}