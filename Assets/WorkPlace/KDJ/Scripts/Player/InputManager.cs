using DesignPattern;
using System.Collections;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public Vector2 MouseInput { get; private set; } // ���콺 �Է�
    public Vector3 MoveDir { get; set; } // �̵� ����

    public bool IsUsingTool { get; private set; } // �׽�Ʈ�� bool ��, ���̴� �ִϸ��̼� ���� ����
    public bool CanMove => !PlayerManager.Instance.Player.IsSlipping && !PlayerManager.Instance.Player.IsUsingJetPack && PlayerManager.Instance.Player != null;
    public int CurHotbar { get; private set; } = 1; // ���� ���õ� �ֹ� ��ȣ, �ʱⰪ�� 1

    private Coroutine _itemCo;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        // Ÿ��Ʋ���� �÷����ϴ� ���
        if (SceneSystem.Instance?.GetCurrentSceneName() == SceneSystem.Instance?.GetFarmingSceneName())
        {
            PlayerInput(); // �÷��̾� �Է� ó��
            return;
        }

        // �׽�Ʈ�� ����ϴ� ���
        if (PlayerManager.Instance.Player != null)
            PlayerInput(); // �÷��̾� �Է� ó��
    }

    private void PlayerInput()
    {
        if (PlayerManager.Instance.Player == null) return; // �÷��̾ ������ �Է� ó�� �ߴ�

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        MouseInput = new Vector2(mouseX, mouseY);


        #region E Ű ��ȣ�ۿ�
        if (Input.GetKeyDown(KeyCode.E))
        {
            // ���� ������ �������� E������ ��� ��ȣ�ۿ�
            if (PlayerManager.Instance.InteractableItem != null)
            {
                Debug.Log($"[PlayerManager] EŰ ����. ��ȣ�ۿ��� ������: {PlayerManager.Instance.InteractableItem.name}");
                // PlayerManager.Instance.InteractableItem.Interact();
                PlayerManager.Instance.InteractableItem.Interact(); // �׽�Ʈ �ڵ�
            }
        }

        if (Input.GetKey(KeyCode.E) && PlayerManager.Instance.InteractableStructure != null)
        {
            if (PlayerManager.Instance.InteractableStructure.InteractCount == 0)
            {
                // �������� ��� 1�ʰ� �����߸� ��ȣ�ۿ�
                PlayerManager.Instance.InteractDelay += 1 * Time.deltaTime;

                if (PlayerManager.Instance.InteractDelay >= 1f)
                {
                    PlayerManager.Instance.InteractableStructure.Interact();
                    PlayerManager.Instance.InteractDelay = 0f; // ��ȣ�ۿ� �� ������ �ʱ�ȭ
                }
            }
            //else
            //{
            //    PlayerManager.Instance.InteractDelay = 0f; // �߰��� �ٸ����� �ٶ󺸾Ƶ� �ʱ�ȭ
            //}
        }
        else
        {
            PlayerManager.Instance.InteractDelay = 0f; // EŰ�� ���� �ʱ�ȭ
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Tab)) // 'tab' Ű�� ������ ��
        {
            SampleUIManager.Instance.ToggleInventoryUI(); // SampleUIManager�� �κ��丮 ��� �޼��� ȣ��
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            SampleUIManager.Instance.ToggleInventoryUI();
        }

        #region �ֹ� ����
        // �������� ��� ���� �ƴҶ��� ��ü
        if (!IsUsingTool)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CurHotbar = 1;
                PlayerManager.Instance.SelectItem = null;
                PlayerManager.Instance.AkimboReset();
                Destroy(PlayerManager.Instance.InHandItem);
                Destroy(PlayerManager.Instance.InHandItem2); // ��Ŵ�� ������ �� �ι�° ������ ����
                Destroy(PlayerManager.Instance.InHeadItem); // �Ӹ��� ������ ������ ����
                // �κ��丮 �ֹ� 1�� ����
                Inventory.Instance.SelectHotbarSlot(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CurHotbar = 2;
                PlayerManager.Instance.SelectItem = null;
                PlayerManager.Instance.AkimboReset();
                Destroy(PlayerManager.Instance.InHandItem);
                Destroy(PlayerManager.Instance.InHandItem2);
                Destroy(PlayerManager.Instance.InHeadItem);
                // �κ��丮 �ֹ� 2�� ����
                Inventory.Instance.SelectHotbarSlot(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CurHotbar = 3;
                PlayerManager.Instance.SelectItem = null;
                PlayerManager.Instance.AkimboReset();
                Destroy(PlayerManager.Instance.InHandItem);
                Destroy(PlayerManager.Instance.InHandItem2);
                Destroy(PlayerManager.Instance.InHeadItem);
                // �κ��丮 �ֹ� 3�� ����
                Inventory.Instance.SelectHotbarSlot(2);
            }
        }

        Item curItem = Inventory.Instance.GetCurrentHotbarItem();

        if (curItem == null)
        {
            PlayerManager.Instance.SelectItem = null; // ���� �ֹٿ� �������� ������ ���� �������� null�� ����
            if (PlayerManager.Instance.InHandItem != null)
            {
                PlayerManager.Instance.AkimboReset();
                Destroy(PlayerManager.Instance.InHandItem); // ������Ʈ ����
                Destroy(PlayerManager.Instance.InHandItem2);
                Destroy(PlayerManager.Instance.InHeadItem);
            }
            // _testHandItem = null; // �׽�Ʈ�� ���ΰǵ� null�� ����
        }
        else if (PlayerManager.Instance.SelectItem != curItem)
        {
            PlayerManager.Instance.SelectItem = curItem; // ���� �ֹٿ� �������� ������ ���� ���������� ����

            if (_itemCo == null)
            {
                PlayerManager.Instance.AkimboReset();
                Destroy(PlayerManager.Instance.InHandItem); // ������Ʈ ����
                Destroy(PlayerManager.Instance.InHandItem2);
                Destroy(PlayerManager.Instance.InHeadItem);
                // �������� ���ٸ� ���� �ڷ�ƾ ����
                _itemCo = StartCoroutine(ItemInstantiate());
            }
        }
        else
        {
            PlayerManager.Instance.SelectItem = curItem; // ���� �ֹٿ� �������� ������ ���� ���������� ����

            if (PlayerManager.Instance.InHandItem == null)
            {
                if (_itemCo == null)
                {
                    // �������� ���ٸ� ���� �ڷ�ƾ ����
                    _itemCo = StartCoroutine(ItemInstantiate());
                }
            }
        }
        #endregion

        #region ������ ���
        if (Input.GetMouseButtonDown(0) && PlayerManager.Instance.SelectItem as MaterialItem && !SampleUIManager.Instance.inventoryPanel.activeSelf)
        {
            // �տ� �ڿ� �������� ��� �ִ� ���
            // _animator.SetTrigger("Swing");
        }
        else if (Input.GetMouseButton(0) && PlayerManager.Instance.SelectItem as ToolItem && !SampleUIManager.Instance.inventoryPanel.activeSelf)
        {
            IsUsingTool = true; // ���̴� �ִϸ��̼� ������ ���� bool �� ����

            float itemUseRate = 0.1f;



            if (PlayerManager.Instance.IsAkimbo)
            {
                itemUseRate = 0.05f; // ��Ŵ�� ���¿����� ������ ��� �ӵ��� ������
            }

            // ������ ����� �߰��� ���콺�� ���� ����� �ϱ⿡ �ڷ�ƾ�� �ƴ� �׳� ����
            PlayerManager.Instance.ItemDelay += Time.deltaTime;
            if (PlayerManager.Instance.ItemDelay >= itemUseRate)
            {
                Debug.Log("������ ���!");
                PlayerManager.Instance.SelectItem?.Use(this.gameObject);
                PlayerManager.Instance.ItemDelay = 0f; // ������ ��� �� ������ �ʱ�ȭ
            }
        }
        else if (Input.GetMouseButton(0) && PlayerManager.Instance.SelectItem as ConsumableItem && !SampleUIManager.Instance.inventoryPanel.activeSelf)
        {
            PlayerManager.Instance.ItemDelay += Time.deltaTime;
            if (PlayerManager.Instance.ItemDelay >= 1f)
            {
                Debug.Log("������ ���!");
                PlayerManager.Instance.SelectItem?.Use(this.gameObject);
                PlayerManager.Instance.ItemDelay = 0f; // ������ ��� �� ������ �ʱ�ȭ
            }
        }
        else
        {
            IsUsingTool = false;
            PlayerManager.Instance.ItemDelay = 0f; // ���콺�� ���� ������ ��� ������ �ʱ�ȭ
        }
        #endregion

        // ��Ʈ���� ���߿����� ���
        if (PlayerManager.Instance.Player != null)
            if (Input.GetKeyDown(KeyCode.LeftShift) && !PlayerManager.Instance.Player.Controller.isGrounded &&
            PlayerManager.Instance.AirGauge.Value > 0 && PlayerManager.Instance.CanUseJetpack)
            {
                PlayerManager.Instance.Player.IsUsingJetPack = true;
                MoveDir = Vector3.zero; // ��Ʈ�� ���� �̵� ���� �ʱ�ȭ
            }
        if (PlayerManager.Instance.Player != null)
            if (Input.GetKeyUp(KeyCode.LeftShift) && PlayerManager.Instance.Player.IsUsingJetPack || PlayerManager.Instance.Player.Controller.isGrounded)
            {
                PlayerManager.Instance.Player.IsUsingJetPack = false;
            }

        if (!CanMove) return; // ������ �� ���ٸ� �̵��� ����

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        MoveDir = new Vector3(x, 0, y).normalized;
    }

    IEnumerator ItemInstantiate()
    {
        // ������ ���� �����̸� ���� �ڷ�ƾ
        // ������ ������ ������ ������ interact �� �� �����鿡 �Ҵ� �� �̷������ �ϱ⿡ �����̸� �߰���
        yield return new WaitForSeconds(0.01f);

        Item curItem = Inventory.Instance.GetCurrentHotbarItem();

        if (PlayerManager.Instance.IsAkimbo)
        {
            // ��Ŵ���� ��� ��տ� �� ����
            PlayerManager.Instance.InHandItem = Instantiate(curItem.HandleItem,
                PlayerManager.Instance.Player.PlayerRightHand.position,
                PlayerManager.Instance.Player.PlayerRightHand.rotation);
            PlayerManager.Instance.InHandItem2 = Instantiate(curItem.HandleItem,
                PlayerManager.Instance.Player.PlayerLeftHand.position,
                PlayerManager.Instance.Player.PlayerLeftHand.rotation);
            PlayerManager.Instance.InHeadItem = Instantiate(PlayerManager.Instance.SunGlasses,
                PlayerManager.Instance.Player.PlayerHead.position,
                PlayerManager.Instance.Player.PlayerHead.rotation);
        }
        else
        {
            PlayerManager.Instance.InHandItem = Instantiate(curItem.HandleItem,
            PlayerManager.Instance.Player.PlayerRightHand.position,
            PlayerManager.Instance.Player.PlayerRightHand.rotation);
        }

        _itemCo = null; // �ڷ�ƾ ���� �� null�� ����
    }
}
