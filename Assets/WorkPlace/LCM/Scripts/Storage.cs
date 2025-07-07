using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using System;
using System.IO;

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

    private Dictionary<string, Item> _allItemsDictionary = new Dictionary<string, Item>();

    private string saveFilePath;

    private void Awake()
    {
        base.Awake();
        if (_storageUIRootPanel != null)
        {
            _storageUIRootPanel.SetActive(false); // �ʱ⿡�� ����
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "storage_data.json");
        Debug.Log("Storage Save Path: " + saveFilePath);

        _allItemsDictionary.Clear();

        LoadItemsFromFolder("Items/Consumable");

        LoadItemsFromFolder("Items/Matarials");

        LoadItemsFromFolder("Items/Tool");
        Debug.Log($"Storage.Awake: ��� �������� �� {_allItemsDictionary.Count}���� �������� _allItemsDictionary�� �ε�Ǿ����ϴ�.");
    }

    void Update()
    {
        // ����� Ű�� ������ ���� ����
        if (Input.GetKeyDown(KeyCode.L)) // 'L' Ű�� ������ ��
        {
            Debug.Log("--- [DEBUG] ���� â�� ���� ���� (L Ű �Է�) ---");
            if (storageSlots == null || storageSlots.Length == 0)
            {
                Debug.Log("â�� ������ �������� �ʾҰų� ����ֽ��ϴ�.");
                return;
            }

            for (int i = 0; i < storageSlots.Length; i++)
            {
                if (storageSlots[i] != null && storageSlots[i].myItemData != null && storageSlots[i].myItemUI != null)
                {
                    Debug.Log($"- ���� {i}: {storageSlots[i].myItemData.itemName}, ����: {storageSlots[i].myItemUI.CurrentQuantity}");
                }
                else if (storageSlots[i] != null)
                {
                    Debug.Log($"- ���� {i}: �������");
                }
                else
                {
                    Debug.Log($"- ���� {i}: (���� ������Ʈ ��ü�� null)");
                }
            }
            Debug.Log("-------------------------------------------");
        }
    }

    public void SetStorageSlots(InventorySlot[] slots)
    {
        this.storageSlots = slots;
        Debug.Log($"Storage: {slots.Length}���� ������ Storage �ν��Ͻ��� �����Ǿ����ϴ�.");

        LoadStorageData();
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
        SaveStorageData();

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
                    slot.ClearSlot();
                    // ClearSlot() ȣ�� �� myItemUI�� null�� �� �� �����Ƿ�,
                    // �̺�Ʈ ȣ�� �ÿ��� 0�� �����ϰų�, myItemUI�� �������� �ʵ��� �մϴ�.
                    OnStorageSlotItemUpdated?.Invoke(Array.IndexOf(storageSlots, slot), itemData, 0);
                }
                else
                {
                    slot.UpdateSlotUI(); // UI ������Ʈ
                    OnStorageSlotItemUpdated?.Invoke(Array.IndexOf(storageSlots, slot), itemData, slot.myItemUI.CurrentQuantity); // �̺�Ʈ �߻�
                }

                

                if (quantityToRemove <= 0) break; // ��� ���ŵ�
            }
        }

        if (quantityToRemove > 0)
        {
            Debug.LogWarning($"â���� '{itemData.itemName}' {quantityToRemove}���� ��� ������ �� �������ϴ�. ����� ������ �����ϴ�.");
        }

        SaveStorageData();
    }

    public bool HasItem(Item itemData)
    {
        if(itemData == null) return false;

        foreach(var slot in storageSlots)
        {
            if(slot.myItemData == itemData && slot.myItemUI != null && slot.myItemUI.CurrentQuantity > 0)
            {
                return true;
            }
        }

        return false;
    }

    // -------- JSON ����/�ҷ����� ---------------

    public void SaveStorageData()
    {
        StorageSaveData saveData = new StorageSaveData();

        foreach (var slot in storageSlots)
        {
            InventorySlotSaveData slotSaveData = new InventorySlotSaveData();
            if (slot.myItemData != null && slot.myItemUI != null)
            {
                slotSaveData.itemInSlot = new InventoryItemSaveData
                {
                    itemData = new ItemSaveData { itemName = slot.myItemData.itemName }, // ������ �̸��� ����
                    currentQuantity = slot.myItemUI.CurrentQuantity
                };
            }
            // �������� ������ itemInSlot�� null�� ����
            saveData.slots.Add(slotSaveData);
        }

        string json = JsonUtility.ToJson(saveData, true); // true�� �������� ���� ���ڰ� ������
        File.WriteAllText(saveFilePath, json);
        Debug.Log("â�� ������ ���� �Ϸ�: " + saveFilePath);
    }

    public void LoadStorageData()
    {

        Debug.Log("LoadStorageData() ȣ��");
        if (File.Exists(saveFilePath))
        {
            Debug.Log("�������� ����");
            string json = File.ReadAllText(saveFilePath);
            StorageSaveData loadedData = JsonUtility.FromJson<StorageSaveData>(json);

            // ���� ���� �ʱ�ȭ
            foreach (var slot in storageSlots)
            {
                if (slot != null) // null üũ �߰�
                {
                    slot.ClearSlot(); // ���� UI ������ ����
                    Debug.Log($"Ŭ���� ���� ����: {Array.IndexOf(storageSlots, slot)}�� ���� �ʱ�ȭ��."); // ����� �α� �߰�
                }
            }

            for (int i = 0; i < loadedData.slots.Count; i++)
            {

                InventorySlotSaveData slotSaveData = loadedData.slots[i];

                if (i >= storageSlots.Length)
                {
                    Debug.LogWarning("�ҷ��� ���� ������ ���� â�� ���� �������� �����ϴ�. �ʰ��� �����ʹ� ���õ˴ϴ�.");
                    break;
                }

                
                if (slotSaveData.itemInSlot != null)
                {
                    string itemName = slotSaveData.itemInSlot.itemData.itemName;
                    int quantity = slotSaveData.itemInSlot.currentQuantity;

                    Debug.Log($"[LoadStorageData] ���� {i}: JSON���� '{itemName}' (����: {quantity}) ������ �ε� �õ� ��.");

                    // ����� ������ �̸����� ���� Item ScriptableObject ã��
                    if (_allItemsDictionary.TryGetValue(itemName, out Item actualItemData))
                    {
                        Debug.Log($"[LoadStorageData] ���� {i}: _allItemsDictionary���� '{itemName}' ������ ã�� ����!");


                        // ���ο� InventoryItem UI ���� �� �ʱ�ȭ
                        InventoryItem newItemUI = Instantiate(_itemPrefab, storageSlots[i].transform);
                        newItemUI.Initialize(actualItemData, storageSlots[i]);
                        RectTransform itemRectTransform = newItemUI.GetComponent<RectTransform>();
                        if (itemRectTransform != null)
                        {
                            itemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 80);
                            itemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80);
                            itemRectTransform.sizeDelta = new Vector2(-10, -10);
                        }
                        newItemUI.CurrentQuantity = quantity;

                        storageSlots[i].SetItem(newItemUI);
                        storageSlots[i].UpdateSlotUI();
                        OnStorageSlotItemUpdated?.Invoke(i, actualItemData, quantity);
                        Debug.Log($"[LoadStorageData] ���� {i}: UI�� '{itemName}' ������ ���� �Ϸ�.");
                    }
                    else
                    {
                        Debug.LogWarning($"������ '{itemName}'��(��) ã�� �� �����ϴ�. �ش� �������� �ε���� �ʽ��ϴ�.");
                    }
                }
                else
                {
                    // ������ ��������� ClearSlot()�� ȣ���Ͽ� UI�� ��� (�̹� ������ ��ü �ʱ�ȭ������ ���������)
                    Debug.Log($"[LoadStorageData] ���� {i}: ����ִ� �����Դϴ�. ClearSlot ȣ��.");
                    storageSlots[i].ClearSlot();
                    OnStorageSlotItemUpdated?.Invoke(i, null, 0); // ����ִ� ���� UI ������Ʈ �˸�
                }
            }
            Debug.Log("Storage â�� ������ �ҷ����� �Ϸ�.");
        }
        else
        {
            Debug.LogWarning("����� â�� ������ ������ �����ϴ�. ���ο� â�� �����մϴ�.");
        }
    }

    private void LoadItemsFromFolder(string folderPath)
    {
        Item[] itemsInFolder = Resources.LoadAll<Item>(folderPath);
        Debug.Log($"Resources.LoadAll<Item>('{folderPath}') ���: {itemsInFolder.Length}���� ������ �߰�.");
        foreach (Item item in itemsInFolder)
        {
            if (item == null)
            {
                Debug.LogWarning($"��� '{folderPath}'���� null �������� �ε�Ǿ����ϴ�. ���� �ջ� �Ǵ� �߸��� Ÿ���� �� �ֽ��ϴ�.");
                continue;
            }

            if (!_allItemsDictionary.ContainsKey(item.itemName))
            {
                _allItemsDictionary.Add(item.itemName, item);
                Debug.Log($"'_allItemsDictionary'�� ������ �߰���: {item.itemName}");
            }
            else
            {
                Debug.LogWarning($"�ߺ��� ������ �̸��� �߰ߵǾ����ϴ�: {item.itemName} (���: {folderPath}). ������ �̸��� ����ϼ���.");
            }
        }
        Debug.Log($"'{folderPath}'���� {itemsInFolder.Length}���� �������� �ε��߽��ϴ�.");
    }
}
