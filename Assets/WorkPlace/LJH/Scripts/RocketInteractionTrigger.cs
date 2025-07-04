using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketInteractionTrigger : Structure
{
    public Rocket rocketDialogue;

    public override void Interact()
    {
        // ��ȭ UI �Ѱ� ù �� ǥ��
        rocketDialogue.gameObject.SetActive(true);   // RenpyCanvas�� ����
        
        //UIManager.Instance.HidePopup();

        // �÷��̾� �̵�/���� ���
        //InputManager.Instance.LockPlayerControl(true);

        // ���� �Ͻ� ����
        Time.timeScale = 0f;

        // Ŀ�� Ȱ��(PC���)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        enabled = false;               // ������ ����
    }

}
