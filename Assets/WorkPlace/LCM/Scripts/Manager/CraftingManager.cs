using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using System;

public class CraftingManager : Singleton<CraftingManager>
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
        SingletonInit();
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

            int playerHasAmount = Inventory.Instance.GetItemCount(material.materialItem);
            if (playerHasAmount < material.quantity)
            {
                // ����״� ���� �߹Ƿ�, �ʿ��� ���� ����ϵ��� �ּ� ó���ϰų� ���Ǻη� ���
                // Debug.Log($"��� ����: {material.materialItem.name} (����: {playerHasAmount} / �ʿ�: {material.quantity})");
                return false;
            }
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
        //��� �Ҹ�
        foreach(var material in recipe.requiredMaterials)
        {
            Inventory.Instance.RemoveItem(material.materialItem, material.quantity);
        }
        //������ ���� �� �κ��丮 �߰�
        for(int i = 0; i < recipe.craftedAmount; i++)
        {
            Inventory.Instance.SpawnInventoryItem(recipe.craftedItem);
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
