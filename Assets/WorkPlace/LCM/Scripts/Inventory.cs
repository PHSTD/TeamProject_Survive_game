using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DesignPattern;
using System;
using TMPro;
using UnityEngine.SceneManagement;

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

    //[Header("Debug")]
    //[SerializeField] Button giveItemBtn;

    [Header("UI Management")]
    [SerializeField] private GameObject _inventoryUIRootPanel; // 인벤토리 UI의 최상위 GameObject (Panel 등)
    [SerializeField] private Canvas _gameCanvas; // 게임의 메인 Canvas (수동으로 할당하거나 자동으로 찾기)

    [Header("Hotbar Management")]
    [SerializeField] public int _currentHotbarSlotIndex = 0; // 현재 선택된 핫바 슬롯 인덱스 (기본값 0)

    [Header("Item Dropping")]
    [SerializeField] private float _dropDistance = 1.5f; // 플레이어로부터 아이템이 떨어질 거리
    [SerializeField] private LayerMask _groundLayer; // 바닥 레이어
    [SerializeField] private float _scatterForce = 2f;

    // 핫바 슬롯 변경을 외부에 알리는 이벤트 (UI 업데이트 등에 사용)
    public event Action<int> OnHotbarSlotChanged;

    //// 추가: 핫바 슬롯들의 아이템 데이터를 동기화하기 위한 이벤트 (필요하다면)
    public event Action<int, Item, int> OnHotbarSlotItemUpdated;

    private const string MAIN_CANVAS_TAG = "MainUICanvas";



    void Awake()
    {
        base.Awake();

        // Canvas를 찾거나 설정
        //SetupCanvas();

        


        // _gameCanvas가 에디터에서 직접 할당되지 않았다면 태그로 찾기
        if (_gameCanvas == null)
        {
            GameObject canvasGO = GameObject.FindWithTag(MAIN_CANVAS_TAG);
            if (canvasGO != null)
            {
                _gameCanvas = canvasGO.GetComponent<Canvas>();
            }

            if (_gameCanvas == null)
            {
                Debug.LogError($"'{MAIN_CANVAS_TAG}' 태그를 가진 Canvas를 찾을 수 없습니다. 씬에 Canvas가 있는지, 태그가 올바른지 확인해주세요.");
                return;
            }
        }

        // _gameCanvas가 DDOL 씬에 있는지 확인 (최초 1회만 설정)
        if (_gameCanvas.gameObject.scene.buildIndex != -1)
        {
            DontDestroyOnLoad(_gameCanvas.gameObject);
            Debug.Log($"Inventory: '{MAIN_CANVAS_TAG}' 태그를 가진 _gameCanvas를 DontDestroyOnLoad로 설정했습니다.");
        }
        else
        {
            Debug.Log($"Inventory: '{MAIN_CANVAS_TAG}' 태그를 가진 _gameCanvas가 이미 DontDestroyOnLoad 씬에 있습니다.");

        }

        if (_inventoryUIRootPanel != null && _inventoryUIRootPanel.transform.parent != _gameCanvas.transform)
        {
            _inventoryUIRootPanel.transform.SetParent(_gameCanvas.transform, false);
            // UI 위치 및 크기 조정 (필요시)
            RectTransform rectTransform = _inventoryUIRootPanel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
            }
            Debug.Log("인벤토리 UI 패널이 Canvas 아래로 이동되었습니다.");
        }
        

        
        
        // 인벤토리 UI 패널 초기 상태 설정 (처음엔 비활성화)
        if (_inventoryUIRootPanel != null)
        {
            _inventoryUIRootPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Inventory: Inventory UI Root Panel이 할당되지 않았습니다. UI 토글이 작동하지 않을 수 있습니다.");
        }
        

    }
    private void Start()
    {
        if (draggablesTransform == null)
        {
            GameObject draggablesGO = new GameObject("DraggableItems");
            draggablesTransform = draggablesGO.transform;
            draggablesTransform.SetParent(_gameCanvas.transform, false);
            // 최상위 레이어에 표시되도록 설정
            RectTransform dragRect = draggablesGO.AddComponent<RectTransform>();
            dragRect.anchorMin = Vector2.zero;
            dragRect.anchorMax = Vector2.one;
            dragRect.offsetMin = Vector2.zero;
            dragRect.offsetMax = Vector2.zero;
            draggablesTransform.SetAsLastSibling();
            Debug.Log("DraggableItems 컨테이너가 생성되고 Canvas 아래에 배치되었습니다.");
        }
        else if (draggablesTransform.parent != _gameCanvas.transform)
        {
            draggablesTransform.SetParent(_gameCanvas.transform, false);
            draggablesTransform.SetAsLastSibling();
        }

        draggablesTransform.SetAsLastSibling();
        Debug.Log("DraggableItems 컨테이너가 Canvas의 최상위 형제로 설정되었습니다.");

        // 초기 핫바 슬롯 선택을 알림 (기존 Awake에 있었다면 Start로 이동)
        OnHotbarSlotChanged?.Invoke(_currentHotbarSlotIndex);

        // 상시 핫바 슬롯 배열 초기화 및 동기화 (기존 Awake에 있었다면 Start로 이동)
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
            // 핫바 선택이 변경되었음을 외부에 알립니다.
            OnHotbarSlotChanged?.Invoke(_currentHotbarSlotIndex);
            Debug.Log($"핫바 슬롯 {index + 1}번이 선택되었습니다.");
        }
        else
        {
            Debug.LogWarning($"유효하지 않은 핫바 슬롯 인덱스: {index}. 핫바 슬롯 범위는 0에서 {hotbarSlots.Length - 1}입니다.");
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
        Debug.Log($"[Inventory] SpawnInventoryItem 호출됨. 추가 시도 아이템: {item.itemName}");
        // 스택 가능한 아이템인 경우, 먼저 기존 슬롯을 확인
        if (item.isStackable)
        {
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                if (hotbarSlots[i].myItemData == item && hotbarSlots[i].myItemUI != null && hotbarSlots[i].myItemUI.CurrentQuantity < item.maxStackSize)
                {
                    hotbarSlots[i].myItemUI.CurrentQuantity++; // 수량 증가
                    SyncHotbarSlotUI(i); // 핫바 동기화 (이 내부에서 SetItemInternal을 통해 UI 업데이트)
                    Debug.Log($"SUCCESS: '{item.itemName}' stacked in hotbar slot {i}. New Qty: {hotbarSlots[i].myItemUI.CurrentQuantity}");
                    return;
                }
            }
            // === 인벤토리 슬롯 확인 (스택 로직) ===
            foreach (var slot in inventorySlots)
            {
                if (slot.myItemData == item && slot.myItemUI != null && slot.myItemUI.CurrentQuantity < item.maxStackSize)
                {
                    slot.myItemUI.CurrentQuantity++; // 수량 증가
                    Debug.Log($"SUCCESS: '{item.itemName}' stacked in inventory slot {slot.name}. New Qty: {slot.myItemUI.CurrentQuantity}");
                    return; // 아이템 추가 완료
                }
            }
        }

        // 스택 불가능하거나 꽉 찼을 경우, 빈 슬롯에 생성
        //먼저 핫바의 빈 슬롯 확인(두 배열 모두)
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].myItemUI == null) // 빈 핫바 슬롯을 찾음
            {
                var newItemUI = Instantiate(itemPrefab, hotbarSlots[i].transform);
                newItemUI.Initialize(item, hotbarSlots[i]); // 인벤토리 핫바에 UI 생성
                // 초기 수량이 1이 아니라면 여기서 설정
                newItemUI.CurrentQuantity = 1; // 기본적으로 1이므로 생략 가능
                SyncHotbarSlotUI(i); // 핫바 동기화
                Debug.Log($"새 핫바 슬롯 {i}에 '{item.itemName}' 추가.");
                return;
            }
        }

        // 스택할 수 없거나, 스택 가능한 아이템이지만 모든 기존 슬롯이 꽉 찼을 경우
        // 비어있는 새 슬롯에 아이템을 생성
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItemUI == null) // 비어있는 슬롯을 찾음
            {
                var newItemUI = Instantiate(itemPrefab, inventorySlots[i].transform);
                newItemUI.Initialize(item, inventorySlots[i]); // Initialize 호출
                Debug.Log($"새 슬롯에 '{item.itemName}' 추가.");
                return; // 아이템 추가 완료
            }
        }

        Debug.LogWarning($"인벤토리가 가득 찼습니다. '{item.itemName}'을(를) 추가할 수 없습니다.");


    }
    public void DropItemFromSlot(InventoryItem itemToDropUI)
    {
        if (itemToDropUI == null || itemToDropUI.myItem == null)
        {
            Debug.LogWarning("유효하지 않은 아이템을 버리려고 시도했습니다.");
            CarriedItem = null; // 혹시 모를 상황 대비
            return;
        }

        GameObject itemWorldPrefab = itemToDropUI.myItem.WorldPrefab;
        if (itemWorldPrefab == null)
        {
            Debug.LogWarning($"아이템 '{itemToDropUI.myItem.itemName}'에 연결된 3D 월드 프리팹이 없습니다. 버릴 수 없습니다.", itemToDropUI.myItem);
            // 아이템 프리팹이 없어도 인벤토리에서는 지워야 하므로 아래 ClearSlot() 로직은 진행합니다.
        }
        else
        {
            //드롭할 아이템의 수량
            int quantityToDrop = itemToDropUI.CurrentQuantity;
            Item droppedItemData = itemToDropUI.myItem;

            //아이템이 떨어질 위치를 계산 (플레이어 전방)
            Transform playerTransform = PlayerManager.Instance.Player.transform; // PlayerController의 transform
            Vector3 playerForward = playerTransform.forward;
            Vector3 dropPosition = playerTransform.position + playerTransform.forward * _dropDistance;
            dropPosition.y += 0.5f;

            RaycastHit hit;
            if (Physics.Raycast(dropPosition + Vector3.up * 10f, Vector3.down, out hit, 20f, _groundLayer))
            {
                dropPosition.y = hit.point.y + 0.1f;
            }

            // 수량만큼 월드 아이템 개별 생성
            for (int i = 0; i < quantityToDrop; i++)
            {
                // 아이템이 떨어질 위치를 조금씩 다르게 하여 겹치지 않게 함
                Vector3 scatteredPosition = dropPosition;
                // 랜덤한 수평 방향으로 살짝 퍼지게 함
                scatteredPosition.x += UnityEngine.Random.Range(-0.5f, 0.5f);
                scatteredPosition.z += UnityEngine.Random.Range(-0.5f, 0.5f);
                scatteredPosition.y += UnityEngine.Random.Range(0f, 0.2f); // 높이도 살짝 다르게

                GameObject worldItemGO = Instantiate(itemWorldPrefab, scatteredPosition, Quaternion.identity);
                WorldItem worldItemScript = worldItemGO.GetComponent<WorldItem>();

                if (worldItemScript != null)
                {
                    // WorldItem의 Initialize 메서드를 호출하여 아이템 데이터만 전달 (수량은 1개로 가정)
                    worldItemScript.Initialize(droppedItemData);

                    // Rigidbody가 있다면 힘을 가해서 좀 더 자연스럽게 퍼지게 할 수 있음
                    Rigidbody rb = worldItemGO.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
                        rb.AddForce(randomDirection * _scatterForce, ForceMode.Impulse);
                    }
                }
                else
                {
                    Debug.LogError($"드롭된 월드 프리팹 '{itemWorldPrefab.name}'에 WorldItem 스크립트가 없습니다!");
                }
            }
            // 인벤토리 슬롯에서 아이템을 제거하고 UI 인스턴스 파괴
            bool wasHotbarItem = false;
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                if (hotbarSlots[i] == itemToDropUI.activeSlot)
                {
                    hotbarSlots[i].ClearSlot(); 
                    SyncHotbarSlotUI(i); // 핫바 동기화
                    wasHotbarItem = true;
                    break;
                }
            }

            if (!wasHotbarItem && itemToDropUI.activeSlot != null) // 일반 인벤토리 슬롯에서 버려진 경우
            {
                itemToDropUI.activeSlot.ClearSlot();
            }
            else if (itemToDropUI.activeSlot == null) // 슬롯에 없던 아이템이 버려진 경우 (예: 드래그 중 월드 밖으로 버림)
            {
                Destroy(itemToDropUI.gameObject);
            }

            CarriedItem = null;
        }

        //인벤토리 슬롯에서 아이템을 제거하고 UI 인스턴스 파괴
        if (itemToDropUI.activeSlot != null)
        {
            itemToDropUI.activeSlot.ClearSlot(); // 해당 슬롯을 비움 (데이터 및 UI 참조 제거)
            Debug.Log($"아이템 '{itemToDropUI.myItem.itemName}'이(가) 슬롯에서 버려졌습니다.");
        }
        else
        {
            // 슬롯에 할당되지 않은 아이템(예: 드래그 도중 생성된 아이템이 버려진 경우)
            Destroy(itemToDropUI.gameObject);
        }

        // CarriedItem을 비웁니다.
        CarriedItem = null;
    }

    public int GetItemCount(Item item)
    {
        int count = 0;
        foreach(var slot in inventorySlots)
        {
            if(slot.myItemData == item) 
            {
                count += slot.myItemUI.CurrentQuantity; 
            }
        }
        foreach(var slot in hotbarSlots)
        {
            if(slot.myItemData == item)
            {
                count += slot.myItemUI.CurrentQuantity;
            }
        }

        return count;
    }

    public void RemoveItem(Item itemToRemove, int amount = 1)
    {
        int removedCount = 0;

        // 핫바 슬롯에서 제거
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].myItemData == itemToRemove)
            {
                // 스택 가능한 아이템이고, 남은 수량이 제거할 수량보다 많으면 수량만 감소
                if (itemToRemove.isStackable && hotbarSlots[i].myItemUI.CurrentQuantity > amount - removedCount)
                {
                    hotbarSlots[i].myItemUI.CurrentQuantity -= (amount - removedCount);
                    removedCount = amount; // 모두 제거된 것으로 간주
                    break;
                }
                else // 스택 불가능하거나, 남은 수량이 제거할 수량 이하이면 슬롯 비움
                {
                    int currentStack = hotbarSlots[i].myItemUI.CurrentQuantity;
                    hotbarSlots[i].ClearSlot();
                    removedCount += currentStack;
                    SyncHotbarSlotUI(i);
                    if (removedCount >= amount) break;
                }
            }
        }

        // 인벤토리 슬롯에서 제거 (핫바에서 전부 제거되지 않은 경우)
        if (removedCount < amount)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].myItemData == itemToRemove)
                {
                    // 스택 가능한 아이템이고, 남은 수량이 제거할 수량보다 많으면 수량만 감소
                    if (itemToRemove.isStackable && inventorySlots[i].myItemUI.CurrentQuantity > amount - removedCount)
                    {
                        inventorySlots[i].myItemUI.CurrentQuantity -= (amount - removedCount);
                        removedCount = amount;
                        break;
                    }
                    else // 스택 불가능하거나, 남은 수량이 제거할 수량 이하이면 슬롯 비움
                    {
                        int currentStack = inventorySlots[i].myItemUI.CurrentQuantity;
                        inventorySlots[i].ClearSlot();
                        removedCount += currentStack;
                        if (removedCount >= amount) break;
                    }
                }
            }
        }

        Debug.Log($"{itemToRemove.name} {removedCount}개를 인벤토리에서 제거했습니다.");
        if (removedCount < amount)
        {
            Debug.LogWarning($"요청한 {amount}개 중 {amount - removedCount}개를 제거하지 못했습니다. 아이템 부족.");
        }
    }

    // 새롭게 추가된 메서드: 상시 핫바 슬롯 초기화 및 동기화
    private void InitializePersistentHotbarSlots()
    {
        if (hotbarSlots == null || persistentHotbarSlots == null)
        {
            Debug.LogError("Hotbar slots arrays are not assigned!");
            return;
        }

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            // persistentHotbarSlots[i] 슬롯 자체의 InventoryItem UI를 가져오거나, 없다면 생성
            // 이 예시에서는 persistentHotbarSlots[i] 밑에 InventoryItem 컴포넌트가 있다고 가정합니다.
            // 만약 없다면, 여기에 미리 할당된 InventoryItem GameObject를 활성화/비활성화하는 로직이 필요합니다.

            InventoryItem persistentItemUI = persistentHotbarSlots[i].GetComponentInChildren<InventoryItem>(true); // 비활성화된 자식도 찾기
            if (persistentItemUI == null)
            {
                // 만약 persistentItemUI가 미리 할당되지 않았다면 여기서 생성 (한번만)
                // 이 부분은 InitializePersistentHotbarSlots()가 아니라, 씬 로드 시 persistentHotbarSlots 자체를 구성할 때 하는 것이 좋습니다.
                // 이 예시에서는 슬롯에 이미 InventoryItem이 있다고 가정합니다.
                // 또는 persistentHotbarSlots[i]의 자식으로 InventoryItem 프리팹을 Instantiate하고,
                // 그 InventoryItem을 persistentItemUI로 할당하는 방식으로 구현할 수 있습니다.
                Debug.LogWarning($"Persistent Hotbar Slot {i}에 InventoryItem이 없습니다. 프리팹 설정 확인.");
                if (hotbarSlots[i].myItemUI != null)
                {
                    // 만약 persistentItemUI가 없다면 새로 생성해야 합니다.
                    // 이 로직은 한번만 실행되도록 (예: 씬 로드 시) 주의해야 합니다.
                    persistentItemUI = Instantiate(itemPrefab, persistentHotbarSlots[i].transform);
                }
                else
                {
                    persistentHotbarSlots[i].ClearSlot();
                    if (persistentItemUI != null) Destroy(persistentItemUI.gameObject); // 비어있는데 UI가 남아있으면 파괴
                    continue;
                }
            }

            if (hotbarSlots[i].myItemUI != null)
            {
                // 원본 핫바 슬롯의 아이템 데이터를 상시 핫바 슬롯의 UI로 업데이트
                persistentItemUI.Initialize(hotbarSlots[i].myItemUI.myItem, persistentHotbarSlots[i]);
                persistentItemUI.CurrentQuantity = hotbarSlots[i].myItemUI.CurrentQuantity;
                persistentItemUI.gameObject.SetActive(true); // 활성화
                persistentHotbarSlots[i].SetItem(persistentItemUI); // 슬롯 데이터도 업데이트
            }
            else
            {
                // 원본 핫바 슬롯이 비어있다면, persistent 핫바 슬롯을 비움
                persistentHotbarSlots[i].ClearSlot();
                if (persistentItemUI != null)
                {
                    persistentItemUI.gameObject.SetActive(false); // 비활성화
                                                                  // persistentItemUI.ClearData(); // 필요하다면 내부 데이터도 초기화하는 메서드 호출
                }
            }
        }
    }

    // 새롭게 추가된 메서드: 핫바 슬롯 변경 시 동기화
    private void SyncHotbarSlotUI(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length || index >= persistentHotbarSlots.Length) return;

        

        // UI 인스턴스 동기화 (기존 UI 파괴 후 새로 생성 또는 업데이트)
        if (persistentHotbarSlots[index].myItemUI != null)
        {
            Destroy(persistentHotbarSlots[index].myItemUI.gameObject);
            persistentHotbarSlots[index].myItemUI = null;
        }

        // 원본 핫바 슬롯의 아이템 데이터를 상시 핫바 슬롯으로 복사
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

        // 아이템을 들고 있고, 빈 슬롯에 드롭하는 경우 (클릭은 아님)
        // (CarriedItem != null && targetSlot.myItemUI == null)
        else if (targetSlot.myItemUI == null)
        {
            Debug.Log("HangleItem: 빈 슬롯에 내려놓기 (드롭)");
            InventorySlot originalSlot = CarriedItem.activeSlot;

            targetSlot.SetItem(CarriedItem);


            //이전 슬롯 지우기
            if (originalSlot != null)
            {
                //originalSlot.ClearSlot(); // <-- 이 라인을 주석 처리하거나 제거해야 합니다!
                originalSlot.myItemData = null; // 원본 슬롯의 데이터만 비웁니다.
                originalSlot.myItemUI = null;   // 원본 슬롯의 UI 참조만 비웁니다.


                CheckAndSyncSlotIfHotbar(originalSlot); // 핫바라면 동기화
            }

            // 핫바라면 동기화
            CheckAndSyncSlotIfHotbar(targetSlot);
            CarriedItem = null; // 들고 있는 아이템 해제
        }
        // 아이템을 들고 있고, 아이템이 있는 슬롯에 드롭하는 경우 (클릭은 아님)
        else // (CarriedItem != null && targetSlot.myItemUI != null)
        {
            // 같은 아이템이고 스택 가능하다면 스택 시도
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

                    // 동기화
                    CheckAndSyncSlotIfHotbar(targetSlot);
                    if (CarriedItem.CurrentQuantity <= 0) // 들고 있던 아이템이 모두 스택되었으면
                    {
                        if (CarriedItem.activeSlot != null) // 원래 슬롯이 있었다면
                        {
                            //CarriedItem.activeSlot.ClearSlot(); // 원래 슬롯 비움
                            CheckAndSyncSlotIfHotbar(CarriedItem.activeSlot); // 핫바라면 동기화
                        }
                        Destroy(CarriedItem.gameObject); // 들고 있던 아이템 UI 파괴
                        CarriedItem = null; // 들고 있던 아이템 해제
                    }
                    // else: 들고 있던 아이템이 남아있으면, CarriedItem은 계속 마우스에 붙어있으므로 별도 처리 불필요.
                    return; // 스택 완료 후 함수 종료
                }
            }

            // 스택 불가능하거나 스택 공간이 없으면 아이템 교환
            Debug.Log("HangleItem: 아이템 교환");
            InventoryItem tempCarriedItem = CarriedItem;
            InventoryItem tempTargetItem = targetSlot.myItemUI;

            // 원본 슬롯에 대상 아이템을 놓기
            if (tempCarriedItem.activeSlot != null)
            {
                //tempCarriedItem.activeSlot.ClearSlot(); // 이전 슬롯 비움
                CheckAndSyncSlotIfHotbar(tempCarriedItem.activeSlot);

                tempCarriedItem.activeSlot.SetItem(tempTargetItem); // 이전 슬롯에 대상 아이템 놓기
                CheckAndSyncSlotIfHotbar(tempCarriedItem.activeSlot);
            }
            else // 드래그 중이던 아이템이 원본 슬롯이 없었을 경우 (예: 새로 생성된 아이템이 바로 드래그된 경우)
            {
                // 원래 있던 아이템 UI를 파괴
                targetSlot.ClearSlot(); // 대상 슬롯을 비움 (tempTargetItem UI는 파괴됨)
                                        // tempTargetItem.activeSlot은 이미 targetSlot이므로 중복 Clear 불필요
            }

            // 대상 슬롯에 들고 있던 아이템을 놓음
            targetSlot.SetItem(tempCarriedItem);
            CheckAndSyncSlotIfHotbar(targetSlot);

            CarriedItem = null; // 들고 있는 아이템 해제
        }
    }

    // 주어진 슬롯이 핫바 슬롯 중 하나인지 확인하고, 그렇다면 SyncHotbarSlotUI를 호출
    public void CheckAndSyncSlotIfHotbar(InventorySlot slot)
    {
        Debug.Log("핫바슬롯인지 체크");
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i] == slot)
            {
                Debug.Log("핫바슬롯이 맞데유");
                SyncHotbarSlotUI(i);
                return;
            }
        }
    }

    void OnEnable()
    {
        // 씬 로드 이벤트를 구독합니다.
        // 이 이벤트는 새 씬이 로드된 후에 호출됩니다.
        Debug.Log("Inventory: OnEnable 호출됨. SceneManager.sceneLoaded 이벤트 구독.");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 스크립트가 비활성화될 때 이벤트 구독을 해제합니다. (메모리 누수 방지)
        Debug.Log("Inventory: OnDisable 호출됨. SceneManager.sceneLoaded 이벤트 구독 해제.");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬이 로드될 때마다 이 함수가 호출됩니다.
        Debug.Log($"Inventory: 새로운 씬이 로드되었습니다: {scene.name}");

        if (Storage.Instance == null)
        {
            // 이 로그가 뜬다면, Storage가 Inventory보다 늦게 초기화된 것입니다.
            Debug.LogError("Inventory: Storage.Instance가 OnSceneLoaded 시점에 아직 초기화되지 않았습니다! 아이템 이동 불가.");
            // 여기서 return하면 MoveAllInventoryItemsToStorage()는 호출되지 않습니다.
            // Script Execution Order를 조정하여 Storage가 먼저 초기화되도록 해야 합니다.
            return;
        }

        Debug.Log("Inventory: Storage.Instance가 준비되었습니다. MoveAllInventoryItemsToStorage() 호출 시도.");
        // 여기에 씬 전환 시 아이템을 Storage로 보내는 로직을 호출합니다.
        // 단, Storage가 먼저 초기화되어 Instance를 사용할 수 있는지 확인해야 합니다.
        // Start()나 Awake()에서 Storage.Instance에 접근하면 안전합니다.
        MoveAllInventoryItemsToStorage();

        // 인벤토리 UI를 닫아줍니다 (선택 사항).
        if (_inventoryUIRootPanel != null && _inventoryUIRootPanel.activeSelf)
        {
            _inventoryUIRootPanel.SetActive(false);
        }
    }


    // 인벤토리의 모든 아이템을 Storage로 보내는 메서드
    public void MoveAllInventoryItemsToStorage()
    {
        Debug.Log("Inventory: MoveAllInventoryItemsToStorage() 시작.");

        if (Storage.Instance == null)
        {
            Debug.LogError("Storage.Instance가 아직 초기화되지 않았습니다. 인벤토리 아이템을 보낼 수 없습니다.");
            return;
        }

        // 인벤토리 슬롯의 아이템들을 Storage로 보냅니다.
        // hotbarSlots, persistentHotbarSlots, inventorySlots 모두 순회합니다.

        //Hotbar 슬롯 처리
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].myItemData != null && hotbarSlots[i].myItemUI != null)
            {
                Item itemData = hotbarSlots[i].myItemData;
                InventoryItem itemUI = hotbarSlots[i].myItemUI;
                int quantity = hotbarSlots[i].myItemUI.CurrentQuantity;
                switch (itemData) // itemData의 타입에 따라 분기
                {
                    case AirTankItem airTank:
                        Debug.Log($"Inventory: 핫바 슬롯 {i}의 '{airTank.itemName}' ({quantity}개) 공기 회복 후 파괴 시도.");
                        for (int k = 0; k < quantity; k++)
                        {
                            //airTank.Use(PlayerManager.Instance.Player); // AirTankItem 사용
                            Debug.Log("에어탱크사용");
                        }
                        hotbarSlots[i].ClearSlot();
                        SyncHotbarSlotUI(i);
                        Destroy(itemUI.gameObject);
                        Debug.Log($"핫바 슬롯 {i}의 '{airTank.itemName}' {quantity}개가 사용 및 파괴되었습니다.");
                        break; // case 문 종료

                    case BatteryPackItem batteryPack: // BatteryPackItem 추가
                        Debug.Log($"Inventory: 핫바 슬롯 {i}의 '{batteryPack.itemName}' ({quantity}개) 배터리 충전 후 파괴 시도.");
                        for (int k = 0; k < quantity; k++)
                        {
                            // batteryPack.Use(PlayerManager.Instance.Player); // 실제 BatteryPackItem 사용 로직 호출
                            Debug.Log("배터리팩 사용 (핫바)"); // 임시 로그
                        }
                        hotbarSlots[i].ClearSlot();
                        SyncHotbarSlotUI(i);
                        Destroy(itemUI.gameObject);
                        Debug.Log($"핫바 슬롯 {i}의 '{batteryPack.itemName}' {quantity}개가 사용 및 파괴되었습니다.");
                        break; // case 문 종료

                    case ToolItem toolItem:
                        Debug.Log($"Inventory: 핫바 슬롯 {i}의 '{toolItem.itemName}' ({quantity}개) 창고로 이동 대신 파괴 시도.");
                        hotbarSlots[i].ClearSlot();
                        SyncHotbarSlotUI(i);
                        Destroy(itemUI.gameObject);
                        Debug.Log($"핫바 슬롯 {i}의 '{toolItem.itemName}' {quantity}개가 파괴되었습니다.");
                        break; // case 문 종료

                    default: // 위에 해당하는 타입이 없을 경우 (기존 else와 동일)
                        Debug.Log($"Inventory: 핫바 슬롯 {i}의 '{itemData.itemName}' ({quantity}개) 창고로 이동 시도.");
                        Storage.Instance.AddItemToStorage(itemData, quantity);
                        hotbarSlots[i].ClearSlot();
                        SyncHotbarSlotUI(i);
                        Debug.Log($"핫바 슬롯 {i}의 '{itemData.itemName}' {quantity}개를 창고로 보냈습니다.");
                        break; // default 문 종료
                }
            }
            else
            {
                Debug.Log($"Inventory: 핫바 슬롯 {i}는 비어있습니다. (데이터: {hotbarSlots[i].myItemData != null}, UI: {hotbarSlots[i].myItemUI != null})");
            }
        }


        // 3. Main Inventory 슬롯 처리
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItemData != null && inventorySlots[i].myItemUI != null)
            {
                Item itemData = inventorySlots[i].myItemData;
                InventoryItem itemUI = inventorySlots[i].myItemUI;
                int quantity = inventorySlots[i].myItemUI.CurrentQuantity;
                switch (itemData) // itemData의 타입에 따라 분기합니다.
                {
                    case AirTankItem airTank:
                        Debug.Log($"Inventory: 인벤토리 슬롯 {i}의 '{airTank.itemName}' ({quantity}개) 공기 회복 후 파괴 시도.");
                        for (int k = 0; k < quantity; k++)
                        {
                            //airTank.Use(PlayerManager.Instance.Player); // AirTankItem 사용
                            Debug.Log("에어탱크사용");
                        }
                        inventorySlots[i].ClearSlot(); // 인벤토리 슬롯을 비웁니다.
                        Destroy(itemUI.gameObject); // UI 인스턴스를 파괴합니다.
                        Debug.Log($"인벤토리 슬롯 {i}의 '{airTank.itemName}' {quantity}개가 사용 및 파괴되었습니다.");
                        break; // case 문 종료

                    case BatteryPackItem batteryPack: // BatteryPackItem 추가
                        Debug.Log($"Inventory: 인벤토리 슬롯 {i}의 '{batteryPack.itemName}' ({quantity}개) 배터리 충전 후 파괴 시도.");
                        for (int k = 0; k < quantity; k++)
                        {
                            // batteryPack.Use(PlayerManager.Instance.Player); // 실제 BatteryPackItem 사용 로직 호출
                            Debug.Log("배터리팩 사용 (인벤토리)"); // 임시 로그
                        }
                        inventorySlots[i].ClearSlot();
                        Destroy(itemUI.gameObject);
                        Debug.Log($"인벤토리 슬롯 {i}의 '{batteryPack.itemName}' {quantity}개가 사용 및 파괴되었습니다.");
                        break;

                    case ToolItem toolItem:
                        Debug.Log($"Inventory: 인벤토리 슬롯 {i}의 '{toolItem.itemName}' ({quantity}개) 창고로 이동 대신 파괴 시도.");
                        inventorySlots[i].ClearSlot(); // 인벤토리 슬롯을 비웁니다.
                        Destroy(itemUI.gameObject); // UI 인스턴스를 파괴합니다.
                        Debug.Log($"인벤토리 슬롯 {i}의 '{toolItem.itemName}' {quantity}개가 파괴되었습니다.");
                        break; // case 문 종료

                    default: // 위에 해당하는 타입이 없을 경우 (기존 else와 동일)
                        Debug.Log($"Inventory: 인벤토리 슬롯 {i}의 '{itemData.itemName}' ({quantity}개) 창고로 이동 시도.");
                        Storage.Instance.AddItemToStorage(itemData, quantity);
                        inventorySlots[i].ClearSlot(); // 인벤토리 슬롯 비우기
                        Debug.Log($"인벤토리 슬롯 {i}의 '{itemData.itemName}' {quantity}개를 창고로 보냈습니다.");
                        break; // default 문 종료
                }
            }
        }

        // 혹시 드래그 중인 아이템이 있다면 파괴합니다.
        if (CarriedItem != null)
        {
            Destroy(CarriedItem.gameObject);
            CarriedItem = null;
            Debug.Log("드래그 중인 아이템을 파괴했습니다.");
        }

        Debug.Log("인벤토리의 모든 아이템을 창고로 이동 완료했습니다.");
    }
}
