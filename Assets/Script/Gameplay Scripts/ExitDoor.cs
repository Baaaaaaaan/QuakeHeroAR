using UnityEngine;
using UnityEngine.EventSystems;

public class DoorExit : MonoBehaviour, IPointerClickHandler
{
    private EarthquakeSequenceManager sequenceManager;

    private void Awake()
    {
        enabled = false;
    }

    public void Setup(EarthquakeSequenceManager manager)
    {
        sequenceManager = manager;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sequenceManager != null)
        {
            sequenceManager.OnDoorTapped();
        }
    }
}