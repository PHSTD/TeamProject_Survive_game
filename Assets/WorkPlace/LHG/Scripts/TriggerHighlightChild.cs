using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TriggerHighlightChild : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image highlightImage;

    private Color highlightColor = new Color(0.5f, 1f, 0.9f, 0.5f); // 연한 민트
    private Color transparentColor;

    private Outline outline;

    private void Start()
    {
        highlightImage = transform.GetChild(0).GetComponent<Image>(); // 첫 번째 자식이 Highlight
        outline=highlightImage.GetComponent<Outline>();
        if(outline == null )
            outline = highlightImage.gameObject.AddComponent<Outline>();

        outline.effectColor= new Color(1f, 0f, 0f, 1f);
        outline.effectDistance = new Vector2(3f, -3f);
        outline.enabled = false;

        transparentColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0f);

        highlightImage.color = transparentColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightImage.color = highlightColor;
        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightImage.color = transparentColor;
        outline.enabled = false;
    }
}
