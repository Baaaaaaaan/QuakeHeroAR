using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomCarousel : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;

    [Header("Room Cards")]
    [SerializeField] private RectTransform[] roomCards;

    [Header("Settings")]
    [SerializeField] private float selectedScale = 1.15f;
    [SerializeField] private float normalScale = 0.85f;
    [SerializeField] private float snapSpeed = 8f;
    [SerializeField] private float swipeThreshold = 50f;

    private int currentIndex = 0;
    private bool isDragging;
    private float targetX;

    private void Start()
    {
        if (roomCards.Length == 0)
            return;

        Canvas.ForceUpdateCanvases();

        SnapToRoom(currentIndex, true);
    }

    private void Update()
    {
        if (roomCards.Length == 0)
            return;

        if (!isDragging)
        {
            Vector2 position = content.anchoredPosition;

            position.x = Mathf.Lerp(
                position.x,
                targetX,
                Time.deltaTime * snapSpeed
            );

            content.anchoredPosition = position;
        }

        UpdateCardScales();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        float dragDistance = eventData.position.x -
                             eventData.pressPosition.x;

        if (Mathf.Abs(dragDistance) >= swipeThreshold)
        {
            if (dragDistance < 0)
                currentIndex++;
            else
                currentIndex--;

            currentIndex = Mathf.Clamp(
                currentIndex,
                0,
                roomCards.Length - 1
            );
        }

        SnapToRoom(currentIndex, false);
    }

    private void SnapToRoom(int index, bool instant)
    {
        currentIndex = Mathf.Clamp(
            index,
            0,
            roomCards.Length - 1
        );

        RectTransform card = roomCards[currentIndex];

        float viewportCenter = viewport.rect.width / 2f;

        targetX = viewportCenter - card.localPosition.x;

        if (instant)
        {
            Vector2 position = content.anchoredPosition;
            position.x = targetX;
            content.anchoredPosition = position;
        }
    }

    private void UpdateCardScales()
    {
        for (int i = 0; i < roomCards.Length; i++)
        {
            float targetScale =
                i == currentIndex
                ? selectedScale
                : normalScale;

            float newScale = Mathf.Lerp(
                roomCards[i].localScale.x,
                targetScale,
                Time.deltaTime * snapSpeed
            );

            roomCards[i].localScale =
                Vector3.one * newScale;
        }
    }
}