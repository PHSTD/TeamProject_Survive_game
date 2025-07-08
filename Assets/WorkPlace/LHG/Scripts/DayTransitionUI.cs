using Assets.WorkPlace.LHG.Scripts.EventSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DayTransitionUI : MonoBehaviour
{
    public CanvasGroup fadeGroup;
    public TMP_Text dayText;
    public TMP_Text eventText;
    public Button confirmButton;

    private bool isSkipping = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmClicked);
        confirmButton.gameObject.SetActive(false);
        fadeGroup.alpha = 0f;
    }

    public void StartDayTransition(List<GameEventData> uncompletedEvents)
    {
        gameObject.SetActive(true);
        StartCoroutine(TransitionSequence(uncompletedEvents));
        
    }

    private IEnumerator TransitionSequence(List<GameEventData> events)
    {
        Debug.Log("[DayTransitionUI] TransitionSequence 시작");
        // 페이드 인
        Debug.Log("[DayTransitionUI] 페이드 인 시작");
        yield return StartCoroutine(FadeIn());

         Debug.Log("[DayTransitionUI] 페이드 인 완료");

    // 날짜 표시
        int day = StatusSystem.Instance.GetCurrentDay();
        yield return StartCoroutine(TypeText(dayText, $"[{day}일차]"));

        // 이벤트 효과 출력
        string fullEventText = "";
        foreach (var evt in events)
        {
            fullEventText += $"- {evt.dialogue}\n{evt.eventEffectDesc}\n\n";
        }

        foreach (EventController ctr in EventManager.Instance.GetUnCompletedEvents())
        {
            Debug.Log($"[DayTransitionUI] 이벤트 효과 적용: {ctr.data.dialogue}");
            EventManager.Instance.EventEffect(ctr.data);
        }

    Debug.Log($"[DayTransitionUI] 현재 상태 - 산소: {StatusSystem.Instance.GetOxygen()}, 에너지: {StatusSystem.Instance.GetEnergy()}, 내구도: {StatusSystem.Instance.GetDurability()}");

    typingCoroutine = StartCoroutine(TypeText(eventText, fullEventText));
    Debug.Log("[DayTransitionUI] 이벤트 텍스트 출력 완료");

    // 스킵 대기
    Debug.Log("[DayTransitionUI] 스킵 대기 시작");
    isSkipping = false;
    while (typingCoroutine != null)
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("[DayTransitionUI] 스킵 버튼 클릭 감지");
            isSkipping = true;
        }
        yield return null;
    }
    Debug.Log("[DayTransitionUI] 스킵 대기 완료");

    // 확인 버튼 활성화
    Debug.Log("[DayTransitionUI] 확인 버튼 활성화");
    confirmButton.gameObject.SetActive(true);

    EventManager.Instance.EventGeneration(StatusSystem.Instance.GetCurrentDay());
    Debug.Log("[DayTransitionUI] 이벤트 생성 완료");

    EventManager.Instance.SaveEventData();
    Debug.Log("[DayTransitionUI] 이벤트 데이터 저장 완료");

    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime / 1f;
            fadeGroup.alpha = time;
            yield return null;
        }
    }

    private IEnumerator TypeText(TMP_Text target, string fullText)
    {
        target.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            if (isSkipping)
            {
                target.text = fullText;
                break;
            }

            target.text += fullText[i];
            yield return new WaitForSeconds(0.03f);
        }

        typingCoroutine = null;
    }

    private void OnConfirmClicked()
    {
        // 다음날
        StatusSystem.Instance.NextCurrentDay();
        // 탐색 여부 초기화
        StatusSystem.Instance.SetIsToDay(false);
        // 쉘터 씬으로 이동
        SceneSystem.Instance.LoadSceneWithDelayAndSave(SceneSystem.Instance.GetShelterSceneName());
        
        GameSystem.Instance.CheckGameOver();
        // SceneSystem.Instance.LoadSceneWithCallback(SceneSystem.Instance.GetShelterSceneName(), () =>
        // {
        // });
    }
}
