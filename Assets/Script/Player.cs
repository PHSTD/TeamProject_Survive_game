using UnityEngine;

public class TPSController : MonoBehaviour
{
    // �̵� �� ȸ�� �ӵ�
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 200.0f;

    // ī�޶� ���� ȸ���� ���� ����
    public Transform cameraHolder;
    private float verticalLookRotation;

    // �ڡڡ� �߷� ���� ���� �߰� �ڡڡ�
    public float jumpSpeed = 8.0f; // ���� �ӵ� (Ŭ���� ��� ���� ������ �߰�)
    public float gravity = 9.81f; // �߷°�
    private float yVelocity = 0;  // ĳ������ ���� �ӵ�

    // �ʼ� ������Ʈ
    private CharacterController charController;

    void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. �÷��̾� ȸ�� (���콺 �¿�)
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
        // 2. ī�޶� ȸ�� (���콺 ����)
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60f, 60f);
        cameraHolder.localEulerAngles = new Vector3(verticalLookRotation, 0, 0);

        // 3. �÷��̾� �̵� (WASD)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(h, 0, v);
        // �̵� ������ �÷��̾ �ٶ󺸴� ���� �������� ��ȯ
        moveDirection = transform.TransformDirection(moveDirection);

        // �ڡڡ� �߷� ���� ���� �ڡڡ�

        // isGrounded�� CharacterController�� ���� ����ִ��� ���θ� �˷���
        if (charController.isGrounded)
        {
            yVelocity = -1.0f;

            if (Input.GetButtonDown("Jump"))
            {
                yVelocity = jumpSpeed;
            }
        }
        else
        {
            yVelocity -= gravity * Time.deltaTime;  // �߷� ����
        }

        // ���� �ӵ�(yVelocity)�� �̵� ������ y���� ����
        moveDirection.y = yVelocity;

        // �ڡڡ� �߷� ���� �� �ڡڡ�
        // �ڡڡ� ���� ��� �߰� �ڡڡ�


        // �߷� ���� ����, charController.Move() ������ �߰��� �ڵ�:
        // ���� �Է� ���� (�����̽���)
        if (Input.GetButtonDown("Jump") && charController.isGrounded)
        {
            // ���� ���� ���� ���� ����
            // yVelocity�� ���� �ӵ��� ������
            yVelocity = jumpSpeed;
        }
        // �ڡڡ� ���� ��� �� �ڡڡ�
        
      
// ���������� ���� ����� �ӵ��� ĳ���͸� �̵���Ŵ
// ���� Y�� ������(�߷�)�� ���ԵǾ� ����
charController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
}