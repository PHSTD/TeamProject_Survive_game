using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CraftingManager : MonoBehaviour
{

    [Header("All Crafting Recipes")]
    [SerializeField]
    private List<Recipe> _allCraftingRecipes; // ��� ���� ������ ���

    //������ ��� ���� ������Ƽ
    public List<Recipe> AllCraftingRecipes => _allCraftingRecipes;

    // ���õ� �����ǰ� ����� �� ȣ��� �̺�Ʈ (UI ������Ʈ��)
    public event Action<Recipe> OnRecipeSelected;
    // ������ �Ϸ�Ǿ��� �� ȣ��� �̺�Ʈ (UI ������Ʈ��)
    public event Action OnCraftingCompleted;


    protected void Awake()
    {
        if (Storage.Instance == null)
        {
            Debug.LogError("CraftingManager: Storage.Instance�� ã�� �� �����ϴ�. Storage ��ũ��Ʈ�� ���� �ִ��� Ȯ�����ּ���!", this);
        }
    }
    
    public bool CanCraft(Recipe recipe)
    {
        if (recipe == null)
        {
            return false;
        }

        foreach (var material in recipe.requiredMaterials)
        {
            if (material.materialItem == null)
            {
                Debug.LogWarning($"������ '{recipe.name}'�� ��� �������� �Ҵ���� �ʾҽ��ϴ�. ���� �Ұ�.");
                return false;
            }

            int playerHasAmount = Storage.Instance.GetItemCount(material.materialItem);
            if (playerHasAmount < material.quantity)
            {
                // ����״� ���� �߹Ƿ�, �ʿ��� ���� ����ϵ��� �ּ� ó���ϰų� ���Ǻη� ���
                // Debug.Log($"��� ����: {material.materialItem.name} (����: {playerHasAmount} / �ʿ�: {material.quantity})");
                return false;
            }
        }

        if (StatusSystem.Instance == null)
        {
            Debug.LogError("StatusSystem.Instance�� ã�� �� �����ϴ�. ���� Ȯ�� �Ұ�.");
            return false;
        }
        if (StatusSystem.Instance.GetEnergy() < recipe.energyCost)
        {
            Debug.Log($"���� ����: {recipe.name} ���ۿ� �ʿ��� ���� {recipe.energyCost} / ���� ���� {StatusSystem.Instance.GetEnergy()}");
            return false;
        }

        return true;
    }

    public void CraftItem(Recipe recipe)
    {

        if (!CanCraft(recipe))
        {
            Debug.LogWarning($"������ '{recipe.name}'�� ���� �Ҽ� �����ϴ�. ��Ḧ Ȯ���� �ּ���");
            return;
        }
        // --- �߰��� �κ�: ���� �Ҹ� ---
        StatusSystem.Instance.SetMinusEnergy(recipe.energyCost);
        Debug.Log($"���� {recipe.energyCost} �Ҹ�. ���� ����: {StatusSystem.Instance.GetEnergy()}");
        //��� �Ҹ�
        foreach (var material in recipe.requiredMaterials)
        {
            Storage.Instance.RemoveItem(material.materialItem, material.quantity);
        }
        //������ ���� �� �κ��丮 �߰�
        for(int i = 0; i < recipe.craftedAmount; i++)
        {
            Storage.Instance.AddItemToStorage(recipe.craftedItem, recipe.craftedAmount);
        }

        Debug.Log($"'{recipe.craftedItem.name}' (x{recipe.craftedAmount}) ���� �Ϸ�!");
        OnCraftingCompleted?.Invoke(); // ���� �Ϸ� �̺�Ʈ �߻�
        OnRecipeSelected?.Invoke(recipe); // ���� �� ��� ������ ���� ���õ� ������ ���� �ٽ� ���� (���� ���� ����)
    }

    public void SelectRecipe(Recipe recipe)
    {
        OnRecipeSelected?.Invoke(recipe);
    }

}
