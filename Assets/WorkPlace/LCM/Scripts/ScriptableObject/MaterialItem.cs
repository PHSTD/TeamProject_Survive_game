using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Item/Material Item")]
public class MaterialItem : Item
{
    //��͵� ���� �������� �Ӽ� �߰��� �̰���

    public override void Use(GameObject user)
    {
        Debug.Log($"{itemName}�� ���� ����Ҽ� �����ϴ�.");
    }
}
