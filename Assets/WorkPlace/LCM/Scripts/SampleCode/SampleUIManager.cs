using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using System;
using TMPro;

public class SampleUIManager : Singleton<SampleUIManager>
{
    
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;

    [Header("Hotbar UI")]
    [SerializeField] private GameObject[] hotbarSelectionIndicators;

    [Header("Crafting UI")]
    [SerializeField] private GameObject craftingPanel; // ���� UI�� �ֻ��� �г�

    [SerializeField] private GameObject sceneSpecificUIRoot;

    public GameObject inventoryPanel;

    private int _currentSelectedHotbarIndex = -1;

    public event Action<bool> OnInventoryUIToggled;

    public event Action<bool> OnCraftingUIToggled;

    

    private void Awake()
    {
        SingletonInit();
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false); // �κ��丮 UI�� ���� �� ��Ȱ��ȭ
        }
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(false);
        }

        SetItemDescription(""); // ������ ���� �ʱ�ȭ

        // Inventory�� �ֹ� ���� �̺�Ʈ ����
        Inventory.Instance.OnHotbarSlotChanged += OnHotbarSlotSelectionChanged;
        Inventory.Instance.OnHotbarSlotItemUpdated += OnHotbarSlotItemContentUpdated;

        if (sceneSpecificUIRoot != null)
        {
            sceneSpecificUIRoot.SetActive(false);
            Debug.Log($"�� ���� UI ��Ʈ '{sceneSpecificUIRoot.name}'�� ��Ȱ��ȭ�߽��ϴ�.");
        }

        // �ʱ� �ֹ� ���� ���� UI ������Ʈ (Awake���� Inventory�� �ʱ�ȭ�� �� ȣ��)
        // Inventory���� _currentHotbarSlotIndex�� �ʱ�ȭ�ϹǷ�, �� ���� �ݿ��ؾ� ��
        UpdateHotbarSelectionUI(Inventory.Instance._currentHotbarSlotIndex);
    }
    public void ToggleInventoryUI()
    {
        bool currentStatus = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(currentStatus);

        OnInventoryUIToggled?.Invoke(currentStatus);

        // Ŀ�� �� �÷��̾� ���� ����
        if (currentStatus)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // PlayerController.Instance.SetCanMove(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // PlayerController.Instance.SetCanMove(true);
        }
    }

    public void ToggleCraftingUI()
    {
        bool currentStatus = !craftingPanel.activeSelf;
        craftingPanel.SetActive(currentStatus);

        OnCraftingUIToggled?.Invoke(currentStatus);

        // �κ��丮�� ���������� Ŀ�� �� �÷��̾� ���� ������ ����
        if (currentStatus)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // PlayerController.Instance.SetCanMove(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // PlayerController.Instance.SetCanMove(true);
        }

        // ���� �κ��丮�� ���� UI�� ���ÿ� ���� �� ���ٸ�,
        // ���� UI�� �� �� �κ��丮 UI�� �ݰ�, �� �ݴ뵵 ���������� ó���ؾ� �մϴ�.
        if (currentStatus && inventoryPanel.activeSelf)
        {
            ToggleInventoryUI(); // ���� UI ������ �κ��丮 �ݱ�
        }
        // �Ǵ�, Inventory.cs�� ToggleInventoryUI������ �� ������ �߰��Ͽ�
        // �κ��丮 ���� �� ���� UI �ݱ�
    }

    public void SetItemDescription(string description)
    {
        if (_itemDescriptionText != null)
        {
            _itemDescriptionText.text = description;
        }
    }

    private void OnHotbarSlotSelectionChanged(int newIndex)
    {
        _currentSelectedHotbarIndex = newIndex;
        UpdateHotbarSelectionUI(newIndex);
    }

    private void UpdateHotbarSelectionUI(int selectedIndex)
    {
        if (hotbarSelectionIndicators == null || hotbarSelectionIndicators.Length == 0) return;

        for (int i = 0; i < hotbarSelectionIndicators.Length; i++)
        {
            if (hotbarSelectionIndicators[i] != null)
            {
                hotbarSelectionIndicators[i].SetActive(i == selectedIndex);
            }
        }
    }

    // Inventory.OnHotbarSlotItemUpdated �̺�Ʈ �ڵ鷯
    // �ֹ� ������ ������ ������ ����� �� ȣ��˴ϴ�.
    // ���⼭�� �ַ� ������ ���� �ε������Ͱ� �ƴ�, �ֹ� ��ü�� ������ ������/���� ������Ʈ�� ����մϴ�.
    // ������ ���� Inventory.SyncHotbarSlotUI���� �̹� UI�� ���� ������Ʈ�ϹǷ�,
    // �� �̺�Ʈ�� �ٸ� UI ���(��: �ܺ� ��� UI)�� ����ȭ�ϴ� �� ������ �� �ֽ��ϴ�.
    private void OnHotbarSlotItemContentUpdated(int index, Item newItemData, int newQuantity)
    {
        // �� �̺�Ʈ�� �ַ� �ֹ� �������� �ð��� ��ȭ�� �Ͼ�� �� �߰����� UI�� ������Ʈ�ϴ� �� ���˴ϴ�.
        // ��: ���õ� �ֹ� ������ ������ �������� �ٽ� �ε��ϰų�, ������ ������Ʈ�ϴ� ���� ����.
        // ����� Inventory.SyncHotbarSlotUI�� ���� UI�� ����/�ı�/�ʱ�ȭ�ϹǷ�,
        // �̰������� Ư���� �߰��� ������ ���� �� �ֽ��ϴ�.
        // ������ ���� �ֹ� ���� �ܺο� �ش� �������� ����(��: ��� ������ ��ų ������)�� ǥ���Ѵٸ� �����մϴ�.
        Debug.Log($"�ֹ� ���� {index}�� ������ ������ ����Ǿ����ϴ�: {newItemData?.itemName} (����: {newQuantity})");
    }
}
