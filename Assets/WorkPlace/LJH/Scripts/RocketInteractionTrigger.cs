using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class RocketInteractionTrigger : Structure
{
    public Rocket rocketDialogue;
    string message;
    public override void Interact()
    {

        // ��ȭ UI �Ѱ� ù �� ǥ��
        //rocketDialogue.gameObject.SetActive(true);   // RenpyCanvas�� ����

        //UIManager.Instance.HidePopup();

        // �÷��̾� �̵�/���� ���
        //InputManager.Instance.LockPlayerControl(true);

        // ���� �Ͻ� ����
        //Time.timeScale = 0f;

        ShowMessage(message);

        // Ŀ�� Ȱ��(PC���)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        enabled = false;               // ������ ����
    }

    private void ShowMessage(string message)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPopup(message);
        }
        else
        {
            Debug.LogWarning("UIManager�� ���� �����ϴ�.");
        }
    }
}
