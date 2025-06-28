using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class TestDrillMining : ToolAction
{
    private Collider[] _colls =  new Collider[10];
    private LayerMask _layerMask = 1 << 8;
    private bool _isMining = false;

    public override void Action(int power)
    {
        // ���� ü�� ����
        int count = Physics.OverlapSphereNonAlloc(PlayerManager.Instance.Player._testHandItem.transform.position 
            + PlayerManager.Instance.Player.transform.forward * 1.5f, 3f, _colls, _layerMask);
        
        if (count > 0)
        {
            _isMining = true;
            for (int i = 0; i < count; i++)
            {
                //_colls[i].GetComponent<TestOre>()?.TakeDamage(power);
                bool s = _colls[i].TryGetComponent<TestOre>(out TestOre ore);
                if (!s)
                {
                    continue;
                }
                ore.TakeDamage(power);
            }
        }
        else
        {
            Debug.Log("�ֺ��� ������ �����ϴ�.");
            _isMining = false;
        }
    }

    private void OnDrawGizmos()
    {
        if(PlayerManager.Instance.Player._testHandItem == null)
            return;
    
        if (_isMining)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(PlayerManager.Instance.Player._testHandItem.transform.position 
                + PlayerManager.Instance.Player.transform.forward * 1.5f, 3f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(PlayerManager.Instance.Player._testHandItem.transform.position
                + PlayerManager.Instance.Player.transform.forward * 1.5f, 3f);
        }
    }
}
