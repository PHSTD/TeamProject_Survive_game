using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractable
{
    [Tooltip("�� 3D ������Ʈ�� ��Ÿ���� Item ScriptableObject ������")]
    public Item itemData;

    [Header("Interaction Settings")]
    [Tooltip("�÷��̾ ��ȣ�ۿ� ������ ����")]
    public float interactionRange = 0.5f;

    void Start()
    {
    }

    void Update()
    {

    }

    public void Interact()
    {
        if(itemData == null)
        {
            Debug.Log("������ �����Ͱ� �Ҵ���� �ʾҽ��ϴ�.");
        }

        Debug.Log("�����۰� ��ȣ�ۿ� �߽��ϴ�. �κ��丮�� �߰� �ϰڽ��ϴ�");

        Inventory.Instance.SpawnInventoryItem(this.itemData);

        Destroy(gameObject);
    }
}
