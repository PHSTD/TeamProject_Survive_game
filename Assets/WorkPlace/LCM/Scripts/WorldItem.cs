using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractable
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
        Debug.Log($"[WorldItem] Interact() ȣ���. ������: {itemData?.itemName ?? "Unknown"}");
        Debug.Log("�����۰� ��ȣ�ۿ� �߽��ϴ�. �κ��丮�� �߰� �ϰڽ��ϴ�");

        Inventory.Instance.SpawnInventoryItem(this.itemData);

        PlayerManager.Instance.SelectItem = this.itemData;

        ToolItem toolItem = this.itemData as ToolItem;

        if (toolItem != null)
        {
            // GameObject toolObject = Instantiate(toolItem.toolPrefab);
            // toolItem.toolObject = toolObject;
            // toolItem.toolAction = toolObject.GetComponent<TestToolAction>();
            // toolObject.SetActive(false);
            // Ȥ�� �ش� �ֹٸ� �����Ҷ� ���� ���� �� �ı�
            toolItem.toolAction = toolItem.HandleItem.GetComponent<ToolAction>();
        }
        Debug.Log($"[WorldItem] Destroy(gameObject) ȣ�� ����. �ı��� ������Ʈ: {gameObject.name}");
        Destroy(gameObject);
        Debug.Log($"[WorldItem] Destroy(gameObject) ȣ�� �Ϸ�.");
    }
}
