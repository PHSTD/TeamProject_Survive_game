using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DesignPattern;
using System;
using TMPro;

public class Inventory : Singleton<Inventory>
{
    public static InventoryItem CarriedItem;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] hotbarSlots;
    [SerializeField] InventorySlot[] persistentHotbarSlots;

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
    [SerializeField] public int _currentHotbarSlotIndex = 0; // ���� ���õ� �ֹ� ���� �ε��� (�⺻�� 0)

    [Header("Item Dropping")]
    [SerializeField] private float _dropDistance = 1.5f; // �÷��̾�κ��� �������� ������ �Ÿ�
    [SerializeField] private LayerMask _groundLayer; // �ٴ� ���̾�
    [SerializeField] private float _scatterForce = 2f;

    // �ֹ� ���� ������ �ܺο� �˸��� �̺�Ʈ (UI ������Ʈ � ���)
    public event Action<int> OnHotbarSlotChanged;

    //// �߰�: �ֹ� ���Ե��� ������ �����͸� ����ȭ�ϱ� ���� �̺�Ʈ (�ʿ��ϴٸ�)
    public event Action<int, Item, int> OnHotbarSlotItemUpdated;



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

        // ��� �ֹ� ���� �迭 �ʱ�ȭ �� ����ȭ (Awake �Ǵ� Start���� �� ���� ȣ��)
        InitializePersistentHotbarSlots();
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
        bool inventoryActive = _inventoryUIRootPanel.activeSelf;
    }


    public void SetCarriedItem(InventoryItem item)
    {
        if(CarriedItem != null)
        {
            item.activeSlot.SetItem(CarriedItem);
        }

        CarriedItem = item;
        CarriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }

    public void SpawnInventoryItem(Item item)
    {
        Debug.Log($"[Inventory] SpawnInventoryItem ȣ���. �߰� �õ� ������: {item.itemName}");
        // ���� ������ �������� ���, ���� ���� ������ Ȯ��
        if (item.isStackable)
        {
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                if (hotbarSlots[i].myItemData == item && hotbarSlots[i].myItemUI != null && hotbarSlots[i].myItemUI.CurrentQuantity < item.maxStackSize)
                {
                    hotbarSlots[i].myItemUI.CurrentQuantity++; // ���� ����
                    SyncHotbarSlotUI(i); // �ֹ� ����ȭ (�� ���ο��� SetItemInternal�� ���� UI ������Ʈ)
                    Debug.Log($"SUCCESS: '{item.itemName}' stacked in hotbar slot {i}. New Qty: {hotbarSlots[i].myItemUI.CurrentQuantity}");
                    return;
                }
            }
            // === �κ��丮 ���� Ȯ�� (���� ����) ===
            foreach (var slot in inventorySlots)
            {
                if (slot.myItemData == item && slot.myItemUI != null && slot.myItemUI.CurrentQuantity < item.maxStackSize)
                {
                    slot.myItemUI.CurrentQuantity++; // ���� ����
                    Debug.Log($"SUCCESS: '{item.itemName}' stacked in inventory slot {slot.name}. New Qty: {slot.myItemUI.CurrentQuantity}");
                    return; // ������ �߰� �Ϸ�
                }
            }
        }

        // ���� �Ұ����ϰų� �� á�� ���, �� ���Կ� ����
        Debug.Log($"[Inventory] '{item.itemName}' ���� �Ұ����ϰų� �� ���� Ž�� ����.");
        // ���� �ֹ��� �� ���� Ȯ�� (�� �迭 ���)
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].myItemUI == null) // �� �ֹ� ������ ã��
            {
                var newItemUI = Instantiate(itemPrefab, hotbarSlots[i].transform);
                newItemUI.Initialize(item, hotbarSlots[i]); // �κ��丮 �ֹٿ� UI ����
                // �ʱ� ������ 1�� �ƴ϶�� ���⼭ ����
                newItemUI.CurrentQuantity = 1; // �⺻������ 1�̹Ƿ� ���� ����
                SyncHotbarSlotUI(i); // �ֹ� ����ȭ
                Debug.Log($"�� �ֹ� ���� {i}�� '{item.itemName}' �߰�.");
                return;
            }
        }

        // ������ �� ���ų�, ���� ������ ������������ ��� ���� ������ �� á�� ���
        // ����ִ� �� ���Կ� �������� ����
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItemUI == null) // ����ִ� ������ ã��
            {
                var newItemUI = Instantiate(itemPrefab, inventorySlots[i].transform);
                newItemUI.Initialize(item, inventorySlots[i]); // Initialize ȣ��
                Debug.Log($"�� ���Կ� '{item.itemName}' �߰�.");
                return; // ������ �߰� �Ϸ�
            }
        }

        Debug.LogWarning($"�κ��丮�� ���� á���ϴ�. '{item.itemName}'��(��) �߰��� �� �����ϴ�.");


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
            //����� �������� ����
            int quantityToDrop = itemToDropUI.CurrentQuantity;
            Item droppedItemData = itemToDropUI.myItem;

            //�������� ������ ��ġ�� ��� (�÷��̾� ����)
            Transform playerTransform = SamplePlayerManager.Instance.Player.transform; // PlayerController�� transform
            Vector3 playerForward = playerTransform.forward;
            Vector3 dropPosition = playerTransform.position + playerTransform.forward * _dropDistance;
            dropPosition.y += 0.5f;

            RaycastHit hit;
            if (Physics.Raycast(dropPosition + Vector3.up * 10f, Vector3.down, out hit, 20f, _groundLayer))
            {
                dropPosition.y = hit.point.y + 0.1f;
            }

            // ������ŭ ���� ������ ���� ����
            for (int i = 0; i < quantityToDrop; i++)
            {
                // �������� ������ ��ġ�� ���ݾ� �ٸ��� �Ͽ� ��ġ�� �ʰ� ��
                Vector3 scatteredPosition = dropPosition;
                // ������ ���� �������� ��¦ ������ ��
                scatteredPosition.x += UnityEngine.Random.Range(-0.5f, 0.5f);
                scatteredPosition.z += UnityEngine.Random.Range(-0.5f, 0.5f);
                scatteredPosition.y += UnityEngine.Random.Range(0f, 0.2f); // ���̵� ��¦ �ٸ���

                GameObject worldItemGO = Instantiate(itemWorldPrefab, scatteredPosition, Quaternion.identity);
                WorldItem worldItemScript = worldItemGO.GetComponent<WorldItem>();

                if (worldItemScript != null)
                {
                    // WorldItem�� Initialize �޼��带 ȣ���Ͽ� ������ �����͸� ���� (������ 1���� ����)
                    worldItemScript.Initialize(droppedItemData);

                    // Rigidbody�� �ִٸ� ���� ���ؼ� �� �� �ڿ������� ������ �� �� ����
                    Rigidbody rb = worldItemGO.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
                        rb.AddForce(randomDirection * _scatterForce, ForceMode.Impulse);
                    }
                }
                else
                {
                    Debug.LogError($"��ӵ� ���� ������ '{itemWorldPrefab.name}'�� WorldItem ��ũ��Ʈ�� �����ϴ�!");
                }
            }
            // �κ��丮 ���Կ��� �������� �����ϰ� UI �ν��Ͻ� �ı�
            bool wasHotbarItem = false;
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                if (hotbarSlots[i] == itemToDropUI.activeSlot)
                {
                    hotbarSlots[i].ClearSlot(); 
                    SyncHotbarSlotUI(i); // �ֹ� ����ȭ
                    wasHotbarItem = true;
                    break;
                }
            }

            if (!wasHotbarItem && itemToDropUI.activeSlot != null) // �Ϲ� �κ��丮 ���Կ��� ������ ���
            {
                itemToDropUI.activeSlot.ClearSlot();
            }
            else if (itemToDropUI.activeSlot == null) // ���Կ� ���� �������� ������ ��� (��: �巡�� �� ���� ������ ����)
            {
                Destroy(itemToDropUI.gameObject);
            }

            CarriedItem = null;
        }

        //�κ��丮 ���Կ��� �������� �����ϰ� UI �ν��Ͻ� �ı�
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

        // �ֹ� ���Կ��� ����
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].myItemData == itemToRemove)
            {
                // ���� ������ �������̰�, ���� ������ ������ �������� ������ ������ ����
                if (itemToRemove.isStackable && hotbarSlots[i].myItemUI.CurrentQuantity > amount - removedCount)
                {
                    hotbarSlots[i].myItemUI.CurrentQuantity -= (amount - removedCount);
                    removedCount = amount; // ��� ���ŵ� ������ ����
                    break;
                }
                else // ���� �Ұ����ϰų�, ���� ������ ������ ���� �����̸� ���� ���
                {
                    int currentStack = hotbarSlots[i].myItemUI.CurrentQuantity;
                    hotbarSlots[i].ClearSlot();
                    removedCount += currentStack;
                    SyncHotbarSlotUI(i);
                    if (removedCount >= amount) break;
                }
            }
        }

        // �κ��丮 ���Կ��� ���� (�ֹٿ��� ���� ���ŵ��� ���� ���)
        if (removedCount < amount)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].myItemData == itemToRemove)
                {
                    // ���� ������ �������̰�, ���� ������ ������ �������� ������ ������ ����
                    if (itemToRemove.isStackable && inventorySlots[i].myItemUI.CurrentQuantity > amount - removedCount)
                    {
                        inventorySlots[i].myItemUI.CurrentQuantity -= (amount - removedCount);
                        removedCount = amount;
                        break;
                    }
                    else // ���� �Ұ����ϰų�, ���� ������ ������ ���� �����̸� ���� ���
                    {
                        int currentStack = inventorySlots[i].myItemUI.CurrentQuantity;
                        inventorySlots[i].ClearSlot();
                        removedCount += currentStack;
                        if (removedCount >= amount) break;
                    }
                }
            }
        }

        Debug.Log($"{itemToRemove.name} {removedCount}���� �κ��丮���� �����߽��ϴ�.");
        if (removedCount < amount)
        {
            Debug.LogWarning($"��û�� {amount}�� �� {amount - removedCount}���� �������� ���߽��ϴ�. ������ ����.");
        }
    }

    // ���Ӱ� �߰��� �޼���: ��� �ֹ� ���� �ʱ�ȭ �� ����ȭ
    private void InitializePersistentHotbarSlots()
    {
        if (hotbarSlots == null || persistentHotbarSlots == null)
        {
            Debug.LogError("Hotbar slots arrays are not assigned!");
            return;
        }

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            // �κ��丮 �� �ֹ� ���Կ� ������ UI�� �����ϴ��� Ȯ���մϴ�.
            if (hotbarSlots[i].myItemUI != null)
            {
                var newItemUI = Instantiate(itemPrefab, persistentHotbarSlots[i].transform);
                newItemUI.Initialize(hotbarSlots[i].myItemUI.myItem, persistentHotbarSlots[i]);
                newItemUI.CurrentQuantity = hotbarSlots[i].myItemUI.CurrentQuantity;
                persistentHotbarSlots[i].SetItem(newItemUI);
            }
            else // �κ��丮 �� �ֹ� ������ ����ִ� ���
            {
                persistentHotbarSlots[i].ClearSlot();
            }
        }
    }

    // ���Ӱ� �߰��� �޼���: �ֹ� ���� ���� �� ����ȭ
    private void SyncHotbarSlotUI(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length || index >= persistentHotbarSlots.Length) return;

        

        // UI �ν��Ͻ� ����ȭ (���� UI �ı� �� ���� ���� �Ǵ� ������Ʈ)
        if (persistentHotbarSlots[index].myItemUI != null)
        {
            Destroy(persistentHotbarSlots[index].myItemUI.gameObject);
            persistentHotbarSlots[index].myItemUI = null;
        }

        // ���� �ֹ� ������ ������ �����͸� ��� �ֹ� �������� ����
        persistentHotbarSlots[index].myItemData = hotbarSlots[index].myItemData;

        if (hotbarSlots[index].myItemUI != null)
        {
            var newItemUI = Instantiate(itemPrefab, persistentHotbarSlots[index].transform);
            newItemUI.Initialize(hotbarSlots[index].myItemUI.myItem, persistentHotbarSlots[index]);
            newItemUI.CurrentQuantity = hotbarSlots[index].myItemUI.CurrentQuantity;
        }

        OnHotbarSlotItemUpdated?.Invoke(index, persistentHotbarSlots[index].myItemData,
                                        persistentHotbarSlots[index].myItemUI != null ? persistentHotbarSlots[index].myItemUI.CurrentQuantity : 0);
    }

    public void HandleItemDropOrClick(InventorySlot targetSlot, InventoryItem droppedItemUI)
    {
        if (CarriedItem == null)
        {
            return;
        }

        // Case 2: �������� ��� �ְ�, �� ���Կ� ����ϴ� ��� (Ŭ���� �ƴ�)
        // (CarriedItem != null && targetSlot.myItemUI == null)
        else if (targetSlot.myItemUI == null)
        {
            Debug.Log("HangleItem: �� ���Կ� �������� (���)");
            InventorySlot originalSlot = CarriedItem.activeSlot;

            targetSlot.SetItem(CarriedItem);


            //���� ���� �����
            if (originalSlot != null)
            {
                //originalSlot.ClearSlot(); // <-- �� ������ �ּ� ó���ϰų� �����ؾ� �մϴ�!
                originalSlot.myItemData = null; // ���� ������ �����͸� ���ϴ�.
                originalSlot.myItemUI = null;   // ���� ������ UI ������ ���ϴ�.


                CheckAndSyncSlotIfHotbar(originalSlot); // �ֹٶ�� ����ȭ
            }

            // �ֹٶ�� ����ȭ
            CheckAndSyncSlotIfHotbar(targetSlot);
            CarriedItem = null; // ��� �ִ� ������ ����
        }
        // Case 3: �������� ��� �ְ�, �������� �ִ� ���Կ� ����ϴ� ��� (Ŭ���� �ƴ�)
        // (CarriedItem != null && targetSlot.myItemUI != null)
        else // (CarriedItem != null && targetSlot.myItemUI != null)
        {
            // ���� �������̰� ���� �����ϴٸ� ���� �õ�
            if (CarriedItem.myItem == targetSlot.myItemData && CarriedItem.myItem.isStackable)
            {
                int transferAmount = Mathf.Min(
                    CarriedItem.CurrentQuantity,
                    CarriedItem.myItem.maxStackSize - targetSlot.myItemUI.CurrentQuantity
                );

                if (transferAmount > 0)
                {
                    targetSlot.myItemUI.CurrentQuantity += transferAmount;
                    CarriedItem.CurrentQuantity -= transferAmount;

                    // ����ȭ
                    CheckAndSyncSlotIfHotbar(targetSlot);
                    if (CarriedItem.CurrentQuantity <= 0) // ��� �ִ� �������� ��� ���õǾ�����
                    {
                        if (CarriedItem.activeSlot != null) // ���� ������ �־��ٸ�
                        {
                            //CarriedItem.activeSlot.ClearSlot(); // ���� ���� ���
                            CheckAndSyncSlotIfHotbar(CarriedItem.activeSlot); // �ֹٶ�� ����ȭ
                        }
                        Destroy(CarriedItem.gameObject); // ��� �ִ� ������ UI �ı�
                        CarriedItem = null; // ��� �ִ� ������ ����
                    }
                    // else: ��� �ִ� �������� ����������, CarriedItem�� ��� ���콺�� �پ������Ƿ� ���� ó�� ���ʿ�.
                    return; // ���� �Ϸ� �� �Լ� ����
                }
            }

            // ���� �Ұ����ϰų� ���� ������ ������ ������ ��ȯ
            Debug.Log("HangleItem: ������ ��ȯ");
            InventoryItem tempCarriedItem = CarriedItem;
            InventoryItem tempTargetItem = targetSlot.myItemUI;

            // ���� ���Կ� ��� �������� ����
            if (tempCarriedItem.activeSlot != null)
            {
                //tempCarriedItem.activeSlot.ClearSlot(); // ���� ���� ���
                CheckAndSyncSlotIfHotbar(tempCarriedItem.activeSlot);

                tempCarriedItem.activeSlot.SetItem(tempTargetItem); // ���� ���Կ� ��� ������ ����
                CheckAndSyncSlotIfHotbar(tempCarriedItem.activeSlot);
            }
            else // �巡�� ���̴� �������� ���� ������ ������ ��� (��: ���� ������ �������� �ٷ� �巡�׵� ���)
            {
                // ���� �ִ� ������ UI�� �ı�
                targetSlot.ClearSlot(); // ��� ������ ��� (tempTargetItem UI�� �ı���)
                                        // tempTargetItem.activeSlot�� �̹� targetSlot�̹Ƿ� �ߺ� Clear ���ʿ�
            }

            // ��� ���Կ� ��� �ִ� �������� ����
            targetSlot.SetItem(tempCarriedItem);
            CheckAndSyncSlotIfHotbar(targetSlot);

            CarriedItem = null; // ��� �ִ� ������ ����
        }
    }

    // �־��� ������ �ֹ� ���� �� �ϳ����� Ȯ���ϰ�, �׷��ٸ� SyncHotbarSlotUI�� ȣ��
    public void CheckAndSyncSlotIfHotbar(InventorySlot slot)
    {
        Debug.Log("�ֹٽ������� üũ");
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i] == slot)
            {
                Debug.Log("�ֹٽ����� �µ���");
                SyncHotbarSlotUI(i);
                return;
            }
        }
    }

    //Item PickRandomItem()
    //{
    //    int random = Random.Range(0, items.Length);
    //    return items[random];
    //}
}
