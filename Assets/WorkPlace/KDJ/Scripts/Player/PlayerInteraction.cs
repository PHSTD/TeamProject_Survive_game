using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private LayerMask _layerMask = 1 << 6;
    private RaycastHit _rayHit;
    private Vector3 _rayEndPos;
    private Collider[] _colls = new Collider[10];

    public Vector3 GroundNormal { get; private set; } = Vector3.up; // ���� ���� ����
    public Collider CurHitColl { get; private set; } // ���� �浹�� �ݶ��̴�
    public Collider LastHitColl { get; private set; } // ���������� �浹�� �ݶ��̴�
    public float GroundCos { get; private set; } // ���� ���� ���Ϳ� Y���� �ڻ��� ��
    public float SlopeCos { get; private set; } // �÷��̾��� ������ ���� ������ ���� �ڻ��� ��
    public bool IsRayHit { get; private set; } // ����ĳ��Ʈ�� �����ߴ��� ����


    private void Start()
    {
        Init();
    }

    private void OnDrawGizmos()
    {
        // Gizmos�� ����Ͽ� ���� ǥ��
        Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.92f, 0), 2.5f);

        if (IsRayHit)
        {
            Gizmos.DrawLine(Camera.main.transform.position, _rayHit.point);
            Gizmos.DrawWireSphere(_rayHit.point, 2.5f);
        }
        else
        {
            Gizmos.DrawLine(Camera.main.transform.position, _rayEndPos);
            Gizmos.DrawWireSphere(_rayEndPos, 2.5f);
        }
    }

    private void Init()
    {
        SlopeCos = Mathf.Cos(PlayerManager.Instance.Player.Controller.slopeLimit * Mathf.Deg2Rad);
    }

    private void Update()
    {
        FindCloseInteractableItemAtRay();
    }

    private void LateUpdate()
    {
        OnControllerColliderExit(); // LateUpdate���� �ݶ��̴��� ���� �ʴ� ��츦 ó��
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        CurHitColl = hit.collider; // ���� �浹�� �ݶ��̴��� ����
        GroundNormal = hit.normal; // �浹�� ǥ���� ���� ���͸� ����
        GroundCos = Vector3.Dot(hit.normal, Vector3.up);
    }

    private void OnControllerColliderExit()
    {
        // �ݶ��̴��� ��Ҿ��µ� ������ ���� �ʴ� ���
        if (LastHitColl != null && CurHitColl == null)
        {
            // �������� ���� �������� ����
            PlayerManager.Instance.Player.FixedDir = transform.TransformDirection(InputManager.Instance.MoveDir) * PlayerManager.Instance.Speed * 0.5f;
        }

        LastHitColl = CurHitColl; // ���� �ݶ��̴��� ������ �ݶ��̴��� ����
        CurHitColl = null; // ���� �ݶ��̴��� null�� �ʱ�ȭ
    }

    public void FindCloseInteractableItemAtRay()
    {
        // overlapsphere�� �÷��̾� ��ġ�� �ƴ� ������ ���������� ����
        // ��ũ�� �߾� ���� ���� ������ �ִ� ������Ʈ�� _interactableItem�� ����
        // ����ĳ��Ʈ�� �߾��� �����ϰ� ������ hit ���� �Ÿ� ���
        Collider closestColl = null;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        IsRayHit = Physics.Raycast(ray, out _rayHit, 6f);
        int collsCount = 0;
        Gizmos.color = Color.green;

        if (IsRayHit)
        {
            // ����ĳ��Ʈ�� �����ϸ� hit.point�� �������� overlapsphere�� ����
            collsCount = Physics.OverlapSphereNonAlloc(_rayHit.point, 2.5f, _colls, _layerMask);
        }
        else
        {
            // ���н� ī�޶��� ��ġ���� ���� �������� 6f ������ �������� overlapsphere�� ����
            _rayEndPos = PlayerManager.Instance.Player.VirCamAxis.position + Camera.main.transform.forward * 2f;
            collsCount = Physics.OverlapSphereNonAlloc(_rayEndPos, 2.5f, _colls, _layerMask);
        }

        if (collsCount > 0)
        {
            for (int i = 0; i < collsCount; i++)
            {
                if (IsRayHit)
                {
                    // ����ĳ��Ʈ�� ������ ��� hit.point���� ������Ʈ�� �Ÿ� ����
                    float distance = Vector3.Distance(_rayHit.point, _colls[i].transform.position);
                    // closestColl�� null�̰ų� ���� ������Ʈ�� closestColl���� ����� ��� ���� �ε����� �ݶ��̴��� closestColl�� ����
                    if (closestColl == null || distance < Vector3.Distance(_rayHit.point, closestColl.transform.position))
                    {
                        closestColl = _colls[i];
                    }
                }
                else
                {
                    // ����ĳ��Ʈ�� ������ ��� rayEndPos���� ������Ʈ�� �Ÿ� ����
                    float distance = Vector3.Distance(_rayEndPos, _colls[i].transform.position);
                    // closestColl�� null�̰ų� ���� ������Ʈ�� closestColl���� ����� ��� ���� �ε����� �ݶ��̴��� closestColl�� ����
                    if (closestColl == null || distance < Vector3.Distance(_rayEndPos, closestColl.transform.position))
                    {
                        closestColl = _colls[i];
                    }
                }
            }
            // ������ closestColl�� ������ _interactableItem�� �Ҵ�
            if (closestColl != null && closestColl.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                if (interactable as WorldItem)
                {
                    PlayerManager.Instance.InteractableItem = interactable as WorldItem;
                    PlayerManager.Instance.IsInIntercation = true;
                }
                else if (interactable as Structure)
                {
                    PlayerManager.Instance.InteractableStructure = interactable as Structure;
                    PlayerManager.Instance.IsInIntercation = true;
                }
                // �Ʒ��� �׽�Ʈ �ڵ�
                else if (interactable as TestWorldItem)
                {
                    PlayerManager.Instance.InteractableTestItem = interactable as TestWorldItem;
                    PlayerManager.Instance.IsInIntercation = true;
                }
            }
        }
        else
        {
            // �ֺ��� ���ͷ��� ������ ������Ʈ�� ������ ��ȣ�ۿ��� null�� ����
            PlayerManager.Instance.InteractableStructure = null;
            PlayerManager.Instance.InteractableItem = null;
            PlayerManager.Instance.IsInIntercation = false;
        }
    }
}
