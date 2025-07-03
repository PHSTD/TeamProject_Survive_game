using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using System;

public class Storage : Singleton<Storage>
{
    [SerializeField]
    private InventorySlot[] storageSlots; // â�� ���� �迭

    // �ʿ��ϴٸ� â�� UI �г� � ������ �� �ֽ��ϴ�.
    [SerializeField]
    private GameObject _storageUIRootPanel;

    [SerializeField] private InventoryItem _itemPrefab;

    // â�� ������ ������ ������ �˸��� �̺�Ʈ (UI ������Ʈ��)
    public event Action<int, Item, int> OnStorageSlotItemUpdated;

    private void Awake()
    {
        SingletonInit();
        if (_storageUIRootPanel != null)
        {
            _storageUIRootPanel.SetActive(false); // �ʱ⿡�� ����
        }


    }

    public void SetStorageSlots(InventorySlot[] slots)
    {
        this.storageSlots = slots;
        Debug.Log($"Storage: {slots.Length}���� ������ Storage �ν��Ͻ��� �����Ǿ����ϴ�.");
    }

    public void AddItemToStorage(Item itemData, int quantity)
    {
        if (itemData == null || quantity <= 0) return;

        // itemPrefab�� �Ҵ�Ǿ����� Ȯ��
        if (_itemPrefab == null)
        {
            Debug.LogError("Storage: InventoryItem Prefab�� �Ҵ���� �ʾҽ��ϴ�. Inspector�� Ȯ���ϼ���!");
            return;
        }

        if (itemData.isStackable)
        {
            foreach (var slot in storageSlots)
            {
                if (slot.myItemData == itemData && slot.myItemUI != null && slot.myItemUI.CurrentQuantity < itemData.maxStackSize)
                {
                    int spaceLeft = itemData.maxStackSize - slot.myItemUI.CurrentQuantity;
                    int actualAdd = Mathf.Min(quantity, spaceLeft);
                    slot.myItemUI.CurrentQuantity += actualAdd;
                    quantity -= actualAdd;

                    //slot.UpdateSlotUI();
                    OnStorageSlotItemUpdated?.Invoke(Array.IndexOf(storageSlots, slot), itemData, slot.myItemUI.CurrentQuantity); // �̺�Ʈ �߻�

                    if (quantity <= 0) return; // ��� �߰���
                }
            }
        }

        while (quantity > 0)
        {
            foreach (var slot in storageSlots)
            {
                if (slot.myItemUI == null) // �� ������ ã��
                {
                    InventoryItem newItemUI = Instantiate(_itemPrefab, slot.transform);
                    newItemUI.Initialize(itemData, slot);
                    RectTransform itemRectTransform = newItemUI.GetComponent<RectTransform>();
                    if (itemRectTransform != null)
                    {
                        // ����: ���� ũ�⿡ �� ä��ų� �ణ �۰� ����ϴ�.
                        // Anchor Presets�� �̿��ϴ� ���
                        //itemRectTransform.anchorMin = Vector2.zero;   // ���� �Ʒ�
                        //itemRectTransform.anchorMax = Vector2.one;    // ������ ��
                        //itemRectTransform.sizeDelta = new Vector2(0, 0); // �θ� ũ�⿡ �� ä��

                        // �Ǵ� ������ ũ��� ���� (�θ� ũ��� ����)
                        itemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 80); // �ʺ� 80
                        itemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80);   // ���� 80

                        // �ణ�� �е��� �ַ��� (sizeDelta�� ������ ����)
                        itemRectTransform.sizeDelta = new Vector2(-10, -10); // �¿���� 5�� �е�
                    }
                    slot.SetItem(newItemUI);
                    int addedQuantity = Mathf.Min(quantity, itemData.maxStackSize);
                    newItemUI.CurrentQuantity = addedQuantity;

                    quantity -= addedQuantity;

                    slot.UpdateSlotUI(); // UI ������Ʈ�� ���� �߰�
                    OnStorageSlotItemUpdated?.Invoke(Array.IndexOf(storageSlots, slot), itemData, slot.myItemUI.CurrentQuantity); // �̺�Ʈ �߻�

                    Debug.Log($"â�� '{itemData.itemName}' {slot.myItemUI.CurrentQuantity}�� �߰���.");
                    break; // ���� ������ �Ǵ� ���� ���� ó��
                }
            }
            if (quantity > 0)
            {
                Debug.LogWarning($"â�� ���� ���� '{itemData.itemName}' {quantity}���� ��� �߰��� �� �������ϴ�.");
                break;
            }
        }


    }

    public int GetItemCount(Item itemData)
    {
        if (itemData == null) return 0;

        int count = 0;
        foreach (var slot in storageSlots)
        {
            // InventorySlot�� Item �����͸� ���� ������ �ִ��� Ȯ��
            // �Ǵ� InventorySlotUI�� ����Ǿ� �ְ� �ش� UI�� Item �����͸� ������ �ִٸ� �׷��� ����
            if (slot.myItemData == itemData)
            {
                if (slot.myItemUI != null)
                {
                    count += slot.myItemUI.CurrentQuantity;
                }
                // ���� myItemUI�� null������ myItemData�� �Ҵ�Ǿ� �ִٸ�
                // (�����ʹ� �ִµ� UI�� ���� �������� ���� ���)
                // InventorySlot ��ü�� ���� ������ �ִٸ� �� ������ ����ؾ� �մϴ�.
                // slot.CurrentQuantity ���� �ʵ尡 InventorySlot�� �ִٸ� �� ��Ȯ�� �� �ֽ��ϴ�.
            }
        }
        return count;
    }
    public void ToggleStorageUI()
    {
        if (_storageUIRootPanel != null)
        {
            _storageUIRootPanel.SetActive(!_storageUIRootPanel.activeSelf);
        }
    }

    public void RemoveItem(Item itemData, int quantityToRemove)
    {
        if (itemData == null || quantityToRemove <= 0) return;

        // ���� ������ �����ۺ��� �������� ���� (���� ����: ������ ����)
        // ���⼭�� ���� �տ� �ִ� �����ۺ��� �����ϴ� ������ �����մϴ�.
        foreach (var slot in storageSlots)
        {
            if (slot.myItemData == itemData && slot.myItemUI != null)
            {
                int currentQuantity = slot.myItemUI.CurrentQuantity;
                int actualRemove = Mathf.Min(quantityToRemove, currentQuantity);

                slot.myItemUI.CurrentQuantity -= actualRemove;
                quantityToRemove -= actualRemove;

                if (slot.myItemUI.CurrentQuantity <= 0)
                {
                    // ������ ������ �����Ϳ� UI�� ������ ����
                    slot.ClearSlot(); // InventorySlot�� ClearSlot() �޼��尡 �ʿ��մϴ�!
                }
                else
                {
                    slot.UpdateSlotUI(); // UI ������Ʈ
                }

                OnStorageSlotItemUpdated?.Invoke(Array.IndexOf(storageSlots, slot), itemData, slot.myItemUI.CurrentQuantity); // �̺�Ʈ �߻�

                if (quantityToRemove <= 0) break; // ��� ���ŵ�
            }
        }

        if (quantityToRemove > 0)
        {
            Debug.LogWarning($"â���� '{itemData.itemName}' {quantityToRemove}���� ��� ������ �� �������ϴ�. ����� ������ �����ϴ�.");
        }
    }
}
