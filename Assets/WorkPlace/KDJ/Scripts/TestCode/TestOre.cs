using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOre : MonoBehaviour
{
    private int hp;

    private void Awake()
    {
        hp = 10;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        Debug.Log($"{gameObject.name}��(��) {damage}�� ���ظ� �޾ҽ��ϴ�. ���� HP: {hp}");
    }
}
