using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;

public class CraftingManager : Singleton<CraftingManager>
{

    [Header("All Crafting Recipes")]
    [SerializeField]
    private List<Recipe> _allCraftingRecipes; // ��� ���� ������ ���

    //������ ��� ���� ������Ƽ
    public List<Recipe> AllCraftingRecipes => _allCraftingRecipes;


    protected void Awake()
    {
        SingletonInit();
    }
    
    public bool CanCraft(Recipe recipe)
    {
        if(recipe == null)
        {
            return false;
        }

        foreach(var material in recipe.requiredMaterials)
        {
            if(material.materialItem == null)
            {
                Debug.LogWarning($"�����ǿ� ��� �������� �Ҵ� �ȵ�");
                return false;
            }

            int playerHasAmount = Inventory.Instance.GetItemCount(material.materialItem);
            if (playerHasAmount < material.quantity)
            {
                Debug.Log($"��� ����: {material.materialItem.name} ({playerHasAmount}/{material.quantity}) for {recipe.craftedItem.name}");
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

        foreach(var material in recipe.requiredMaterials)
        {
            Inventory.Instance.RemoveItem(material.materialItem, material.quantity);
        }

        for(int i = 0; i < recipe.craftedAmount; i++)
        {
            Inventory.Instance.SpawnInventoryItem(recipe.craftedItem);
        }

        Debug.Log("���ۿϷ�");
    }
}
