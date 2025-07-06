using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HighlightOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image highlightImage;
    private Outline outline;

    // 하이라이트 색 (연한 민트색 + 살짝 투명)
    private Color highlightColor = new Color(0.5f, 1f, 0.9f, 0.6f);
    private Color transparentColor;

    private void Awake()
    {
        highlightImage = GetComponent<Image>();
        transparentColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0f);

        // 기본 상태는 숨김
        highlightImage.color = transparentColor;

        // Outline 컴포넌트 자동 추가
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        // 기본 Outline 비활성화
        outline.enabled = false;
        outline.effectColor = Color.red; // 민트색 테두리
        outline.effectDistance = new Vector2(10f, -10f);

        Debug.LogError("******하이라이트온호버********");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightImage.color = highlightColor;
        outline.enabled = true;
        Debug.LogError("******온포인터엔터********");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightImage.color = transparentColor;
        outline.enabled = false;
        Debug.LogError("******온포인터exit********");
    }
}
