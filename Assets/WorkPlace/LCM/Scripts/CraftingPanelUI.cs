using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPanelUI : MonoBehaviour
{
    public StorageManager storageManager; // StorageManager ��ũ��Ʈ ����
    public Button openStorageButton;

    [Header("Dependencies")]
    [SerializeField] private CraftingManager _craftingManager; // CraftingManager ����
    //[SerializeField] private Inventory _inventory;             // Inventory ����

    [Header("UI References")]
    [SerializeField] private GameObject _craftingPanel; // ��ü ���� UI�� ��� �ִ� �θ� GameObject (�� ��ũ��Ʈ�� ���� GameObject ��ü�� �г��� ���� ����)
    [SerializeField] private RectTransform _craftingListContent; // ��ũ�Ѻ��� Content
    [SerializeField] private GameObject _craftingItemSlotPrefab; // ���� ������ ���� ������

    [Header("Detail Panel References")]
    [SerializeField] private GameObject _craftingDetailPanel;
    [SerializeField] private Image _craftedItemImage;
    [SerializeField] private TextMeshProUGUI _craftedItemNameText;
    [SerializeField] private TextMeshProUGUI _craftedItemDescriptionText;
    [SerializeField] private TextMeshProUGUI _craftedItemCurrentAmountText;
    [SerializeField] private Transform _materialListContainer; // ��� ����� �� �θ�
    [SerializeField] private GameObject _materialItemUIPrefab; // ��� �׸� UI ������
    [SerializeField] private Button _craftButton;

    private Recipe _currentSelectedRecipe; // ���� ���õ� ������

    private List<GameObject> _instantiatedRecipeSlots = new List<GameObject>(); // ������ ������ ���� ����
    private List<GameObject> _instantiatedMaterialUIs = new List<GameObject>(); // ������ ��� UI ����



    private void Awake()
    {
        // ���� CraftingPanelUI�� Awake ���� (���� �־��ٸ�)
        // Example: UI ���� �� ��Ȱ��ȭ
        // _craftingPanel.SetActive(false); // ���� �� ��ũ��Ʈ�� �θ� �г��̶�� �ʿ�
        // _craftingDetailPanel.SetActive(false);

        // SampleCraftingUIController�� Awake ���� ����
        // �� ��ũ��Ʈ�� CraftingPanel ��ü�� �ٴ´ٸ� _craftingPanel ������ this.gameObject�� ����� �� �ֽ��ϴ�.
        // ���⼭�� _craftingPanel�� ��ü Crafting UI�� �ǹ��Ѵٰ� �����ϰ� �״�� �Ӵϴ�.
        if (_craftingDetailPanel != null) _craftingDetailPanel.SetActive(false);

        // ������ üũ
        if (_craftingManager == null) { Debug.LogError("CraftingManager�� �Ҵ���� �ʾҽ��ϴ�!", this); enabled = false; return; }
        if (_craftButton != null) _craftButton.onClick.AddListener(OnCraftButtonClicked);

        // ���� CraftingPanelUI�� openStorageButton ����
        if (openStorageButton != null)
        {
            openStorageButton.onClick.AddListener(OpenStorage);
        }
        else
        {
            Debug.LogWarning("openStorageButton�� CraftingPanelUI ��ũ��Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
        }


    }

    private void OnEnable()
    {
        // SampleCraftingUIController�� OnEnable ���� ����
        PopulateCraftingList();
        _craftingDetailPanel.SetActive(false);
        _currentSelectedRecipe = null;

        if (_craftingManager != null)
        {
            _craftingManager.OnRecipeSelected += DisplayRecipeDetails;
            _craftingManager.OnCraftingCompleted += UpdateUIOnCraftingCompleted;
        }
        if (Storage.Instance != null)
        {
            Storage.Instance.OnStorageSlotItemUpdated += OnInventoryOrHotbarChanged;
        }
    }

    private void OnDisable()
    {
        // SampleCraftingUIController�� OnDisable ���� ����
        if (_craftingManager != null)
        {
            _craftingManager.OnRecipeSelected -= DisplayRecipeDetails;
            _craftingManager.OnCraftingCompleted -= UpdateUIOnCraftingCompleted;
        }
        if (Storage.Instance != null)
        {
            Storage.Instance.OnStorageSlotItemUpdated -= OnInventoryOrHotbarChanged;
        }

        ClearCraftingListSlots();
        ClearMaterialList();
    }
    private void Start()
    {

    }

    public void OpenStorage()
    {
        if (StorageManager.Instance != null)
        {
            StorageManager.Instance.OpenStorageUI();
        }
        else
        {
            Debug.LogError("StorageManager ������ CraftingPanelUI�� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }

    public void CloseStorage()
    {
        if (StorageManager.Instance != null)
        {
            StorageManager.Instance.CloseStorageUI();
        }
        else
        {
            Debug.LogError("StorageManager ������ CraftingPanelUI�� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }

    private void PopulateCraftingList()
    {
        ClearCraftingListSlots();

        if (_craftingItemSlotPrefab == null) { Debug.LogError("_craftingItemSlotPrefab�� �Ҵ���� �ʾҽ��ϴ�!"); return; }
        if (_craftingListContent == null) { Debug.LogError("_craftingListContent�� �Ҵ���� �ʾҽ��ϴ�!"); return; }
        if (_craftingManager == null) { Debug.LogError("CraftingManager�� �Ҵ���� �ʾҽ��ϴ�!"); return; }

        foreach (Recipe recipe in _craftingManager.AllCraftingRecipes)
        {
            if (recipe.craftedItem == null) { Debug.LogWarning($"������ '{recipe.name}'�� ���� �������� �Ҵ���� �ʾҽ��ϴ�. �� �����Ǵ� �ǳʍ��ϴ�."); continue; }

            GameObject slotGO = Instantiate(_craftingItemSlotPrefab, _craftingListContent);
            _instantiatedRecipeSlots.Add(slotGO);

            CraftingItemUISlot uiSlot = slotGO.GetComponent<CraftingItemUISlot>();

            if (uiSlot == null) { Debug.LogError($"'{_craftingItemSlotPrefab.name}' �����տ� CraftingItemUISlot ������Ʈ�� �����ϴ�! Ȯ�����ּ���."); Destroy(slotGO); continue; }

            uiSlot.SetUI(recipe, _craftingManager);
        }
    }

    private void DisplayRecipeDetails(Recipe recipe)
    {
        _currentSelectedRecipe = recipe;
        _craftingDetailPanel.SetActive(true);

        if (recipe == null)
        {
            _craftedItemImage.sprite = null;
            _craftedItemNameText.text = "������ ���� �ȵ�";
            _craftedItemDescriptionText.text = "";
            _craftedItemCurrentAmountText.text = "";
            ClearMaterialList();
            _craftButton.interactable = false;
            return;
        }

        if (_craftedItemImage != null) _craftedItemImage.sprite = recipe.craftedItem.icon;
        if (_craftedItemNameText != null) _craftedItemNameText.text = recipe.craftedItem.itemName;
        if (_craftedItemDescriptionText != null) _craftedItemDescriptionText.text = recipe.description;

        int currentCraftedItemAmount = Storage.Instance.GetItemCount(recipe.craftedItem);
        if (_craftedItemCurrentAmountText != null) _craftedItemCurrentAmountText.text = $"����: {currentCraftedItemAmount}��";

        ClearMaterialList();
        foreach (var material in recipe.requiredMaterials)
        {
            GameObject materialUI = Instantiate(_materialItemUIPrefab, _materialListContainer);
            if (materialUI == null) continue;
            _instantiatedMaterialUIs.Add(materialUI);

            MaterialItemUISlot uiSlot = materialUI.GetComponent<MaterialItemUISlot>();

            if (uiSlot == null) { Debug.LogError($"'{_materialItemUIPrefab.name}' �����տ� MaterialItemUISlot ������Ʈ�� �����ϴ�! Ȯ�����ּ���."); continue; }

            Sprite icon = material.materialItem?.icon;
            string name = material.materialItem?.itemName;
            int playerHasAmount = Storage.Instance.GetItemCount(material.materialItem);
            string quantityText = $"{playerHasAmount} / {material.quantity}";
            Color quantityColor = (playerHasAmount < material.quantity) ? Color.red : Color.white;

            uiSlot.SetUI(icon, name, quantityText, quantityColor);
        }

        _craftButton.interactable = _craftingManager.CanCraft(recipe);
    }

    private void ClearMaterialList()
    {
        foreach (GameObject go in _instantiatedMaterialUIs) { Destroy(go); }
        _instantiatedMaterialUIs.Clear();
    }

    private void OnCraftButtonClicked()
    {
        if (_currentSelectedRecipe != null)
        {
            _craftingManager.CraftItem(_currentSelectedRecipe);
        }
    }

    private void OnInventoryOrHotbarChanged(int index, Item item, int quantity)
    {
        if (_currentSelectedRecipe != null)
        {
            DisplayRecipeDetails(_currentSelectedRecipe);
        }
    }

    private void UpdateUIOnCraftingCompleted()
    {
        if (_currentSelectedRecipe != null)
        {
            DisplayRecipeDetails(_currentSelectedRecipe);
        }
        PopulateCraftingList();
    }

    private void ClearCraftingListSlots()
    {
        foreach (GameObject go in _instantiatedRecipeSlots) { Destroy(go); }
        _instantiatedRecipeSlots.Clear();
    }
}
