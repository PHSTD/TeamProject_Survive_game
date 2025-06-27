using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

[CreateAssetMenu(menuName = "Item/Tool Item")]
public class ToolItem : Item
{
    [Header("Tool Specific Info")]
    public float miningPower = 1f; // ä�� ������ ä���� (��: ������ ü���� ��� ��)
    public ToolType toolType;       // �� ������ ���� (��: ���, ����, �� ��)

    public override void Use(GameObject user)
    {
        // �θ� Item Ŭ������ Use �޼ҵ带 ȣ���մϴ�.
        base.Use(user); // ����� ��

        Debug.Log($"{itemName}�� ����߽��ϴ�. (������ ����)");

        // ������ ���� ������ �����Ƿ�, ���⼭�� �߰����� ������ ������ �����ϰų�
        // �÷��̾� ��Ʈ�ѷ����� ó���� �׼��� Ʈ�����ϵ��� �մϴ�.
        // ��: �÷��̾� ��Ʈ�ѷ����� ����ĳ��Ʈ�� ���� ä�� ������ ����� ã�� �����ϵ��� ���
    }
}

public enum ToolType
{
    //������ ���� ����
    None,
    Pickaxe,
    Axe,
    Shovel
}
