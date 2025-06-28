using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class Recipe : ScriptableObject
{
    public Item craftedItem; // ���۵� ������ (Item ScriptableObject)
    public int craftedAmount = 1; // ���۵� �������� ����

    [System.Serializable]
    public class Material
    {
        public Item materialItem; // ��� ������
        public int quantity; // �ʿ��� ����
    }

    public List<Material> requiredMaterials; // �ʿ��� ��� ���
    public string description; // ���� �����ۿ� ���� ����
}

