using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class Recipe : ScriptableObject
{
    [Header("Crafted Item")]
    public Item craftedItem; // ���� ����� ������ (�ϼ��� ����Ʈ ������ ��)
    public int craftedAmount = 1; // ���� �� ��� ����

    [Header("Required Materials")]
    // �ʿ��� ��� �����۰� �� ����� ����
    public List<CraftingMaterial> requiredMaterials = new List<CraftingMaterial>();

    [System.Serializable]
    public class CraftingMaterial
    {
        public Item materialItem;
        public int requiredAmount;
    }
}

