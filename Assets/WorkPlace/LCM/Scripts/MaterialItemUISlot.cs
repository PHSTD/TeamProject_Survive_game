using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialItemUISlot : MonoBehaviour
{
    [SerializeField] public Image materialIcon; // �ν����Ϳ��� �Ҵ�
    [SerializeField] public TextMeshProUGUI materialName; // �ν����Ϳ��� �Ҵ�
    [SerializeField] public TextMeshProUGUI materialQuantity; // �ν����Ϳ��� �Ҵ�

    // ���߿� UI�� ������Ʈ�ϴ� �޼��带 �߰��� ���� �ֽ��ϴ�.
    public void SetUI(Sprite icon, string name, string quantityText, Color quantityColor)
    {
        if (materialIcon != null) materialIcon.sprite = icon;
        if (materialName != null) materialName.text = name;
        if (materialQuantity != null)
        {
            materialQuantity.text = quantityText;
            materialQuantity.color = quantityColor;
        }
    }
}
