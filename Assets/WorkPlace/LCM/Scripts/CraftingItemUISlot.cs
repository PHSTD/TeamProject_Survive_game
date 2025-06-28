using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingItemUISlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public Image itemIcon;
    [SerializeField] public TextMeshProUGUI itemNameText;

    // �� ������ � �����Ǹ� ��Ÿ������ ������ ����
    private Recipe _assignedRecipe;
    // Start is called before the first frame update

    // UI�� �����ϰ� �����Ǹ� �Ҵ��ϴ� �޼���
    public void SetUI(Recipe recipeToAssign)
    {
        _assignedRecipe = recipeToAssign;

        if (recipeToAssign != null && recipeToAssign.craftedItem != null)
        {
            if (itemIcon != null && recipeToAssign.craftedItem.icon != null)
            {
                itemIcon.sprite = recipeToAssign.craftedItem.icon;
            }
            else if (itemIcon != null) // ������ ��������Ʈ�� ���� ��� �⺻�� ����
            {
                itemIcon.sprite = null; // �Ǵ� �⺻ ��������Ʈ �Ҵ�
            }

            if (itemNameText != null)
            {
                itemNameText.text = recipeToAssign.craftedItem.itemName;
            }
        }
        else
        {
            // ������ ������ ���� �� UI �ʱ�ȭ
            if (itemIcon != null) itemIcon.sprite = null;
            if (itemNameText != null) itemNameText.text = "";
        }
    }

    // Ŭ�� �̺�Ʈ ó�� (�� ������ Ŭ������ �� ȣ��� �Լ�)
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CraftingItemUISlot Ŭ����: " + _assignedRecipe?.craftedItem?.itemName);
        if (_assignedRecipe != null)
        {
            // CraftingManager�� ������ ���� �޼��带 ȣ��
            CraftingManager.Instance.SelectRecipe(_assignedRecipe);
        }
    }
}
