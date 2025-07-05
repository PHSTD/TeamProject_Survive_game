using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        // 0. ���̵���
        yield return StartCoroutine(FadeIn());

        // 1. ��¥ ǥ��
        int day = StatusSystem.Instance.GetCurrentDay();
        yield return StartCoroutine(TypeText(dayText, $"[{day}����]"));

        // 2. �̺�Ʈ ȿ�� ���
        string fullEventText = "";
        foreach (var evt in events)
        {
            fullEventText += $"- {evt.dialogue}\n{evt.eventEffectDesc}\n\n";

            foreach (GameEventData data in EventManager.Instance.CurEvents) 
            {
                EventManager.Instance.EventEffect(data);
            }
        }

        Debug.Log($"{StatusSystem.Instance.GetOxygen()}, {StatusSystem.Instance.GetEnergy()}, {StatusSystem.Instance.GetDurability()}");


        GameSystem.Instance.CheckGameOver();

        typingCoroutine = StartCoroutine(TypeText(eventText, fullEventText));

        // 4. ��ŵ ���
        isSkipping = false;
        while (typingCoroutine != null)
        {
            if (Input.GetMouseButtonDown(0)) isSkipping = true;
            yield return null;
        }

        // 5. Ȯ�� ��ư
        confirmButton.gameObject.SetActive(true);
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
        // ������ ó��
        StatusSystem.Instance.NextCurrentDay();
        StatusSystem.Instance.SetIsToDay(false);

        // ���ο� �̺�Ʈ ����
        EventManager.Instance.EventStart();

        // �� ��ε�
        SceneSystem.Instance.LoadSceneWithDelayAndSave(SceneSystem.Instance.GetShelterSceneName());
    }
}
