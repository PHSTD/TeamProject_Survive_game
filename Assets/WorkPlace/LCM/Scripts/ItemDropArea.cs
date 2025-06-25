using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // ��ӵ� ���� ������Ʈ�� InventoryItem ������Ʈ�� ������ �ִ��� Ȯ���մϴ�.
        InventoryItem droppedItemUI = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (droppedItemUI != null)
        {
            if (Inventory.Instance == null) // <- �߰�
            {
                Debug.LogError("Error: Inventory.Instance is null when trying to drop item!");
                return; // Inventory �ν��Ͻ��� ������ �� �̻� �������� ����
            }
            // Inventory �Ŵ������� �ش� �������� ������� ��û�մϴ�.
            // �� �޼���� �Ʒ� 3�ܰ迡�� Inventory ��ũ��Ʈ�� �߰��� ���Դϴ�.
            Inventory.Instance.DropItemFromSlot(droppedItemUI);
        }
    }
}
