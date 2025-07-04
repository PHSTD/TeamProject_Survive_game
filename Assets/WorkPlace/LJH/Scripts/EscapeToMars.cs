using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class EscapeToMars : Structure
{
    public override void Interact()
    {
        ShowMessage("Ż�� ���� ��ũ��Ʈ ����");

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
