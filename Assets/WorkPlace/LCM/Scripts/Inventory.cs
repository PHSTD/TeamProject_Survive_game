using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DesignPattern;
using System;

public class Inventory : Singleton<Inventory>
{
    public static InventoryItem CarriedItem;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] hotbarSlots;

    // 0=Head, 1=Chest, 2=Legs, 3=Feet
    //[SerializeField] InventorySlot[] equipmentSlots;

    public Transform draggablesTransform;
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] Item[] items;

    //[Header("Debug")]
    //[SerializeField] Button giveItemBtn;

    [Header("UI Management")]
    [SerializeField] private GameObject _inventoryUIRootPanel; // �κ��丮 UI�� �ֻ��� GameObject (Panel ��)

    [Header("Hotbar Management")]
    [SerializeField] private int _currentHotbarSlotIndex = 0; // ���� ���õ� �ֹ� ���� �ε��� (�⺻�� 0)

    [Header("Item Dropping")]
    [SerializeField] private float _dropDistance = 1.5f; // �÷��̾�κ��� �������� ������ �Ÿ�
    [SerializeField] private LayerMask _groundLayer; // �ٴ� ���̾�

    // �ֹ� ���� ������ �ܺο� �˸��� �̺�Ʈ (UI ������Ʈ � ���)
    public event Action<int> OnHotbarSlotChanged;


    void Awake()
    {
        SingletonInit();

        // �κ��丮 UI �г� �ʱ� ���� ���� (���� �� ��Ȱ��ȭ)
        if (_inventoryUIRootPanel != null)
        {
            _inventoryUIRootPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Inventory: Inventory UI Root Panel�� �Ҵ���� �ʾҽ��ϴ�. UI ����� �۵����� ���� �� �ֽ��ϴ�.");
        }

        // �ʱ� �ֹ� ���� ������ �˸�
        OnHotbarSlotChanged?.Invoke(_currentHotbarSlotIndex);
    }

    void Update()
    {
        if(CarriedItem == null) return;

        CarriedItem.transform.position = Input.mousePosition;
    }

    public void SelectHotbarSlot(int index)
    {
        if (index >= 0 && index < hotbarSlots.Length)
        {
            _currentHotbarSlotIndex = index;
            // �ֹ� ������ ����Ǿ����� �ܺο� �˸��ϴ�.
            OnHotbarSlotChanged?.Invoke(_currentHotbarSlotIndex);
            Debug.Log($"�ֹ� ���� {index + 1}���� ���õǾ����ϴ�.");
        }
        else
        {
            Debug.LogWarning($"��ȿ���� ���� �ֹ� ���� �ε���: {index}. �ֹ� ���� ������ 0���� {hotbarSlots.Length - 1}�Դϴ�.");
        }
    }

    public Item GetCurrentHotbarItem()
    {
        if (_currentHotbarSlotIndex >= 0 && _currentHotbarSlotIndex < hotbarSlots.Length)
        {
            return hotbarSlots[_currentHotbarSlotIndex].myItemData;
        }
        return null;
    }

    public void ToggleInventoryUI()
    {
        SampleUIManager.Instance.ToggleInventoryUI();
    }


    public void SetCarriedItem(InventoryItem item)
    {
        if(CarriedItem != null)
        {
            if(item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != CarriedItem.myItem.itemTag) return;
            item.activeSlot.SetItem(CarriedItem);
        }

        CarriedItem = item;
        CarriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }


    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item;
        //if (_item == null)
        ////TODO: �������� ������� ��� ������ ����
        //{ _item = PickRandomItem(); }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Check if the slot is empty
            if (inventorySlots[i].myItemUI == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }


    }
    public void DropItemFromSlot(InventoryItem itemToDropUI)
    {
        if (itemToDropUI == null || itemToDropUI.myItem == null)
        {
            Debug.LogWarning("��ȿ���� ���� �������� �������� �õ��߽��ϴ�.");
            CarriedItem = null; // Ȥ�� �� ��Ȳ ���
            return;
        }

        GameObject itemWorldPrefab = itemToDropUI.myItem.WorldPrefab;
        if (itemWorldPrefab == null)
        {
            Debug.LogWarning($"������ '{itemToDropUI.myItem.itemName}'�� ����� 3D ���� �������� �����ϴ�. ���� �� �����ϴ�.", itemToDropUI.myItem);
            // ������ �������� ��� �κ��丮������ ������ �ϹǷ� �Ʒ� ClearSlot() ������ �����մϴ�.
        }
        else
        {
            // 1. �������� ������ ��ġ�� ��� (�÷��̾� ����)
            Transform playerTransform = SamplePlayerManager.Instance.Player.transform; // PlayerController�� transform
            Vector3 dropPosition = playerTransform.position + playerTransform.forward * _dropDistance;
            dropPosition.y += 0.5f;

            RaycastHit hit;
            if (Physics.Raycast(dropPosition + Vector3.up * 10f, Vector3.down, out hit, 20f, _groundLayer))
            {
                dropPosition.y = hit.point.y + 0.1f;
            }

            // 2. 3D ���� ������Ʈ�� ���忡 �ν��Ͻ�ȭ
            Instantiate(itemWorldPrefab, dropPosition, Quaternion.identity);
        }

        // 3. �κ��丮 ���Կ��� �������� �����ϰ� UI �ν��Ͻ� �ı�
        if (itemToDropUI.activeSlot != null)
        {
            itemToDropUI.activeSlot.ClearSlot(); // �ش� ������ ��� (������ �� UI ���� ����)
            Debug.Log($"������ '{itemToDropUI.myItem.itemName}'��(��) ���Կ��� ���������ϴ�.");
        }
        else
        {
            // ���Կ� �Ҵ���� ���� ������(��: �巡�� ���� ������ �������� ������ ���)
            Destroy(itemToDropUI.gameObject);
        }

        // CarriedItem�� ���ϴ�.
        CarriedItem = null;
    }

    public int GetItemCount(Item item)
    {
        int count = 0;
        foreach(var slot in inventorySlots)
        {
            if(slot.myItemData == item) 
            { 
                count++; 
            }
        }
        foreach(var slot in hotbarSlots)
        {
            if(slot.myItemData == item)
            {
                count++;
            }
        }

        return count;
    }

    public void RemoveItem(Item itemToRemove, int amount = 1)
    {
        int removedCount = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItemData == itemToRemove)
            {
                inventorySlots[i].ClearSlot();
                removedCount++;
                if (removedCount == amount) break;
            }
        }

        for(int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].myItemData == itemToRemove)
            {
                hotbarSlots[i].ClearSlot();
                removedCount++;
                if (removedCount >= amount) break;
            }
        }

        Debug.Log($"{itemToRemove.name}{removedCount}���� �κ��丮���� ���� �߽��ϴ�.");
    }

    //Item PickRandomItem()
    //{
    //    int random = Random.Range(0, items.Length);
    //    return items[random];
    //}
}
