using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using System;
using System.IO;

public class Storage : Singleton<Storage>
{
    [SerializeField]
    private InventorySlot[] storageSlots; // 창고 슬롯 배열

    // 필요하다면 창고 UI 패널 등도 관리할 수 있습니다.
    [SerializeField]
    private GameObject _storageUIRootPanel;

    [SerializeField] private InventoryItem _itemPrefab;

    // 창고 아이템 데이터 변경을 알리는 이벤트 (UI 업데이트용)
    public event Action<int, Item, int> OnStorageSlotItemUpdated;

    private Dictionary<string, Item> _allItemsDictionary = new Dictionary<string, Item>();

    private string saveFilePath;

    private void Awake()
    {
        base.Awake();
        if (_storageUIRootPanel != null)
        {
            _storageUIRootPanel.SetActive(false); // 초기에는 숨김
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "storage_data.json");
        Debug.Log("Storage Save Path: " + saveFilePath);

        _allItemsDictionary.Clear();

        LoadItemsFromFolder("Items/Consumable");

        LoadItemsFromFolder("Items/Matarials");

        LoadItemsFromFolder("Items/Tool");
        Debug.Log($"Storage.Awake: 모든 폴더에서 총 {_allItemsDictionary.Count}개의 아이템이 _allItemsDictionary에 로드되었습니다.");
    }

    void Update()
    {
        // 디버그 키를 눌렀을 때만 실행
        if (Input.GetKeyDown(KeyCode.L)) // 'L' 키를 눌렀을 때
        {
            Debug.Log("--- [DEBUG] 현재 창고 슬롯 상태 (L 키 입력) ---");
            if (storageSlots == null || storageSlots.Length == 0)
            {
                Debug.Log("창고 슬롯이 설정되지 않았거나 비어있습니다.");
                return;
            }

            for (int i = 0; i < storageSlots.Length; i++)
            {
                if (storageSlots[i] != null && storageSlots[i].myItemData != null && storageSlots[i].myItemUI != null)
                {
                    Debug.Log($"- 슬롯 {i}: {storageSlots[i].myItemData.itemName}, 수량: {storageSlots[i].myItemUI.CurrentQuantity}");
                }
                else if (storageSlots[i] != null)
                {
                    Debug.Log($"- 슬롯 {i}: 비어있음");
                }
                else
                {
                    Debug.Log($"- 슬롯 {i}: (슬롯 오브젝트 자체도 null)");
                }
            }
            Debug.Log("-------------------------------------------");
        }
    }

    public void SetStorageSlots(InventorySlot[] slots)
    {
        this.storageSlots = slots;
        Debug.Log($"Storage: {slots.Length}개의 슬롯이 Storage 인스턴스에 설정되었습니다.");

        LoadStorageData();
    }

    public void AddItemToStorage(Item itemData, int quantity)
    {
        if (itemData == null || quantity <= 0) return;

        // itemPrefab이 할당되었는지 확인
        if (_itemPrefab == null)
        {
            Debug.LogError("Storage: InventoryItem Prefab이 할당되지 않았습니다. Inspector를 확인하세요!");
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
                    OnStorageSlotItemUpdated?.Invoke(Array.IndexOf(storageSlots, slot), itemData, slot.myItemUI.CurrentQuantity); // 이벤트 발생

                    if (quantity <= 0) return; // 모두 추가됨
                }
            }
        }

        while (quantity > 0)
        {
            foreach (var slot in storageSlots)
            {
                if (slot.myItemUI == null) // 빈 슬롯을 찾음
                {
                    InventoryItem newItemUI = Instantiate(_itemPrefab, slot.transform);
                    newItemUI.Initialize(itemData, slot);
                    RectTransform itemRectTransform = newItemUI.GetComponent<RectTransform>();
                    if (itemRectTransform != null)
                    {
                        // 예시: 슬롯 크기에 꽉 채우거나 약간 작게 만듭니다.
                        // Anchor Presets을 이용하는 방식
                        //itemRectTransform.anchorMin = Vector2.zero;   // 왼쪽 아래
                        //itemRectTransform.anchorMax = Vector2.one;    // 오른쪽 위
                        //itemRectTransform.sizeDelta = new Vector2(0, 0); // 부모 크기에 꽉 채움

                        // 또는 고정된 크기로 설정 (부모 크기와 무관)
                        itemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 80); // 너비 80
                        itemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80);   // 높이 80

                        // 약간의 패딩을 주려면 (sizeDelta를 음수로 설정)
                        itemRectTransform.sizeDelta = new Vector2(-10, -10); // 좌우상하 5씩 패딩
                    }
                    slot.SetItem(newItemUI);
                    int addedQuantity = Mathf.Min(quantity, itemData.maxStackSize);
                    newItemUI.CurrentQuantity = addedQuantity;

                    quantity -= addedQuantity;

                    slot.UpdateSlotUI(); // UI 업데이트를 위해 추가
                    OnStorageSlotItemUpdated?.Invoke(Array.IndexOf(storageSlots, slot), itemData, slot.myItemUI.CurrentQuantity); // 이벤트 발생

                    Debug.Log($"창고에 '{itemData.itemName}' {slot.myItemUI.CurrentQuantity}개 추가됨.");
                    break; // 다음 아이템 또는 남은 수량 처리
                }
            }
            if (quantity > 0)
            {
                Debug.LogWarning($"창고가 가득 차서 '{itemData.itemName}' {quantity}개를 모두 추가할 수 없었습니다.");
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
            // InventorySlot이 Item 데이터를 직접 가지고 있는지 확인
            // 또는 InventorySlotUI가 연결되어 있고 해당 UI가 Item 데이터를 가지고 있다면 그렇게 접근
            if (slot.myItemData == itemData)
            {
                if (slot.myItemUI != null)
                {
                    count += slot.myItemUI.CurrentQuantity;
                }
                // 만약 myItemUI가 null이지만 myItemData는 할당되어 있다면
                // (데이터는 있는데 UI가 아직 생성되지 않은 경우)
                // InventorySlot 자체에 수량 정보가 있다면 그 정보를 사용해야 합니다.
                // slot.CurrentQuantity 같은 필드가 InventorySlot에 있다면 더 정확할 수 있습니다.
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

        // 스택 가능한 아이템부터 역순으로 제거 (선택 사항: 전략에 따라)
        // 여기서는 가장 앞에 있는 아이템부터 제거하는 것으로 가정합니다.
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
                    // ClearSlot() 호출 후 myItemUI가 null이 될 수 있으므로,
                    // 이벤트 호출 시에는 0을 전달하거나, myItemUI에 접근하지 않도록 합니다.
                    OnStorageSlotItemUpdated?.Invoke(Array.IndexOf(storageSlots, slot), itemData, 0);
                }
                else
                {
                    slot.UpdateSlotUI(); // UI 업데이트
                    OnStorageSlotItemUpdated?.Invoke(Array.IndexOf(storageSlots, slot), itemData, slot.myItemUI.CurrentQuantity); // 이벤트 발생
                }

                

                if (quantityToRemove <= 0) break; // 모두 제거됨
            }
        }

        if (quantityToRemove > 0)
        {
            Debug.LogWarning($"창고에서 '{itemData.itemName}' {quantityToRemove}개를 모두 제거할 수 없었습니다. 충분한 수량이 없습니다.");
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

    // -------- JSON 저장/불러오기 ---------------

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
                    itemData = new ItemSaveData { itemName = slot.myItemData.itemName }, // 아이템 이름만 저장
                    currentQuantity = slot.myItemUI.CurrentQuantity
                };
            }
            // 아이템이 없으면 itemInSlot은 null로 남음
            saveData.slots.Add(slotSaveData);
        }

        string json = JsonUtility.ToJson(saveData, true); // true는 가독성을 위해 예쁘게 포맷팅
        File.WriteAllText(saveFilePath, json);
        Debug.Log("창고 데이터 저장 완료: " + saveFilePath);
    }

    public void LoadStorageData()
    {

        Debug.Log("LoadStorageData() 호출");
        if (File.Exists(saveFilePath))
        {
            Debug.Log("저장파일 존재");
            string json = File.ReadAllText(saveFilePath);
            StorageSaveData loadedData = JsonUtility.FromJson<StorageSaveData>(json);

            // 기존 슬롯 초기화
            foreach (var slot in storageSlots)
            {
                if (slot != null) // null 체크 추가
                {
                    slot.ClearSlot(); // 기존 UI 아이템 제거
                    Debug.Log($"클리어 슬롯 진행: {Array.IndexOf(storageSlots, slot)}번 슬롯 초기화됨."); // 디버그 로그 추가
                }
            }

            for (int i = 0; i < loadedData.slots.Count; i++)
            {

                InventorySlotSaveData slotSaveData = loadedData.slots[i];

                if (i >= storageSlots.Length)
                {
                    Debug.LogWarning("불러온 슬롯 개수가 현재 창고 슬롯 개수보다 많습니다. 초과된 데이터는 무시됩니다.");
                    break;
                }

                
                if (slotSaveData.itemInSlot != null)
                {
                    string itemName = slotSaveData.itemInSlot.itemData.itemName;
                    int quantity = slotSaveData.itemInSlot.currentQuantity;

                    Debug.Log($"[LoadStorageData] 슬롯 {i}: JSON에서 '{itemName}' (수량: {quantity}) 아이템 로드 시도 중.");

                    // 저장된 아이템 이름으로 실제 Item ScriptableObject 찾기
                    if (_allItemsDictionary.TryGetValue(itemName, out Item actualItemData))
                    {
                        Debug.Log($"[LoadStorageData] 슬롯 {i}: _allItemsDictionary에서 '{itemName}' 아이템 찾기 성공!");


                        // 새로운 InventoryItem UI 생성 및 초기화
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
                        Debug.Log($"[LoadStorageData] 슬롯 {i}: UI에 '{itemName}' 아이템 설정 완료.");
                    }
                    else
                    {
                        Debug.LogWarning($"아이템 '{itemName}'을(를) 찾을 수 없습니다. 해당 아이템은 로드되지 않습니다.");
                    }
                }
                else
                {
                    // 슬롯이 비어있으면 ClearSlot()을 호출하여 UI를 비움 (이미 위에서 전체 초기화했지만 명시적으로)
                    Debug.Log($"[LoadStorageData] 슬롯 {i}: 비어있는 슬롯입니다. ClearSlot 호출.");
                    storageSlots[i].ClearSlot();
                    OnStorageSlotItemUpdated?.Invoke(i, null, 0); // 비어있는 슬롯 UI 업데이트 알림
                }
            }
            Debug.Log("Storage 창고 데이터 불러오기 완료.");
        }
        else
        {
            Debug.LogWarning("저장된 창고 데이터 파일이 없습니다. 새로운 창고를 시작합니다.");
        }
    }

    private void LoadItemsFromFolder(string folderPath)
    {
        Item[] itemsInFolder = Resources.LoadAll<Item>(folderPath);
        Debug.Log($"Resources.LoadAll<Item>('{folderPath}') 결과: {itemsInFolder.Length}개의 아이템 발견.");
        foreach (Item item in itemsInFolder)
        {
            if (item == null)
            {
                Debug.LogWarning($"경로 '{folderPath}'에서 null 아이템이 로드되었습니다. 에셋 손상 또는 잘못된 타입일 수 있습니다.");
                continue;
            }

            if (!_allItemsDictionary.ContainsKey(item.itemName))
            {
                _allItemsDictionary.Add(item.itemName, item);
                Debug.Log($"'_allItemsDictionary'에 아이템 추가됨: {item.itemName}");
            }
            else
            {
                Debug.LogWarning($"중복된 아이템 이름이 발견되었습니다: {item.itemName} (경로: {folderPath}). 고유한 이름을 사용하세요.");
            }
        }
        Debug.Log($"'{folderPath}'에서 {itemsInFolder.Length}개의 아이템을 로드했습니다.");
    }
}
