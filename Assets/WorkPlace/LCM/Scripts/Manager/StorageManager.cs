using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public GameObject inventorySlotPrefab; // �κ��丮 ���� ������
    public Transform contentParent;         // Scroll Rect�� Content ������Ʈ

    public int numberOfSlotsToCreate = 50; // ������ ������ ���� (����)

    // Start is called before the first frame update
    void Start()
    {
        GenerateInventorySlots(numberOfSlotsToCreate);
    }

    public void GenerateInventorySlots(int count)
    {
        // ���� ���� ���� (�ɼ�)
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ���ο� ���� ����
        for (int i = 0; i < count; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, contentParent);
            // ���⿡ �ʿ��ϴٸ� ���Կ� ������(��: ������ ����)�� �����ϴ� ���� �߰�
            // ��: slot.GetComponent<InventorySlotUI>().SetItem(inventoryItems[i]);
        }

        // ContentSizeFitter�� ��� ������Ʈ���� ���� �� �����Ƿ� RebuildLayoutGroup ȣ��
        // LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
    }
}
