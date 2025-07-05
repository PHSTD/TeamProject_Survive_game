using UnityEngine;

public class RocketInteractionTrigger : Structure
{
    public Rocket rocketDialogue;
    public Item requiredFinalItem; // �ν����Ϳ��� ������ ���� ���� ������

    public override void Interact()
    {
        // ��ȭ ���� �� ���̾�α� UI Ȱ��ȭ
        DayScriptSystem.Instance.ShowDialoguse();

        if (Storage.Instance.HasItem(requiredFinalItem))
        {
            // ���� �������� ���� �� ��������
            DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.StartEndingSequence());
        }
        else
        {
            // ���� �� ���� ���� ��� ���
            DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.TriggerSpaceshipDeniedEvent());
        }

        // ���� UI�� �����ַ��� �Ʒ��� Ȱ��ȭ
        rocketDialogue.gameObject.SetActive(true);
    }
}
    

