using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public enum SlotTag { None, Head, Chest, Legs, Feet }

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Item myItemData { get; private set; }
    public InventoryItem myItemUI { get; set; }

    public SlotTag myTag;

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem droppedItemUI = eventData.pointerDrag.GetComponent<InventoryItem>();
        if (droppedItemUI != null)
        {
            SetItem(droppedItemUI); // ���� ���Կ� ������ ���� (�ڵ����� CarriedItem ����)
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(Inventory.CarriedItem == null) return;
            if(myTag != SlotTag.None && Inventory.CarriedItem.myItem.itemTag != myTag) return;
            SetItem(Inventory.CarriedItem);
        }
    }

    public void SetItem(InventoryItem itemUI)
    {
        // ������ ���Կ� �ִ� �������� �ִٸ� ó�� (�巡�� ���� �������� �ƴ϶�, ���� ��ü�� ���� ������)
        if (myItemUI != null)
        {
            // TODO: ���� ������ UI�� ��Ȱ��ȭ�ϰų� �ı��ϴ� ���� �߰�
            // Destroy(myItemUI.gameObject); // �ʿ��ϴٸ� ���� ������ UI�� �ı�
        }
        if (itemUI.activeSlot != null)
        {
            itemUI.activeSlot.myItemData = null; // ���� ������ ������ ���
            itemUI.activeSlot.myItemUI = null; // ���� ������ UI ���� ���
        }

        Inventory.CarriedItem = null;

        myItemData = itemUI.myItem;

        myItemUI = itemUI;
        myItemUI.activeSlot = this; // ������ UI�� ���� ������ �����ϵ��� ����

        myItemUI.transform.SetParent(transform);
        myItemUI.transform.localPosition = Vector3.zero; // ��ġ �ʱ�ȭ
        myItemUI.canvasGroup.blocksRaycasts = true;


    }

    public void ClearSlot()
    {
        if (myItemUI != null)
        {
            Destroy(myItemUI.gameObject); // UI �ν��Ͻ��� �ı�
            myItemUI = null;
        }
        myItemData = null; // �����͵� ���
        // UI�� �ð������� �ʱ�ȭ�ϴ� ���� (��: �̹��� �����)
    }
}
