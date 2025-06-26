using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SampleCraftingUIController : MonoBehaviour
{
    [SerializeField] private Button craftButton;
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI materialInfoText;

    public Recipe currentSelectedRecipe;
    private void Start()
    {
        craftButton.onClick.AddListener(OnCraftButtonClicked);
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (currentSelectedRecipe != null)
        {
            recipeNameText.text = currentSelectedRecipe.craftedItem.itemName;
            // ��� ���� ǥ�� ����
            string materialsString = "���:\n";
            bool canCraft = true;
            foreach (var material in currentSelectedRecipe.requiredMaterials)
            {
                int playerHas = Inventory.Instance.GetItemCount(material.materialItem);
                materialsString += $"{material.materialItem.name}: {playerHas}/{material.quantity}\n";
                if (playerHas < material.quantity) canCraft = false;
            }
            materialInfoText.text = materialsString;

            // ���� ���� ���ο� ���� ��ư Ȱ��ȭ/��Ȱ��ȭ
            craftButton.interactable = CraftingManager.Instance.CanCraft(currentSelectedRecipe); // <- CraftingManager ���!
        }
        else
        {
            recipeNameText.text = "������ ���� �ȵ�";
            materialInfoText.text = "";
            craftButton.interactable = false;
        }
    }

    void OnCraftButtonClicked()
    {
        if (currentSelectedRecipe != null)
        {
            // CraftingManager�� ���� ������ ���� ��û
            CraftingManager.Instance.CraftItem(currentSelectedRecipe); // <- CraftingManager ���!
            RefreshUI(); // ���� �� UI ����
        }
    }

    public void SetSelectedRecipe(Recipe newRecipe)
    {
        currentSelectedRecipe = newRecipe;
        RefreshUI();
    }
}
