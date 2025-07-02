using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��ü �̺�Ʈ �ý����� ����, �̺�Ʈ ������ �ε�, �̺�Ʈ Ȱ��ȭ, ���� ���� ������Ʈ, �̺�Ʈ�Ϸ� ��
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    private Dictionary<int, GameEventData> eventDict = new();
    public List<GameEventData> allEvents = new();
    public List<GameEventData> onWorkingEvents = new();


    private int _curGameDay = StatusSystem.Instance.GetCurrentDay();

    //�κ��丮 �� â�� ����

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllEvents();
    }

    // ��� �̺�Ʈ ������ ����� ��������
    private void LoadAllEvents()
    {
        var events = Resources.LoadAll<GameEventData>("Events");
        foreach (var e in events)
        {
            if (!eventDict.ContainsKey(e.id))
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

    //���� �ߵ��� �̺�Ʈ���� ����Ʈ �־�� �ҵ� ? onWorkingEvents
    



    // �̺�Ʈ�� �ߵ� �Ǵ� �����Ǵ� ����
    public void TriggerEvent()
    {
        // ���� ��ħ ���� �ߵ����� �̺�Ʈ�� ��ü�� ���캸��
        // ������ ��ĥ°���� �Ǻ��ϰ�
        // ���ڿ� �´� �̺�Ʈid������ SO���� �����ϰ� �ߵ�

        // ��, �ߺ��Ǵ� id�� �ߵ���Ű�� ����

        // ��, ���� ���ۺ��� ���� ������Ǵ� �̺�Ʈ id 10001
        if (GameEventData.Instance.reactiveAfterEnd == true)
        {

        }

        //1~3������ id 10002~10004 �� 1��

        //4~6������ id10002~10006 �� 2��

        //7����~ id10005~10009 �� 2��

    }




    // �̺�Ʈ�� �Ϸᰡ������ �Ǻ�
    public void DetermineEventComplete()
    {

        // if required item count < �κ��丮�� cur_item_count�̸� item count --���ְ�
        // �ش� id�� �̺�Ʈ ��ü�� ����

    }


    // 












}
