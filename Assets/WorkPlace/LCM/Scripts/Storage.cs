using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;

public class Storage : Singleton<Storage>
{
    [SerializeField]
    private InventorySlot[] storageSlots; // â�� ���� �迭

    // �ʿ��ϴٸ� â�� UI �г� � ������ �� �ֽ��ϴ�.
    [SerializeField]
    private GameObject _storageUIRootPanel;

    private void Awake()
    {
        SingletonInit();
    }

    public void AddItemToStorage(Item itemData, int quantity)
    {
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
                    // InventoryItem �������� Instanitate�Ͽ� â�� ���Կ� �Ҵ�
                    // (�� �κ��� Inventory���� itemPrefab�� public���� �ϰų�,
                    // Storage������ itemPrefab�� �����ϵ��� �����ؾ� �մϴ�.)
                    // ����: var newItemUI = Instantiate(Inventory.Instance.itemPrefab, slot.transform);
                    // newItemUI.Initialize(itemData, slot);
                    // newItemUI.CurrentQuantity = Mathf.Min(quantity, itemData.maxStackSize);
                    // quantity -= newItemUI.CurrentQuantity;

                    // ����ȭ�� ���� ���� �����͸� �����ϰ� UI�� ���߿� �ٽ� �׸��� ��� ���
                    // ���� ���������� InventoryItem UI�� �����ϰ� �ʱ�ȭ�ϴ� ������ �ʿ��մϴ�.
                    slot.SetItemData(itemData); // ���Կ� ������ �����͸� �����ϴ� ���ο� �޼��尡 �ʿ��� �� ����
                    slot.SetItemQuantity(Mathf.Min(quantity, itemData.maxStackSize)); // ���� ����
                    quantity -= Mathf.Min(quantity, itemData.maxStackSize);

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
    public void ToggleStorageUI()
    {
        if (_storageUIRootPanel != null)
        {
            _storageUIRootPanel.SetActive(!_storageUIRootPanel.activeSelf);
        }
    }
}
