using UnityEngine;
using UnityEngine.UI;

public class HazardInfoPopup : MonoBehaviour
{
    public static HazardInfoPopup Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;

    public Image iconImage;
    public Text titleText;
    public Text explanationText;
    public Button closeButton;

    [Header("Positioning")]
    public Vector2 offset = new Vector2(0f, -40f);

    private RectTransform panelRect;
    private RectTransform canvasRect;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
            panelRect = panel.GetComponent<RectTransform>();
            canvasRect = panelRect != null ? panelRect.root as RectTransform : null;
        }

        if (closeButton != null) closeButton.onClick.AddListener(Hide);
    }

    public void Show(string title, string explanation, Sprite icon, Vector3 worldPosition)
    {
        if (panel == null) return;

        titleText.text = string.IsNullOrEmpty(title) ? "Hazard" : title;
        explanationText.text = string.IsNullOrEmpty(explanation)
            ? "This item could be dangerous during an earthquake."
            : explanation;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }

        panel.SetActive(true);
        PositionNear(worldPosition);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    private void PositionNear(Vector3 worldPosition)
    {
        if (panelRect == null || canvasRect == null) return;

        panelRect.position = (Vector3)((Vector2)worldPosition + offset);

        Canvas.ForceUpdateCanvases();

        Vector3[] panelCorners = new Vector3[4];
        panelRect.GetWorldCorners(panelCorners);

        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        Vector3 correction = Vector3.zero;

        if (panelCorners[0].x < canvasCorners[0].x) correction.x += canvasCorners[0].x - panelCorners[0].x;
        if (panelCorners[2].x > canvasCorners[2].x) correction.x -= panelCorners[2].x - canvasCorners[2].x;
        if (panelCorners[0].y < canvasCorners[0].y) correction.y += canvasCorners[0].y - panelCorners[0].y;
        if (panelCorners[2].y > canvasCorners[2].y) correction.y -= panelCorners[2].y - canvasCorners[2].y;

        panelRect.position += correction;
    }
}