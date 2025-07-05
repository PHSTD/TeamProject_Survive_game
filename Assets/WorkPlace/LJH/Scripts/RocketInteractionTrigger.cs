using UnityEngine;

public class RocketInteractionTrigger : Structure
{
    public Rocket rocketDialogue; // Ȥ�� �� ���� ���� ���
    public Item finalItem;        // ���� ���������� ����� ScriptableObject

    public override void Interact()
    {
        // UI ����
        DayScriptSystem.Instance.ShowDialoguse();

        // �κ��丮�� ���� ������ �ִ��� Ȯ��
        bool hasFinalItem = InventorySystem.Instance.HasItem(finalItem);

        if (!GameState.Instance.HasFoundRocket) // ���� �߰�
        {
            GameState.Instance.HasFoundRocket = true;

            DayScriptSystem.Instance.SetDialogue(
                DayScriptSystem.Instance.TriggerFirstSpaceshipScene()
            );
        }
        else if (!hasFinalItem) // ������ ����
        {
            DayScriptSystem.Instance.SetDialogue(
                DayScriptSystem.Instance.TriggerSpaceshipDeniedEvent()
            );
        }
        else if (!GameState.Instance.HasStartedEnding) // ������ ���� + ���� ���� ���� ����
        {
            GameState.Instance.HasStartedEnding = true;

            DayScriptSystem.Instance.SetDialogue(
                DayScriptSystem.Instance.StartEndingSequence()
            );

            // ���⼭ ������ ���� ������ �߰� ����
            // InventorySystem.Instance.RemoveItem(finalItem, 1); 
        }
        else // ������ ���� ���
        {
            DayScriptSystem.Instance.SetDialogue(
                DayScriptSystem.Instance.EndingScript()
            );
        }
    }
    //private IEnumerator WaitForMouseClick()
    //{
    //    yield return null; // 1������ ��� (���� ������ Ŭ�� ����)

    //    // ���콺 ��ư�� ���� ������ ���
    //    while (!Input.GetMouseButtonDown(0))
    //        yield return null;
        
    //    Time.timeScale = 1f;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;

    //    popupActive = false;
    //}

    //private void ShowMessage(string message)
    //{
    //    if (UIManager.Instance != null)
    //    {
    //        UIManager.Instance.ShowPopup(message);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("UIManager�� ���� �����ϴ�.");
    //    }
    //}
}
