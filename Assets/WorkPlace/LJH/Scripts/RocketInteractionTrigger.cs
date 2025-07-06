using UnityEngine;

public class RocketInteractionTrigger : Structure
{
    public Rocket rocketDialogue;
    public Item requiredFinalItem; // �ν����Ϳ��� ������ ���� ���� ������


    public override void Interact()
    {
        DayScriptSystem.Instance.ShowDialoguse();
        Cursor.lockState = CursorLockMode.None;
        if (PlayerManager.Instance.EventCount == 0)
        {
            DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.TriggerFirstSpaceshipScene());
            PlayerManager.Instance.EventCount++;
        }
        else
        {
            DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.TriggerSpaceshipDeniedEvent());
        }

        // ��ȭ ���� �� ���̾�α� UI Ȱ��ȭ
        if (Storage.Instance.HasItem(requiredFinalItem) && PlayerManager.Instance.EventCount == 1)
        {
            // ���� �������� ���� �� ��������
            DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.StartEndingSequence());
            PlayerManager.Instance.EventCount++;
        }
        else if (PlayerManager.Instance.EventCount == 2)
        {
            // ���� ��ũ��Ʈ ���
            DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.EndingScript());
        }

        // ���� UI�� �����ַ��� �Ʒ��� Ȱ��ȭ
        // rocketDialogue.gameObject.SetActive(true);
    }
}
    

