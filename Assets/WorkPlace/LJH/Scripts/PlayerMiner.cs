using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMiner : MonoBehaviour
{
    private MineableResource currentTarget;
    public float interactRange = 3f;             // ��ȣ�ۿ� �Ÿ�
    public KeyCode interactKey = KeyCode.E;      // ��ȣ�ۿ� Ű
    public LayerMask interactLayer;               // ��ȣ�ۿ� ������ ���̾� ����

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            // �������̽��� ��������
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
            else
            {
                Debug.Log("��ȣ�ۿ� ������ ������Ʈ�� �ƴմϴ�.");
            }
        }
        else
        {
            Debug.Log("��ȣ�ۿ� ����� �����ϴ�.");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Resource"))
        {
            currentTarget = other.GetComponent<MineableResource>();
            if (currentTarget != null)
            {
                currentTarget.StartMining();
                Debug.Log("ä�� ����!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Resource"))
        {
            if (currentTarget != null)
            {
                currentTarget.StopMining();
                Debug.Log("ä�� �ߴ�!");
            }
            currentTarget = null;
        }
    }
}
