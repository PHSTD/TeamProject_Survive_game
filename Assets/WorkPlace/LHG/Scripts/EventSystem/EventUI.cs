using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    //�г�, ��ư, ��ũ�� �� �� ui���� ���� ����ȭ

    public static EventUI Instance { get; private set; }

    [Header("mainUI �Ϸ�Ұ�, �Ϸᰡ��, �Ϸ��")]
    public Button CanNotCompleteBtns;
    public Button CanCompleteBtns;
    public Button Completed;

    [Header("mainUI �̺�Ʈ Ÿ��Ʋ, ����, ȿ��, �Ϸ�����")]
    public TMP_Text mainUIEventTitle;
    public TMP_Text mainUIEventDesc;
    public TMP_Text mainUIEventEffectName; //�ΰ��� �̻��ΰ��?
    public TMP_Text mainUIEEventRequireItemName; //�ΰ��� �̻��ΰ��?
    public TMP_Text uncompletedEventListText; //bedroom subui uncompleted events list display

    [Header("subUI ��ũ�Ѻ��� content")]
    public GameObject[] EventListContent;

    private int eventIndex;

    private void OnEnable()
    {
        EventUI.Instance?.UpdateUncompletedEventList();
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public void SetEventListTitleText(GameEventData data, int _eventIndex)
    {
        CanCompleteBtns.onClick.RemoveAllListeners();

        mainUIEventTitle.text = data.title;
        mainUIEventDesc.text = data.description;
        mainUIEventEffectName.text = data.eventEffectDesc;

        //if (data.id == 10001)
        //{
        //    mainUIEventEffectName.text += $"\n실제 적용될 패널티: 내구도 -{(int)data.RandomMinusDuraValue}";
        //}

        // 필요 아이템 표시 (보유 수 / 필요 수) 0706추가
        string requireText = "";

        if (data.requiredItemA != null)
        {
            int haveA = Storage.Instance.GetItemCount(data.requiredItemA);
            requireText += $"{data.requiredItemA.itemName}: {haveA}/{data.requiredAmountA}";
        }

        if (data.requiredItemB != null)
        {
            int haveB = Storage.Instance.GetItemCount(data.requiredItemB);
            requireText += $"\n{data.requiredItemB.itemName}: {haveB}/{data.requiredAmountB}";
        }

        mainUIEEventRequireItemName.text = requireText;

        EventClearDetermine(data);
        CanCompleteBtns.onClick.AddListener(() => EventClearOnUI(data));
        eventIndex = _eventIndex;
        Completed.gameObject.SetActive(false);

        
    }



    public void SetEventSubUIBtnTitle(GameObject go, int eventIndex) //����uiŸ��Ʋ����Ʈ�����
    {
        CanCompleteBtns.onClick.RemoveAllListeners();
        TMP_Text text = go.GetComponentInChildren<TMP_Text>();
        text.SetText(EventManager.Instance.CurEvents[eventIndex].title);
        //go.GetComponent<Button>().interactable = false; �̺�Ʈ�� ������Ʈ���� �Ϸ�Ȱ��
            //�÷�ƾƮ�� ���̶���Ʈ�� �𽺿��̺��� ������ �ٸ��� �������
    }
    public void EventClearDetermine(GameEventData data)
    {
        CanCompleteBtns.gameObject.SetActive(EventManager.Instance.DetermineEventComplete(data));
        CanNotCompleteBtns.gameObject.SetActive(!EventManager.Instance.DetermineEventComplete(data));
    }


    public void EventClearOnUI(GameEventData data)
    {
        CanCompleteBtns.gameObject.SetActive(false);//0704�Ϸᰡ�ɹ�ư���� ��Ȱ��ȭ��ġ
        Completed.gameObject.SetActive(true);
        data.isComplete = true;
        EventManager.Instance.EventClear(data, eventIndex);
    }

    public void UpdateUncompletedEventList()
    {
        List<GameEventData> uncompleted = EventManager.Instance.GetUnCompletedEvents();

        if (uncompleted.Count == 0)
        {
            uncompletedEventListText.text = "오늘 완료하지 못한 이벤트가 없습니다.";
            Debug.Log("BedRoom 오늘 완료하지 못한 이벤트 없음");
            return;
        }

        string text = "완료하지 못한\n 이벤트들이 있다..:\n";
        foreach (var evt in uncompleted)
        {
            text += $"- {evt.title}\n";
        }

        uncompletedEventListText.text = text;
        Debug.Log("BedRoom 오늘 완료하지 못한 이벤트가 있음");

    }

}