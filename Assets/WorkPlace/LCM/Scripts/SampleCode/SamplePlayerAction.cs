using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplePlayerAction : MonoBehaviour
{
    [SerializeField] private float interactionRange = 3f; // ��ȣ�ۿ� ������ �Ÿ�
    [SerializeField] private LayerMask mineableLayer; // ä�� ������ ������Ʈ ���̾� (���� GameObject�� Layer�� �������ּ���)

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� ���� (�÷��̾ ������ ����Ϸ��� �Է�)
        // MouseButtonDown�� �� �� Ŭ���� �� ���� ȣ��˴ϴ�.
        if (Input.GetMouseButtonDown(0))
        {
            UseEquippedItem();
        }
    }

    void UseEquippedItem()
    {
        // �ֹ� ���� ���� (��: ���� Ű 1~9)
        // PlayerController�� ���� �ʿ�
        //for (int i = 0; i < 9; i++) // �ֹ� ������ 9����� ����
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1 + i)) // Alpha1�� ���� 1 Ű
        //    {
        //        // Inventory ��ũ��Ʈ�� �ֹ� ���� �޼ҵ带 ȣ��
        //        Inventory.Instance.SelectHotbarSlot(i);
        //        break;
        //    }
        //}
        Item currentItem = Inventory.Instance.GetCurrentHotbarItem();

        if (currentItem == null)
        {
            Debug.Log("�ֹٿ� ���õ� �������� �����ϴ�.");
            return;
        }

        // �������� ToolItem Ÿ������ Ȯ���մϴ�.
        if (currentItem is ToolItem tool) // 'is' �����ڷ� Ÿ�� Ȯ�� �� 'tool' ������ ĳ����
        {
            // �� ������ ä�� ����(Pickaxe) Ÿ������ Ȯ��
            if (tool.toolType == ToolType.Pickaxe)
            {
                Debug.Log($"ä�� ���� '{tool.itemName}' ��� �õ�. ä����: {tool.miningPower}");

                // �÷��̾��� ����(ī�޶�)���� ����ĳ��Ʈ �߻�
                // ī�޶� �÷��̾��� �ڽ� ������Ʈ��� Camera.main.transform.forward ���
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // ȭ�� �߾ӿ��� ���� �߻�
                RaycastHit hit;

                // mineableLayer�� �ش��ϴ� ������Ʈ�� ����
                if (Physics.Raycast(ray, out hit, interactionRange, mineableLayer))
                {
                    SampleMineableResource mineable = hit.collider.GetComponent<SampleMineableResource>();
                    if (mineable != null)
                    {
                        Debug.Log($"{hit.collider.name}�� �������� �ݴϴ�.");
                        // ã�� MineableResource�� ä�� ������ miningPower��ŭ ������ ����
                        mineable.TakeMiningDamage(tool.miningPower);

                        // ���⿡ ä�� �ִϸ��̼�, ����, ��ƼŬ ȿ�� ���� �߰��� �� �ֽ��ϴ�.
                        // ��: PlayMiningAnimation();
                        // ��: PlayMiningSound();
                    }
                    else
                    {
                        Debug.Log("�ٶ󺸴� ������Ʈ�� ä�� ������ ����� �ƴմϴ�.");
                    }
                }
                else
                {
                    Debug.Log("ä���� ����� ã�� ���߽��ϴ�.");
                }
            }
            else
            {
                Debug.Log($"{tool.itemName}�� ��̰� �ƴմϴ�. �ٸ� ���� ���� �ʿ�.");
            }
        }
        else // ������ �ƴ� �ٸ� ������ Ÿ�� (��: �Ҹ�ǰ, ����)
        {
            currentItem.Use(this.gameObject); // �ش� �������� �Ϲ� Use �޼ҵ� ȣ��
        }
    }
}
