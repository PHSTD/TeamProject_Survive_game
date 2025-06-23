using Cinemachine;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private CinemachineVirtualCamera _virCam;
    [SerializeField] private Transform _virCamAxis;

    [Header("Config")]
    [SerializeField][Range(0, 5)] private float _mouseSensitivity = 1;
    [SerializeField] private float _speed;

    private Vector3 _verVelocity;
    private Vector3 _moveDir;
    private Vector2 _mouseInput;
    private float _totalMouseY = 0f;
    private TestItem _interactableItem;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        PlayerInput();
        HandlePlayer();
    }

    // ���� ���ͷ��� ����� �Ѱ��� �ϳ��� ������Ʈ�� �ִٴ� �� ������ ���۵�
    // ������ ó�� ������ ������Ʈ�� ���ͷ����� �����ϵ��� ����
    // ���� ������Ʈ�� ���� ���, ���� ����� �ƴ� �ٸ� ������� �ڵ带 �ۼ��Ͽ��� ��
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interact) && _interactableItem == null)
        {
            _interactableItem = interact as TestItem;
            TestPlayerManager.Instance.IsInIntercation = true;
            // ���߿� �����۰� ��ȣ�ۿ� ��ü�� �����ٰ� �ϸ�
            // _interactableItem�� as�� ������ ���ǹ��� �̿��Ͽ� ��Ȳ�� �°� �ִ� ���� �ʿ�
            // Item�̶�� as Item����, �������̶�� as Structure�� �ִ� ������
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interact))
        {
            TestPlayerManager.Instance.IsInIntercation = false;
            _interactableItem = null;
        }
    }

    private void Init()
    {
        // �׽�Ʈ�� ���콺 �����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandlePlayer()
    {
        AimControl();
        Move();
        Jump();
        Run();
        CameraLimit();
    }

    private void Move()
    {
        // ī�޶� �������� ������ ��� �����̵��� �����ؾ���
        Vector3 move = transform.TransformDirection(_moveDir) * _speed;

        // ȭ���� ����̴� �߷��� 3.73
        _verVelocity.y -= 3.73f * Time.deltaTime;

        _controller.Move((move + _verVelocity) * Time.deltaTime);
    }

    private void PlayerInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        _moveDir = new Vector3(x, 0, y).normalized;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _mouseInput = new Vector2(mouseX, mouseY);

        if(Input.GetKeyDown(KeyCode.E) && _interactableItem != null)
        {
            _interactableItem.Interact();
        }
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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _speed = 10f;
        }
        else
        {
            _speed = 5f;
        }
    }

    private void AimControl()
    {
        Vector2 mouseInput = _mouseInput * _mouseSensitivity;
        _totalMouseY = Mathf.Clamp(_totalMouseY - mouseInput.y, -50, 50);

        float clampedMouseY = _virCam.transform.localEulerAngles.x - mouseInput.y;
        if (clampedMouseY > 180)
        {
            clampedMouseY -= 360;
        }

        // �����ϸ� y�� ������ ���� ������ �Ѿ���� ī�޶� ȸ���� �ϴ°� �ƴ� ��ġ�� �̵��ϵ��� ����
        // ī�޶� ���Ϸ� ������ ������ ȸ���ϴ� ��� ī�޶�� ���� �κи� �̵��ϰ� �� ���ķ� ���� ȸ��
        if (_totalMouseY > 30)
        {
            _virCamAxis.transform.localRotation = Quaternion.Euler(_totalMouseY - 30, 0, 0);
        }
        else if (_totalMouseY < -30)
        {
            _virCamAxis.transform.localRotation = Quaternion.Euler(_totalMouseY + 30, 0, 0);
        }
        else
        {
            _virCam.transform.localRotation = Quaternion.Euler(clampedMouseY, 0, 0);
        }

        transform.Rotate(Vector3.up * mouseInput.x);
    }

    private void CameraLimit()
    {
        // ī�޶� ���� �ε����� ��� �� ��ġ��ŭ ������ �̵�
        // �࿡�� ī�޶�� ���̸� �߻��Ͽ� ���� �ε����� ���, �ε����� ��ġ�� ����Ͽ� ī�޶� �̵���Ŵ
        RaycastHit hit;
        if (Physics.Raycast(_virCamAxis.position, (_virCam.transform.position - _virCamAxis.position).normalized, out hit, 9.3f))
        {
            Vector3 targetPos = hit.point + _virCam.transform.forward * 0.3f;
            _virCam.transform.position = Vector3.Lerp(_virCam.transform.position, targetPos, 0.5f); // ī�޶� ���� �ʹ� ���� �ʵ��� �ణ�� ������ �߰�
        }
        else
        {
            Vector3 resetPos = _virCamAxis.position - _virCamAxis.forward * 9f;
            _virCam.transform.position = Vector3.Lerp(_virCam.transform.position, resetPos, 0.5f); // ���� �ε����� �ʴ� ��� �⺻ ��ġ�� �̵�
        }
    }

    private void OnDrawGizmos()
    {
        // Gizmos�� ����Ͽ� ���� ǥ��
        Gizmos.color = Color.red; // ���� ���� ����
        Gizmos.DrawLine(_virCamAxis.position, _virCamAxis.position - _virCamAxis.forward * 9f); // ���� ǥ�ÿ�
    }
}
