using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Item Info")]
    public string itemName = "New Item"; //������ �̸�

    [TextArea]
    public string description = "A basic item."; // ������ ����

    public Sprite icon;

    [Header("World Representation")]
    public GameObject WorldPrefab;

    public GameObject HandleItem;// �÷��̾� �տ� �鸱 ������Ʈ ������


    [Header("If the item can be equipped")]
    public GameObject equipmentPrefab;

    [Header("Stacking")]
    public bool isStackable = false; // �� �������� ���� �������� ����
    public int maxStackSize = 99;

    public virtual void Use(GameObject user)
    {
        Debug.Log($"{user.name} Used {itemName}");
    }
}
