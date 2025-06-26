using Cinemachine;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region ����ȭ ����
    [SerializeField] private CharacterController _controller;
    [SerializeField] private CinemachineVirtualCamera _virCam;
    [SerializeField] private Transform _virCamAxis;

    [Header("Config")]
    [SerializeField][Range(0, 5)] private float _mouseSensitivity = 1;
    [SerializeField] private float _speed;
    [SerializeField] private float _baseSpeed = 5f;
    #endregion

    #region ����
    private Vector3 _verVelocity;
    private Vector3 _moveDir;
    private Vector2 _mouseInput;
    private float _totalMouseY = 0f;
    private LayerMask _ignoreMask = ~(1 << 3);
    private LayerMask _layerMask = 1 << 6;
    private Collider[] _colls = new Collider[10];
    private Item _selectItem;
    private WorldItem _worldItem;
    private Coroutine _itemUseCoroutine;
    private Coroutine _interactCoroutine;
    private float _interactionDelay;
    private bool _canUseItem => _itemUseCoroutine == null;
    private Animator _animator;
    private bool _isMoving => _moveDir != Vector3.zero;
    // private bool _isGrabbing => _selectItem != null;
    // �Ʒ��� �׽�Ʈ �ڵ�
    private bool _isGrabbing = false;
    #endregion

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        PlayerInput();
        HandlePlayer();
        Animation();
    }

    private void Init()
    {
        // �׽�Ʈ�� ���콺 �����
        _animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _speed = _baseSpeed;
    }

    #region ��ȣ�ۿ� : �÷��̾ ���� ����� ��ü
    private void FindCloseInteractableItemFromPlayer()
    {
        // Ʈ���Ÿ� ������� �ʰ� overlapsphere�� ����Ͽ� �ֺ��� ���ͷ��� ������ ������Ʈ ����
        // �÷��̾�κ��� ���� ������ �ִ� ������Ʈ�� _interactableItem�� ����
        Collider closestColl = null;
        int collsCount = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, 0.92f, 0), 2.5f, _colls, _layerMask);
        if (collsCount > 0)
        {
            for (int i = 0; i < collsCount; i++)
            {
                // �÷��̾�� ������Ʈ�� �Ÿ� ����
                float distance = Vector3.Distance(transform.position, _colls[i].transform.position);
                // closestColl�� null�̰ų� ���� ������Ʈ�� closestColl���� ����� ��� ���� �ε����� �ݶ��̴��� closestColl�� ����
                if (closestColl == null || distance < Vector3.Distance(transform.position, closestColl.transform.position))
                {
                    closestColl = _colls[i];
                }
            }
            // ������ closestColl�� ������ _interactableItem�� �Ҵ�
            if (closestColl != null && closestColl.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                //_interactableItem = interactable as TestItem;
                // ���߿� �����۰� ��ȣ�ۿ� ��ü�� �����ٰ� �ϸ�
                // _interactableItem�� as�� ������ ���ǹ��� �̿��Ͽ� ��Ȳ�� �°� �ִ� ���� �ʿ�
                // Item�̶�� as Item����, �������̶�� as Structure�� �ִ� ������
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
    #endregion

    #region ��ȣ�ۿ� : ȭ�� �߾ӿ� ���� ����� ��ü
    private void FindCloseInteractableItemFromRay()
    {
        // Ʈ���Ÿ� ������� �ʰ� overlapsphere�� ����Ͽ� �ֺ��� ���ͷ��� ������ ������Ʈ ����
        // ��ũ�� �߾� ���� ���� ������ �ִ� ������Ʈ�� _interactableItem�� ����
        // ����ĳ��Ʈ�� �߾��� �����ϰ� ������ hit ���� �Ÿ� ���
        Collider closestColl = null;
        int collsCount = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, 0.92f, 0), 2.5f, _colls, _layerMask);
        // ����ĳ��Ʈ�� �߾��� �����ϰ� ������ hit ���� �Ÿ� ���
        
        if (collsCount > 0)
        {
            for (int i = 0; i < collsCount; i++)
            {
                // ȭ�� �߾ӿ��� ������Ʈ�� �Ÿ� ����
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit hit;
                bool isHit = Physics.Raycast(ray, out hit, 10f);
                float distance = Vector3.Distance(hit.point, _colls[i].transform.position);
                // closestColl�� null�̰ų� ���� ������Ʈ�� closestColl���� ����� ��� ���� �ε����� �ݶ��̴��� closestColl�� ����
                if (closestColl == null || distance < Vector3.Distance(hit.point, closestColl.transform.position))
                {
                    closestColl = _colls[i];
                }
            }
            // ������ closestColl�� ������ _interactableItem�� �Ҵ�
            if (closestColl != null && closestColl.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                //_interactableItem = interactable as TestItem;
                // ���߿� �����۰� ��ȣ�ۿ� ��ü�� �����ٰ� �ϸ�
                // _interactableItem�� as�� ������ ���ǹ��� �̿��Ͽ� ��Ȳ�� �°� �ִ� ���� �ʿ�
                // Item�̶�� as Item����, �������̶�� as Structure�� �ִ� ������
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
    #endregion

    private void HandlePlayer()
    {
        AimControl();
        Move();
        Jump();
        Run();
        CameraLimit();
        //FindCloseInteractableItemFromPlayer();
        FindCloseInteractableItemFromRay();
    }

    private void PlayerInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        _moveDir = new Vector3(x, 0, y).normalized;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _mouseInput = new Vector2(mouseX, mouseY);


        #region E Ű ��ȣ�ۿ�
        if (Input.GetKeyDown(KeyCode.E))
        {
            // ���� ������ �������� E������ ��� ��ȣ�ۿ�
            if (PlayerManager.Instance.InteractableItem != null)
                PlayerManager.Instance.InteractableItem.Interact();
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (PlayerManager.Instance.InteractableStructure != null)
            {
                // �������� ��� 1�ʰ� �����߸� ��ȣ�ۿ�
                PlayerManager.Instance.InteractDelay += Time.deltaTime;
                if (PlayerManager.Instance.InteractDelay >= 1f)
                {
                    PlayerManager.Instance.InteractableStructure.Interact();
                    PlayerManager.Instance.InteractDelay = 0f; // ��ȣ�ۿ� �� ������ �ʱ�ȭ
                }
            }
        }
        else
        {
            PlayerManager.Instance.InteractDelay = 0f; // E Ű�� ���� ������ �ʱ�ȭ
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Q)) // 'Q' Ű�� ������ ��
        {
            SampleUIManager.Instance.ToggleInventoryUI(); // SampleUIManager�� �κ��丮 ��� �޼��� ȣ��
        }


        #region ������ ���
        if (Input.GetMouseButton(0) && _selectItem != null)
        {
            // ������ ����� ������ ��Ʈ���� ���� �Ǹ� ���� ����
            // �ϴ��� �������� �������� ����ϴ� �ڷ�ƾ�� ���� �����ϱ�
            if (_canUseItem)
            {
                _itemUseCoroutine = StartCoroutine(ItemUsing());
            }

            // �Һ� �������� ��� �� �������� ���ǵ��� �����ش޶�� ��û����
            // �ش� �κ� ���� �ʿ�
        }
        #endregion

        // �׽�Ʈ �ڵ�
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isGrabbing = !_isGrabbing; // Grab ���� ���
        }
    }

    #region �÷��̾� �̵�
    private void Move()
    {
        // ī�޶� �������� ������ ��� �����̵��� �����ؾ���
        Vector3 move = transform.TransformDirection(_moveDir) * _speed;

        // ȭ���� ����̴� �߷��� 3.73
        _verVelocity.y -= 3.73f * Time.deltaTime;

        _controller.Move((move + _verVelocity) * Time.deltaTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded)
        {
            _verVelocity.y = 5f; // Jump force
        }
    }

    private void Run()
    {
        // �޸��� ����� �ʿ����� ���� ���� ����.
        // �ִ´ٸ� ���ο�� ��ȣ�ۿ� ����Ұ�.
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speed *= 2;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed /= 2;
        }
    }
    #endregion

    #region �÷��̾� ���� ����
    private void AimControl()
    {
        Vector2 mouseInput = _mouseInput * _mouseSensitivity;
        _totalMouseY = Mathf.Clamp(_totalMouseY - mouseInput.y, -75, 75);

        float clampedMouseY = _virCam.transform.localEulerAngles.x - mouseInput.y;
        if (clampedMouseY > 180)
        {
            clampedMouseY -= 360;
        }

        _virCamAxis.transform.localRotation = Quaternion.Euler(_totalMouseY, 0, 0);

        transform.Rotate(Vector3.up * mouseInput.x);
    }

    private void CameraLimit()
    {
        // ī�޶� ���� �ε����� ��� �� ��ġ��ŭ ������ �̵�
        // �࿡�� ī�޶�� ���̸� �߻��Ͽ� ���� �ε����� ���, �ε����� ��ġ�� ����Ͽ� ī�޶� �̵���Ŵ
        RaycastHit hit;
        if (Physics.Raycast(_virCamAxis.position, -_virCamAxis.forward, out hit, 4.5f, _ignoreMask))
        {
            Vector3 targetPos = hit.point + _virCam.transform.forward * 0.5f;
            _virCam.transform.position = Vector3.Lerp(_virCam.transform.position, targetPos, 0.5f);
        }
        else
        {
            // ���� �ε����� �ʴ� ��� ��ġ ����
            Vector3 resetPos = _virCamAxis.position - _virCamAxis.forward * 4f;
            _virCam.transform.position = Vector3.Lerp(_virCam.transform.position, resetPos, 0.5f);
        }
    }
    #endregion

    IEnumerator ItemUsing()
    {
        // ������ ���� ��� �ڷ�ƾ
        // ������ ��밣�� ������ ����
        // �Ʒ��� �ӽ�
        _selectItem.Use(this.gameObject);
        yield return new WaitForSeconds(1f);
        _itemUseCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        // Gizmos�� ����Ͽ� ���� ǥ��
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.92f, 0), 2.5f);
    }

    #region �̻�� �ڵ�
    /// <summary>
    /// ���ο� ������ �ۼ��������� �Է� �޾� �÷��̾� ����
    /// </summary>
    /// <param name="percentage"></param>0~100 ������ ������ �Է�, 0�� ���� ����, 100�� ����
    //public void PlayerSlow(float percentage)
    //{
    //    _speed = _speed * (1f - percentage / 100f);
    //}
    //
    /// <summary>
    /// ���ο��� �������� ����ϱ⿡ ���ο� �� �ۼ��������� �״�� �Է��ؾ���
    /// </summary>
    /// <param name="percentage"></param>
    //public void OutOfSlow(float percentage)
    //{
    //    // ���ο��� ����
    //    _speed = _speed / (1f - percentage / 100f);
    //}

    // overlapsphere�� ��ü�ϱ⿡ �ּ�ó��.
    // ���� ���ͷ��� ����� �Ѱ��� �ϳ��� ������Ʈ�� �ִٴ� �� ������ ���۵�
    // ������ ó�� ������ ������Ʈ�� ���ͷ����� �����ϵ��� ����
    // ���� ������Ʈ�� ���� ���, ���� ����� �ƴ� �ٸ� ������� �ڵ带 �ۼ��Ͽ��� ��
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.TryGetComponent<IInteractable>(out IInteractable interact) && _interactableItem == null)
    //     {
    //         _interactableItem = interact as TestItem;
    //         TestPlayerManager.Instance.IsInIntercation = true;
    //         // ���߿� �����۰� ��ȣ�ۿ� ��ü�� �����ٰ� �ϸ�
    //         // _interactableItem�� as�� ������ ���ǹ��� �̿��Ͽ� ��Ȳ�� �°� �ִ� ���� �ʿ�
    //         // Item�̶�� as Item����, �������̶�� as Structure�� �ִ� ������
    //     }
    // }
    // 
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.TryGetComponent<IInteractable>(out IInteractable interact))
    //     {
    //         TestPlayerManager.Instance.IsInIntercation = false;
    //         _interactableItem = null;
    //     }
    // }
    #endregion

    #region �ִϸ��̼�
    private void Animation()
    {
        MoveAnim();
        GrabAnim();
        SwingAnim();
    }

    private void MoveAnim()
    {
        _animator.SetBool("IsWalking", _isMoving);
    }

    private void GrabAnim()
    {
        _animator.SetBool("IsGrabbing", _isGrabbing);
    }

    private void SwingAnim()
    {
        // ���߿� _isGrabbing�� ���ǿ� �߰��ǵ��� ����
        if (Input.GetMouseButtonDown(0) && _isGrabbing)
        {
            _animator.SetTrigger("Swing");
        }
    }
    #endregion
}
