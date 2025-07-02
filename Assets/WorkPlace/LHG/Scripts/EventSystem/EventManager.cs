using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //��ü �̺�Ʈ �ý����� ����, �̺�Ʈ ������ �ε�, �̺�Ʈ Ȱ��ȭ, ���� ���� ������Ʈ, �̺�Ʈ�Ϸ� ��
    
    public static EventManager Instance { get; private set; }
    private Dictionary<int, GameEventData> eventDict = new();
    public List<GameEventData> allEvents = new();

    // 250702 ����� �������� ����
    // private int _curGameDay = StatusSystem.Instance.GetCurrentDay();
    private int _curGameDay;

    //�κ��丮 �� â�� ����

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize _curGameDay safely in Awake
        // 250702 ����� �������� �߰�
        InitializeCurrentDay();
        LoadAllEvents();
    }
    
    // 250702 ����� �������� �߰�
    private void InitializeCurrentDay()
    {
        // Check if StatusSystem.Instance is available
        if (StatusSystem.Instance != null)
        {
            _curGameDay = StatusSystem.Instance.GetCurrentDay();
        }
        else
        {
            Debug.LogWarning("StatusSystem.Instance is not available yet. Using default value for current day.");
            _curGameDay = 1; // or whatever default value makes sense
        }
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
