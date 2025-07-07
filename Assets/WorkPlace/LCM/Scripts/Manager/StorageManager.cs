using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DesignPattern;
using UnityEngine.SceneManagement;

public class StorageManager : Singleton<StorageManager>
{
    public static InventoryItem CarriedItem;

    public GameObject inventorySlotPrefab; // 인벤토리 슬롯 프리팹
    public Transform contentParent;         // Scroll Rect의 Content 오브젝트

    public int numberOfSlotsToCreate = 50; // 생성할 슬롯의 개수 (예시)

    public GameObject StorageUIPanel;


    [Header("UI Management")]
    [SerializeField] private Canvas _gameCanvas;
    private const string MAIN_CANVAS_TAG = "StorageUICanvas";

    private List<InventorySlot> generatedStorageSlots = new List<InventorySlot>();

    public Transform draggablesTransform;


    //특정 아이템이 들어갈시 테스트 용도
    public Item Testitem;


    private void Awake()
    {
        base.Awake();


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

        if (_gameCanvas.gameObject.scene.buildIndex != -1)
        {
            DontDestroyOnLoad(_gameCanvas.gameObject);
            Debug.Log($"Inventory: '{MAIN_CANVAS_TAG}' 태그를 가진 _gameCanvas를 DontDestroyOnLoad로 설정했습니다.");
        }
        else
        {
            Debug.Log($"Inventory: '{MAIN_CANVAS_TAG}' 태그를 가진 _gameCanvas가 이미 DontDestroyOnLoad 씬에 있습니다.");

        }

    }
    // Start is called before the first frame update
    void Start()
    {
        if (generatedStorageSlots.Count == 0) // 첫 초기화 시에만 슬롯 생성
        {
            GenerateInventorySlots(numberOfSlotsToCreate);
        }
        else
        {
            Debug.LogError("Storage 인스턴스를 찾을 수 없습니다. Storage 스크립트가 씬에 있는지 확인하세요.");
        }
        CloseStorageUI();
        if (StorageUIPanel != null && StorageUIPanel.transform.parent != _gameCanvas.transform)
        {
            StorageUIPanel.transform.SetParent(_gameCanvas.transform, false);
            RectTransform rectTransform = StorageUIPanel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(StorageUIPanel.GetComponent<RectTransform>());

            Debug.Log("Storage 로직 오브젝트가 Canvas 아래로 이동되었습니다.");
        }
        else if (StorageUIPanel != null && StorageUIPanel.transform.parent == _gameCanvas.transform)
        {
            Debug.Log("StorageUIPanel은 이미 Canvas 아래에 있습니다. 다시 설정할 필요가 없습니다.");
        }
        else if (StorageUIPanel == null)
        {
            Debug.LogError("StorageUIPanel이 할당되지 않았습니다. Inspector에서 할당해주세요!");
        }

        Storage.Instance.AddItemToStorage(Testitem, 1);

        CloseStorageUI();
    }


    private void Update()
    {
        if (CarriedItem == null) return;

        CarriedItem.transform.position = Input.mousePosition;
    }
    public void OpenStorageUI()
    {
        if (StorageUIPanel != null)
        {
            StorageUIPanel.SetActive(true);
            Debug.Log("창고 UI를 열었습니다.");
            // 슬롯 재생성이 필요하다면 여기서 호출 (새로운 아이템이 들어왔을 때 등)
            // GenerateInventorySlots(numberOfSlotsToCreate);
        }
    }

    public void CloseStorageUI()
    {
        if (StorageUIPanel != null)
        {
            StorageUIPanel.SetActive(false);
            Debug.Log("창고 UI를 닫았습니다.");
        }
    }

    public void GenerateInventorySlots(int count)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        generatedStorageSlots.Clear(); // 리스트도 비워줍니다.

        // 새로운 슬롯 생성
        for (int i = 0; i < count; i++)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, contentParent);
            InventorySlot slotComponent = slotGO.GetComponent<InventorySlot>();
            if (slotComponent != null)
            {
                generatedStorageSlots.Add(slotComponent); // 생성된 InventorySlot 컴포넌트 저장
            }
            else
            {
                Debug.LogError("인스턴스화된 슬롯 프리팹에 InventorySlot 컴포넌트가 없습니다!");
            }
        }
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if (CarriedItem != null)
        {
            item.activeSlot.SetItem(CarriedItem);
        }

        CarriedItem = item;
        CarriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
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


            }

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


                    if (CarriedItem.CurrentQuantity <= 0) // 들고 있던 아이템이 모두 스택되었으면
                    {
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


            CarriedItem = null; // 들고 있는 아이템 해제
        }
    }

    private void OnEnable()
    {
        // 씬 로드 이벤트에 등록 (StorageManager가 활성화될 때마다)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트에서 해제 (StorageManager가 비활성화될 때)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"StorageManager: 씬 '{scene.name}' 로드됨.");
        // 각 씬 로드 시마다 UI 슬롯을 재생성하고 Storage에 할당
        // 이 로직은 씬마다 UI가 달라질 경우 유용하며,
        // UI가 동일하다면 단순히 Storage.Instance.LoadStorageData()만 호출해도 됩니다.

        // StorageUI 패널이 비활성화되어 있다면 찾아서 활성화 (UI를 재배치하거나 초기화할 경우)
        if (StorageUIPanel != null && !StorageUIPanel.activeSelf)
        {
            // StorageUIPanel.SetActive(true); // 필요한 경우 UI 활성화
        }

        // GenerateInventorySlots를 다시 호출하여 새로운 슬롯 생성 (만약 씬마다 UI가 달라진다면)
        // 주의: 매 씬마다 슬롯을 새로 만들면 성능 부하가 있을 수 있습니다.
        // UI가 변하지 않는다면 굳이 매번 다시 만들 필요는 없습니다.
        // GenerateInventorySlots(numberOfSlotsToCreate); // 이 부분은 신중하게 결정

        // Storage 인스턴스에 슬롯 정보 전달 (가장 중요)
        if (Storage.Instance != null)
        {
            // 새로 생성된 (또는 재활성화된) 슬롯 배열을 Storage에 전달
            // 이 호출이 LoadStorageData를 다시 트리거합니다.
            Storage.Instance.SetStorageSlots(generatedStorageSlots.ToArray());
            Debug.Log($"StorageManager: 씬 로드 후 Storage 인스턴스에 슬롯 정보 다시 전달 완료. ({scene.name})");
        }
        else
        {
            Debug.LogError($"Storage 인스턴스를 찾을 수 없습니다. 씬 '{scene.name}'에서 Storage 스크립트가 로드되었는지 확인하세요.");
        }
    }
}
