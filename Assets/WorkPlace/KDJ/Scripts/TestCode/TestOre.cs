using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOre : MonoBehaviour
{
    private float hp;

    private void Awake()
    {
        hp = 100;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        Debug.Log($"{gameObject.name}��(��) {damage}�� ���ظ� �޾ҽ��ϴ�. ���� HP: {hp}");
    }
}
