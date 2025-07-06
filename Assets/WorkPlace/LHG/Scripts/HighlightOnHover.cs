using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HighlightOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image highlightImage;
    private Outline outline;

    // ���̶���Ʈ �� (���� ��Ʈ�� + ��¦ ����)
    private Color highlightColor = new Color(0.5f, 1f, 0.9f, 0.6f);
    private Color transparentColor;

    private void Awake()
    {
        highlightImage = GetComponent<Image>();
        transparentColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0f);

        // �⺻ ���´� ����
        highlightImage.color = transparentColor;

        // Outline ������Ʈ �ڵ� �߰�
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        // �⺻ Outline ��Ȱ��ȭ
        outline.enabled = false;
        outline.effectColor = Color.red; // ��Ʈ�� �׵θ�
        outline.effectDistance = new Vector2(10f, -10f);

        Debug.LogError("******���̶���Ʈ��ȣ��********");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightImage.color = highlightColor;
        outline.enabled = true;
        Debug.LogError("******�������Ϳ���********");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightImage.color = transparentColor;
        outline.enabled = false;
        Debug.LogError("******��������exit********");
    }
}
