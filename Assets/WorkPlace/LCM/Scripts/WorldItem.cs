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


    //�÷��̾� ����
    //private GameObject player; // �÷��̾� ������Ʈ�� ���� ���� , �Ÿ������� ���
    //private bool playerInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        //�÷��̾� ����
        //player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //�÷��̾� ����
        //if (player != null)
        //{
        //    float distance = Vector3.Distance(transform.position, player.transform.position);
        //    bool wasPlayerInRange = playerInRange;
        //    playerInRange = distance <= interactionRange;

        //    if (playerInRange && !wasPlayerInRange)
        //    {
        //        Debug.Log($"�÷��̾ ������ ��ó�� �ֽ��ϴ�");
        //    }

        //    else if (!playerInRange && wasPlayerInRange)
        //    {
        //        Debug.Log($"�������� �������� ������ϴ�.");
        //    }
        //}
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
