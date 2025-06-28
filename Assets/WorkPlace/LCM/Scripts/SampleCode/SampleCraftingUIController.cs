using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SampleCraftingUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _craftingPanel;
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

    void Awake()
    {
        _craftingPanel.SetActive(false); // ���� �� ���� UI ��Ȱ��ȭ
        _craftingDetailPanel.SetActive(false); // �� ���� �г� ��Ȱ��ȭ

        // ���� ��ư�� Ŭ�� �̺�Ʈ ������ �߰�
        _craftButton.onClick.AddListener(OnCraftButtonClicked);
    }

    void OnEnable()
    {
        // UI ���� �ʱ�ȭ �� ��� ä���
        PopulateCraftingList();
        _craftingDetailPanel.SetActive(false);
        _currentSelectedRecipe = null;

        // �̺�Ʈ�� �� ���� �����ϰ�, �ݵ�� �� üũ�� �����մϴ�.
        if (CraftingManager.Instance != null)
        {
            CraftingManager.Instance.OnRecipeSelected += DisplayRecipeDetails;
            CraftingManager.Instance.OnCraftingCompleted += UpdateUIOnCraftingCompleted;
        }
        else
        {
            Debug.LogWarning("OnEnable: CraftingManager.Instance�� null�Դϴ�. �̺�Ʈ�� �������� �ʽ��ϴ�.");
        }

        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnHotbarSlotItemUpdated += OnInventoryOrHotbarChanged;
            // Inventory�� OnInventoryChanged �̺�Ʈ�� �ִٸ�:
            // Inventory.Instance.OnInventoryChanged += OnInventoryOrHotbarChanged;
        }
        else
        {
            Debug.LogWarning("OnEnable: Inventory.Instance�� null�Դϴ�. �̺�Ʈ�� �������� �ʽ��ϴ�.");
        }
    }

    void OnDisable()
    {
        if (CraftingManager.Instance != null) // ��Ȯ���� ���� ������� �� üũ
        {
            CraftingManager.Instance.OnRecipeSelected -= DisplayRecipeDetails;
            CraftingManager.Instance.OnCraftingCompleted -= UpdateUIOnCraftingCompleted;
        }
        else
        {
            // ���ø����̼� ����/�� ��ȯ �� ������ �޽����Դϴ�.
            Debug.Log("SampleCraftingUIController.OnDisable ���� �� CraftingManager.Instance�� �̹� �ı��Ǿ��ų� null�Դϴ�. ���� ������ �ǳʶݴϴ�.");
        }
        if (Inventory.Instance != null) // ��Ȯ���� ���� ������� �� üũ
        {
            Inventory.Instance.OnHotbarSlotItemUpdated -= OnInventoryOrHotbarChanged;
            // ���� OnInventoryChanged �̺�Ʈ�� ����Ѵٸ�:
            // Inventory.Instance.OnInventoryChanged -= OnInventoryOrHotbarChanged;
        }
        else
        {
            Debug.Log("SampleCraftingUIController.OnDisable ���� �� Inventory.Instance�� �̹� �ı��Ǿ��ų� null�Դϴ�. ���� ������ �ǳʶݴϴ�.");
        }

        // UI�� ��Ȱ��ȭ�� �� ������ ���Ե��� ����
        ClearCraftingListSlots();
        ClearMaterialList();
    }
    void Start()
    {
        PopulateCraftingList(); // ���� �� ���� ��� ä���
    }

    // ���� UI �г��� �Ѱ� ���� �Լ�
    public void ToggleCraftingUI()
    {
        _craftingPanel.SetActive(!_craftingPanel.activeSelf);
        if (_craftingPanel.activeSelf)
        {
            PopulateCraftingList(); // UI�� ���� ������ ��� ���� (���� ����)
            _craftingDetailPanel.SetActive(false); // UI �� �� �� �г��� ��Ȱ��ȭ
            _currentSelectedRecipe = null; // ���õ� ������ �ʱ�ȭ
        }
    }

    // ���� ����� ��ũ�Ѻ信 ä��� �Լ�
    private void PopulateCraftingList()
    {
        ClearCraftingListSlots(); // ���� ���� ����

        // _craftingItemSlotPrefab�� _craftingListContent�� �Ҵ�Ǿ����� ������ üũ �߰� 
        if (_craftingItemSlotPrefab == null)
        {
            Debug.LogError("_craftingItemSlotPrefab�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }
        if (_craftingListContent == null)
        {
            Debug.LogError("_craftingListContent�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        // ��� �����Ǹ� �����ͼ� ���� ����
        foreach (Recipe recipe in CraftingManager.Instance.AllCraftingRecipes)
        {
            if (recipe.craftedItem == null)
            {
                Debug.LogWarning($"������ '{recipe.name}'�� ���� �������� �Ҵ���� �ʾҽ��ϴ�. �� �����Ǵ� �ǳʍ��ϴ�.");
                continue;
            }

            GameObject slotGO = Instantiate(_craftingItemSlotPrefab, _craftingListContent);
            _instantiatedRecipeSlots.Add(slotGO);

            // --- ���Ⱑ ����Ǵ� �κ��Դϴ�! ---
            CraftingItemUISlot uiSlot = slotGO.GetComponent<CraftingItemUISlot>();

            if (uiSlot == null)
            {
                Debug.LogError($"'{_craftingItemSlotPrefab.name}' �����տ� CraftingItemUISlot ������Ʈ�� �����ϴ�! Ȯ�����ּ���.");
                Destroy(slotGO); // �߸��� ������ �ı�
                continue;
            }

            // ���� uiSlot�� SetUI �޼��带 ȣ���Ͽ� �����͸� �����մϴ�.
            uiSlot.SetUI(recipe);
        }
    }

    // ���õ� �������� �� ������ UI�� ǥ��
    private void DisplayRecipeDetails(Recipe recipe)
    {
        _currentSelectedRecipe = recipe;
        _craftingDetailPanel.SetActive(true);

        if (recipe == null)
        {
            // �����ǰ� null�� ��� �� ���� ����
            _craftedItemImage.sprite = null;
            _craftedItemNameText.text = "������ ���� �ȵ�";
            _craftedItemDescriptionText.text = "";
            _craftedItemCurrentAmountText.text = "";
            ClearMaterialList();
            _craftButton.interactable = false;
            return;
        }

        // ���۵� ������ ����
        if (_craftedItemImage != null) _craftedItemImage.sprite = recipe.craftedItem.icon;
        if (_craftedItemNameText != null) _craftedItemNameText.text = recipe.craftedItem.itemName;
        if (_craftedItemDescriptionText != null) _craftedItemDescriptionText.text = recipe.description;

        // �÷��̾ ���� ���� ������ ���� ǥ��
        int currentCraftedItemAmount = Inventory.Instance.GetItemCount(recipe.craftedItem);
        if (_craftedItemCurrentAmountText != null) _craftedItemCurrentAmountText.text = $"����: {currentCraftedItemAmount}��";

        // ��� ��� ǥ��
        ClearMaterialList(); // ���� ��� UI ����
        foreach (var material in recipe.requiredMaterials)
        {
            GameObject materialUI = Instantiate(_materialItemUIPrefab, _materialListContainer);
            if (materialUI == null) continue;
            _instantiatedMaterialUIs.Add(materialUI);

            MaterialItemUISlot uiSlot = materialUI.GetComponent<MaterialItemUISlot>();

            if (uiSlot == null)
            {
                Debug.LogError($"'{_materialItemUIPrefab.name}' �����տ� MaterialItemUISlot ������Ʈ�� �����ϴ�! Ȯ�����ּ���.");
                continue;
            }

            Sprite icon = material.materialItem?.icon; // Null-conditional operator�� �����ϰ� ����
            string name = material.materialItem?.itemName;
            int playerHasAmount = Inventory.Instance.GetItemCount(material.materialItem);
            string quantityText = $"{playerHasAmount} / {material.quantity}";
            Color quantityColor = (playerHasAmount < material.quantity) ? Color.red : Color.white;

            uiSlot.SetUI(icon, name, quantityText, quantityColor); // MaterialItemUISlot�� SetUI �޼��� ���
        }

        // ���� ���� ���ο� ���� ��ư Ȱ��ȭ/��Ȱ��ȭ
        _craftButton.interactable = CraftingManager.Instance.CanCraft(recipe);
    }

    // ��� ��� UI�� ���� �Լ�
    private void ClearMaterialList()
    {
        foreach (GameObject go in _instantiatedMaterialUIs)
        {
            Destroy(go);
        }
        _instantiatedMaterialUIs.Clear();
    }

    // "�����ϱ�" ��ư Ŭ�� �� ȣ��� �Լ�
    private void OnCraftButtonClicked()
    {
        if (_currentSelectedRecipe != null)
        {
            CraftingManager.Instance.CraftItem(_currentSelectedRecipe);
        }
    }

    // �κ��丮�� �ֹ� �������� ����Ǿ��� �� UI�� ����
    // (Inventory ��ũ��Ʈ���� OnInventoryChanged ���� �̺�Ʈ�� �߻����� �� ȣ��)
    // ����� OnHotbarSlotItemUpdated�� ����ϹǷ� �ʿ信 ���� Ȯ��
    private void OnInventoryOrHotbarChanged(int index, Item item, int quantity)
    {
        // ���� ���õ� �����ǰ� �ִٸ�, �ش� �������� �� ������ �ٽ� ǥ���Ͽ� ��� ���� ������ ����
        if (_currentSelectedRecipe != null)
        {
            DisplayRecipeDetails(_currentSelectedRecipe);
        }
    }

    // ���� �Ϸ� �� UI ���� (���õ� �����ǰ� �״�� ������ ��� ���)
    private void UpdateUIOnCraftingCompleted()
    {
        // ������ �Ϸ�Ǿ����Ƿ�, ���� ���õ� �������� ��� ���� ���� ���� �ٽ� ������Ʈ
        // DisplayRecipeDetails(_currentSelectedRecipe)�� CraftingManager.OnCraftingCompleted �̺�Ʈ�� ���� ȣ��ǹǷ� ���⼭�� �߰� �۾� ���ʿ��� �� ����
        // ���� OnRecipeSelected �̺�Ʈ�� �߻����� �ʴ´ٸ� ���⼭ �ٽ� ȣ��
    }

    private void ClearCraftingListSlots()
    {
        foreach (GameObject go in _instantiatedRecipeSlots)
        {
            Destroy(go);
        }
        _instantiatedRecipeSlots.Clear();
    }


}
