using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Test;

public class TestWorldItem : MonoBehaviour, IInteractable
{
    [Tooltip("�� 3D ������Ʈ�� ��Ÿ���� Item ScriptableObject ������")]
    public Item itemData;

    [Header("Interaction Settings")]
    [Tooltip("�÷��̾ ��ȣ�ۿ� ������ ����")]
    public float interactionRange = 0.5f;

    public void Initialize(Item item)
    {
        itemData = item;
        // �ʿ��ϴٸ� ������ ������ ���� �ð��� ��� (SpriteRenderer ��)�� ������ �� �ֽ��ϴ�.
        // ��: GetComponent<SpriteRenderer>().sprite = item.WorldSprite;
    }

    public void Interact()
    {
        if (itemData == null)
        {
            Debug.Log("������ �����Ͱ� �Ҵ���� �ʾҽ��ϴ�.");
        }

        Debug.Log("�����۰� ��ȣ�ۿ� �߽��ϴ�. �κ��丮�� �߰� �ϰڽ��ϴ�");

        Inventory.Instance.SpawnInventoryItem(this.itemData);
        PlayerManager.Instance.SelectItem = this.itemData;

        TestToolItem toolItem = this.itemData as TestToolItem;
        if(toolItem != null)
        {
            // GameObject toolObject = Instantiate(toolItem.toolPrefab);
            // toolItem.toolObject = toolObject;
            // toolItem.toolAction = toolObject.GetComponent<TestToolAction>();
            // toolObject.SetActive(false);
            // Ȥ�� �ش� �ֹٸ� �����Ҷ� ���� ���� �� �ı�
            toolItem.toolAction = toolItem.toolPrefab.GetComponent<TestToolAction>();
        }

        Destroy(gameObject);
    }
}
