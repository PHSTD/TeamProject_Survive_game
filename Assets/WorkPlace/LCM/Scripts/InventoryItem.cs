using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    Image itemIcon;
    public CanvasGroup canvasGroup { get; private set; }

    public Item myItem { get; set; }
    public InventorySlot activeSlot { get; set; }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemIcon = GetComponent<Image>();

        if (canvasGroup == null) Debug.LogWarning("InventoryItem: CanvasGroup�� �����ϴ�! " + gameObject.name);
        if (itemIcon == null) Debug.LogWarning("InventoryItem: Image ������Ʈ�� �����ϴ�! " + gameObject.name);
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        if (itemIcon == null) // <-- �� �κ��� �ٽ��Դϴ�.
        {
            itemIcon = GetComponent<Image>();
        }

        if (itemIcon == null)
        {
            Debug.LogError("InventoryItem: Initialize���� Image ������Ʈ�� ã�� �� �����ϴ�. ������ ������ Ȯ���ϼ���.");
            return; // �� �̻� �������� ����
        }

        activeSlot = parent;
        activeSlot.myItem = this;
        myItem = item;
        itemIcon.sprite = item.sprite;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1; // ���̰�
            canvasGroup.blocksRaycasts = true; // ����ĳ��Ʈ ��� (�巡�� �����ϵ���)
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Instance.SetCarriedItem(this);
        }
    }
}
