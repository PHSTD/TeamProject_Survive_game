using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInventoryAdder : MonoBehaviour
{
    [SerializeField] private List<Item> debugItems; // �ν����Ϳ��� �ְ� ���� Item ScriptableObject���� �Ҵ�
    [SerializeField] private int debugQuantity = 10; // �ѹ��� �߰��� �⺻ ����

    // �� ��ư�� UI�� �����ϰų� �����Ϳ��� ���� ȣ��
    public void AddDebugItemsToStorage()
    {
        if (Storage.Instance == null)
        {
            Debug.LogError("Storage.Instance�� �����ϴ�!");
            return;
        }

        foreach (Item item in debugItems)
        {
            if (item != null)
            {
                Storage.Instance.AddItemToStorage(item, debugQuantity);
                Debug.Log($"{item.name} {debugQuantity}���� â�� �߰��߽��ϴ�.");
            }
        }
    }

    // Ư�� �������� �߰��ϴ� �޼��� (���� ����)
    public void AddSpecificItemToStorage(Item item, int quantity)
    {
        if (Storage.Instance == null)
        {
            Debug.LogError("Storage.Instance�� �����ϴ�!");
            return;
        }
        if (item != null)
        {
            Storage.Instance.AddItemToStorage(item, quantity);
            Debug.Log($"{item.name} {quantity}���� â�� �߰��߽��ϴ�.");
        }
    }
}
