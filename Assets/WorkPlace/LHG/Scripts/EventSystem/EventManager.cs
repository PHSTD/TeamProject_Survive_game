using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //��ü �̺�Ʈ �ý����� ����, �̺�Ʈ ������ �ε�, �̺�Ʈ Ȱ��ȭ, ���� ���� ������Ʈ, �̺�Ʈ�Ϸ� ��

    // �̺�Ʈ ������ ����� ��������
    public static EventManager Instance { get; private set; }
    private Dictionary<int, GameEventData> eventDict = new();
    public List<GameEventData> allEvents = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllEvents();
    }

    private void LoadAllEvents()
    {
        var events = Resources.LoadAll<GameEventData>("Events");
        foreach(var e in events)
        {
            if(!eventDict.ContainsKey(e.id))
            {
                eventDict[e.id] = e;
                allEvents.Add(e);
            }
            else
            {
                Debug.LogWarning($"�ߺ��� �̺�Ʈ ID : {e.id}");
            }

        }
        Debug.Log($"�̺�Ʈ {eventDict.Count}�� �ε� �Ϸ�");
    }

    public GameEventData GetEventById(int id)
    {
        eventDict.TryGetValue(id, out var result);
        return result;
    }


    public List<GameEventData> GetEventsByPhase(TriggerPhase phase)
    {
        return allEvents.FindAll(e => e.triggerphase == phase);
    }

    private void TriggerRandomEvent(int gameDay)
    {
        TriggerPhase phase = GetPhaseFromDay(gameDay);
        var candidates = EventManager.Instance.GetEventsByPhase(phase);

        if (candidates.Count == 0) return;

        int index = Random.Range(0, candidates.Count);
        GameEventData todayEvent = candidates[index];

        Debug.Log($"������ �̺�Ʈ :{todayEvent.title} - {todayEvent.description}");
    }

    TriggerPhase GetPhaseFromDay(int day)
    {
        if (day == 1) return TriggerPhase.Start;
        if (day <= 3) return TriggerPhase.Early;
        return TriggerPhase.Mid;
    }

    //�κ��丮 �� â�� ����


    // �̺�Ʈ �߻�



    // �̺�Ʈ ��������Ǻ�

    // �̺�Ʈ �Ϸ�ó��


}
