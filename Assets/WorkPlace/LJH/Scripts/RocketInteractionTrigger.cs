using UnityEngine;

public class RocketInteractionTrigger : Structure
{
    public Rocket rocketDialogue;
    public Item requiredFinalItem; // 인스펙터에서 세팅할 최종 제작 아이템


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

        // 대화 시작 시 다이얼로그 UI 활성화
        if (Storage.Instance.HasItem(requiredFinalItem) && PlayerManager.Instance.EventCount == 1)
        {
            // 엔딩 아이템이 있을 때 엔딩으로
            DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.StartEndingSequence());
            PlayerManager.Instance.EventCount++;
        }
        else if (PlayerManager.Instance.EventCount == 2)
        {
            // 엔딩 스크립트 출력
            DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.EndingScript());
        }

        // 로켓 UI도 보여주려면 아래도 활성화
        // rocketDialogue.gameObject.SetActive(true);
    }
}
    

