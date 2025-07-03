using UnityEngine;

public class TestDrillMining : ToolAction
{
    private Collider[] _colls =  new Collider[10];
    private LayerMask _layerMask = 1 << 10;
    private bool _isMining = false;

    public override void Action(int power)
    {
        // ���� ü�� ����
        int count = Physics.OverlapSphereNonAlloc(PlayerManager.Instance.InHandItem.transform.position 
            + PlayerManager.Instance.Player.transform.forward * 1.5f, 3f, _colls, _layerMask);

        Debug.Log("���� ���� ���̾�" + _layerMask.value);
        
        if (count > 0)
        {
            Debug.Log($"�ֺ� ���� ����: {count}");
            _isMining = true;
            for (int i = 0; i < count; i++)
            {
                //_colls[i].GetComponent<TestOre>()?.TakeDamage(power);
                bool s = _colls[i].TryGetComponent<MineableResource>(out MineableResource ore);
                if (!s)
                {
                    continue;
                }
                ore.TakeMiningDamage(power);
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
        if(PlayerManager.Instance.InHandItem == null)
            return;
    
        if (_isMining)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(PlayerManager.Instance.InHandItem.transform.position 
                + PlayerManager.Instance.Player.transform.forward * 1.5f, 3f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(PlayerManager.Instance.InHandItem.transform.position
                + PlayerManager.Instance.Player.transform.forward * 1.5f, 3f);
        }
    }
}
