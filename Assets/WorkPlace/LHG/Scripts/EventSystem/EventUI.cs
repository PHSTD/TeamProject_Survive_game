using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    //�г�, ��ư, ��ũ�� �� �� ui���� ���� ����ȭ

    [Header("mainUI �Ϸ�Ұ�, �Ϸᰡ��, �Ϸ��")]
    public Button CanNotCompleteBtns;
    public Button CanCompleteBtns;
    public Button Completed;

    [Header("mainUI �̺�Ʈ Ÿ��Ʋ, ����, ȿ��, �Ϸ�����")]
    public TMP_Text mainUIEventTitle;
    public TMP_Text mainUIEventDesc;
    public TMP_Text mainUIEventEffectName; //�ΰ��� �̻��ΰ��?
    public TMP_Text mainUIEEventRequireItemName; //�ΰ��� �̻��ΰ��?

    [Header("subUI ��ũ�Ѻ��� content")]
    public GameObject[] EventListContent;

    [Header("subUI �̺�Ʈ ����Ʈ�� ����")]
    public TMP_Text[] subUIEventListTitle;

    private int eventIndex;

    public void SetEventListTitleText(GameEventData data, int _eventIndex)
    {
        CanCompleteBtns.onClick.RemoveAllListeners();
        mainUIEventTitle.text = data.title;
        mainUIEventDesc.text = data.description;
        EventClearDetermine(data);
        CanCompleteBtns.onClick.AddListener(() => EventClearOnUI(data));
        eventIndex = _eventIndex;
    }

    public void EventClearDetermine(GameEventData data)
    {
        CanCompleteBtns.gameObject.SetActive(EventManager.Instance.DetermineEventComplete(data));
        CanNotCompleteBtns.gameObject.SetActive(!EventManager.Instance.DetermineEventComplete(data));
    }


    public void EventClearOnUI(GameEventData data)
    {
        Completed.gameObject.SetActive(true);
        data.isComplete = true;
        EventManager.Instance.EventClear(data, eventIndex);
    }

}