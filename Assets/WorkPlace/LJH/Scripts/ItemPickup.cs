using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public GameObject pickupUI;  // UI ǥ�ÿ� (��: "Press E to collect")

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (pickupUI != null)
            pickupUI.SetActive(false);
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactionDistance)
        {
            if (pickupUI != null)
                pickupUI.SetActive(true);

            if (Input.GetKeyDown(interactKey))
            {
                Collect();
            }
        }
        else
        {
            if (pickupUI != null)
                pickupUI.SetActive(false);
        }
    }

    void Collect()
    {
        Debug.Log("�������� ȹ���߽��ϴ�!");

        // ������ �߰� ���� (��: �κ��丮 �߰�)

        Destroy(gameObject);  // �Ĺ�����Ʈ ����
    }
}