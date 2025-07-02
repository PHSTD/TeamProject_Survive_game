using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //��ü �̺�Ʈ �ý����� ����, �̺�Ʈ ������ �ε�, �̺�Ʈ Ȱ��ȭ, ���� ���� ������Ʈ, �̺�Ʈ�Ϸ� ��
    
    public static EventManager Instance { get; private set; }
    private Dictionary<int, GameEventData> eventDict = new();
    public List<GameEventData> allEvents = new();

    private int _curGameDay = StatusSystem.Instance.GetCurrentDay();

    //�κ��丮 �� â�� ����

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllEvents();
    }

    // �̺�Ʈ ������ ����� ��������
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

    

    public void TriggerEvent()
    {
        if (GameEventData.Instance.reactiveAfterEnd==true)
        {

        }
        //1������



        //1~3������
        


        //4����~ 
    }

   

    


    // �̺�Ʈ �߻�



    // �̺�Ʈ ��������Ǻ�

    // �̺�Ʈ �Ϸ�ó��


}
