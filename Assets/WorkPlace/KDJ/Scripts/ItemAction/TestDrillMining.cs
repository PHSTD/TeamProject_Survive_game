using UnityEngine;

public class TestDrillMining : ToolAction
{
    [SerializeField] private GameObject _miningEffect;

    private Collider[] _colls =  new Collider[10];
    private LayerMask _layerMask = 1 << 10;
    private bool _isMining = false;

    public override void Action(int power)
    {
        if(PlayerManager.Instance.InHandItem == null)
        {
            Debug.Log("InHandItem is null. Cannot perform mining action.");
            return;
        }

        Vector3 miningPos = PlayerManager.Instance.InHandItem.transform.position
            + PlayerManager.Instance.Player.transform.forward * 1.5f;

        // ���� ü�� ����
        int count = Physics.OverlapSphereNonAlloc(miningPos, 3f, _colls, _layerMask);

        if (count > 0)
        {
            _isMining = true;
            for (int i = 0; i < count; i++)
            {
                bool s = _colls[i].TryGetComponent<MineableResource>(out MineableResource ore);

                if (!s)
                {
                    continue;
                }

                Ray ray = new Ray(PlayerManager.Instance.InHandItem.transform.position, (_colls[i].transform.position - PlayerManager.Instance.InHandItem.transform.position).normalized);
                bool rc = Physics.Raycast(ray, out RaycastHit hit, 4.5f, _layerMask);
                GameObject effect = Instantiate(_miningEffect, hit.point, Quaternion.identity);
                effect.transform.LookAt(PlayerManager.Instance.InHandItem.transform.position);

                PlayerManager.Instance.HitInfo = hit;

                ore.TakeMiningDamage(power);
            }
        }
        else
        {
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
