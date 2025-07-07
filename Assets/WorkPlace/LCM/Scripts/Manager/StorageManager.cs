using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DesignPattern;
using UnityEngine.SceneManagement;

public class StorageManager : Singleton<StorageManager>
{
    public static InventoryItem CarriedItem;

    public GameObject inventorySlotPrefab; // �κ��丮 ���� ������
    public Transform contentParent;         // Scroll Rect�� Content ������Ʈ

    public int numberOfSlotsToCreate = 50; // ������ ������ ���� (����)

    public GameObject StorageUIPanel;


    [Header("UI Management")]
    [SerializeField] private Canvas _gameCanvas;
    private const string MAIN_CANVAS_TAG = "StorageUICanvas";

    private List<InventorySlot> generatedStorageSlots = new List<InventorySlot>();

    public Transform draggablesTransform;


    //Ư�� �������� ���� �׽�Ʈ �뵵
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
                Debug.LogError($"'{MAIN_CANVAS_TAG}' �±׸� ���� Canvas�� ã�� �� �����ϴ�. ���� Canvas�� �ִ���, �±װ� �ùٸ��� Ȯ�����ּ���.");
                return;
            }
        }

        if (_gameCanvas.gameObject.scene.buildIndex != -1)
        {
            DontDestroyOnLoad(_gameCanvas.gameObject);
            Debug.Log($"Inventory: '{MAIN_CANVAS_TAG}' �±׸� ���� _gameCanvas�� DontDestroyOnLoad�� �����߽��ϴ�.");
        }
        else
        {
            Debug.Log($"Inventory: '{MAIN_CANVAS_TAG}' �±׸� ���� _gameCanvas�� �̹� DontDestroyOnLoad ���� �ֽ��ϴ�.");

        }

    }
    // Start is called before the first frame update
    void Start()
    {
        if (generatedStorageSlots.Count == 0) // ù �ʱ�ȭ �ÿ��� ���� ����
        {
            GenerateInventorySlots(numberOfSlotsToCreate);
        }

        if (Storage.Instance != null)
        {
            Storage.Instance.SetStorageSlots(generatedStorageSlots.ToArray());
            Debug.Log("StorageManager: Start()���� Storage �ν��Ͻ��� ���� ���� ���� �Ϸ�.");
        }
        else
        {
            Debug.LogError("Storage �ν��Ͻ��� ã�� �� �����ϴ�. Storage ��ũ��Ʈ�� ���� �ִ��� Ȯ���ϼ���.");
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

            Debug.Log("Storage ���� ������Ʈ�� Canvas �Ʒ��� �̵��Ǿ����ϴ�.");
        }
        else if (StorageUIPanel != null && StorageUIPanel.transform.parent == _gameCanvas.transform)
        {
            Debug.Log("StorageUIPanel�� �̹� Canvas �Ʒ��� �ֽ��ϴ�. �ٽ� ������ �ʿ䰡 �����ϴ�.");
        }
        else if (StorageUIPanel == null)
        {
            Debug.LogError("StorageUIPanel�� �Ҵ���� �ʾҽ��ϴ�. Inspector���� �Ҵ����ּ���!");
        }

        //Ư�� �������� ���� �׽�Ʈ��
        //Debug.Log("ä�����߰�");
        
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
            Debug.Log("â�� UI�� �������ϴ�.");
            // ���� ������� �ʿ��ϴٸ� ���⼭ ȣ�� (���ο� �������� ������ �� ��)
            // GenerateInventorySlots(numberOfSlotsToCreate);
        }
    }

    public void CloseStorageUI()
    {
        if (StorageUIPanel != null)
        {
            StorageUIPanel.SetActive(false);
            Debug.Log("â�� UI�� �ݾҽ��ϴ�.");
        }
    }

    public void GenerateInventorySlots(int count)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        generatedStorageSlots.Clear(); // ����Ʈ�� ����ݴϴ�.

        // ���ο� ���� ����
        for (int i = 0; i < count; i++)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, contentParent);
            InventorySlot slotComponent = slotGO.GetComponent<InventorySlot>();
            if (slotComponent != null)
            {
                generatedStorageSlots.Add(slotComponent); // ������ InventorySlot ������Ʈ ����
            }
            else
            {
                Debug.LogError("�ν��Ͻ�ȭ�� ���� �����տ� InventorySlot ������Ʈ�� �����ϴ�!");
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

        // �������� ��� �ְ�, �� ���Կ� ����ϴ� ��� (Ŭ���� �ƴ�)
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


            }

            CarriedItem = null; // ��� �ִ� ������ ����
        }
        // �������� ��� �ְ�, �������� �ִ� ���Կ� ����ϴ� ��� (Ŭ���� �ƴ�)
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


                    if (CarriedItem.CurrentQuantity <= 0) // ��� �ִ� �������� ��� ���õǾ�����
                    {
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


            CarriedItem = null; // ��� �ִ� ������ ����
        }
    }

    private void OnEnable()
    {
        // �� �ε� �̺�Ʈ�� ��� (StorageManager�� Ȱ��ȭ�� ������)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // �� �ε� �̺�Ʈ���� ���� (StorageManager�� ��Ȱ��ȭ�� ��)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"StorageManager: �� '{scene.name}' �ε��.");

        if (StorageUIPanel != null && !StorageUIPanel.activeSelf)
        {

        }


        if (Storage.Instance != null)
        {
            Storage.Instance.SetStorageSlots(generatedStorageSlots.ToArray());
            Debug.Log($"StorageManager: �� �ε� �� Storage �ν��Ͻ��� ���� ���� �ٽ� ���� �Ϸ�. ({scene.name})");
        }
        else
        {
            Debug.LogError($"Storage �ν��Ͻ��� ã�� �� �����ϴ�. �� '{scene.name}'���� Storage ��ũ��Ʈ�� �ε�Ǿ����� Ȯ���ϼ���.");
        }
    }
}