using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��ü �̺�Ʈ �ý����� ����, �̺�Ʈ ������ �ε�, �̺�Ʈ Ȱ��ȭ, ���� ���� ������Ʈ, �̺�Ʈ�Ϸ� ��
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    private Dictionary<int, GameEventData> eventDict = new();
    public List<GameEventData> allEvents = new();



    private GameEventData _curEventData;

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

    //���� �ߵ��� �̺�Ʈ���� ��� �־�� �ҵ� ? onWorkingEvents => �ʿ������ 


    public void GenerateDailyEvent() //���ӽ��� + ��ħ������ ȣ��Ǿ�� ��
    {
        _curEventData = eventDict[10001];
        Debug.Log("������ ���� �̺�Ʈ �߻�(����)");
    }

    public void GenerateRandomEvent() // ******��ħ������ ȣ��Ǿ���� ��������ýý��ۻ��캸��, 1~3������ ȣ��1��, 4~6�������� ȣ��2��, 7����~ȣ��2��
    {
        int eventID = 0;
        if (3 >= StatusSystem.Instance.GetCurrentDay())
        {
            eventID = Random.Range(10002, 10005); //�̻�̸�
            Debug.Log("1~3���� �̺�Ʈ �߻�");
        }
        else if (6 >= StatusSystem.Instance.GetCurrentDay())
        {
            eventID = Random.Range(10002, 10007);
            Debug.Log("4~6���� �̺�Ʈ �߻�");
        }
        else
        {
            eventID = Random.Range(10005, 10010);
            Debug.Log("7����~ �̺�Ʈ �߻�");
        }

        _curEventData = eventDict[eventID];
        //1~3������ id 10002~10004 �� 1��

        //4~6������ id10002~10006 �� 2��

        //7����~ id10005~10009 �� 2��
    }

    // �̺�Ʈ�� �Ϸᰡ������ �Ǻ�
    public void DetermineEventComplete() //*��ư�� ȣ�� �޾������
    {

        //**�����ۺ������θ� �ľ��Ҽ��ִ� �̸��̵� id���̵� Ȯ��
        //��ȸ�� �������� �ľ� true false 
        // if required item count < �κ��丮�� cur_item_count�̸� item count --���ְ�
        // �ش� id�� �̺�Ʈ ��ü�� ����

    }

    public void EventEffect(GameEventData data) //******��¥�� �Ѿ�� ����Ǿ���� ���Լ��� ��𼱰� ȣ��Ǿ�� �� => ��¥�Ѿ�½ý����� Ȯ���ؼ� ����ֱ�
    {

        if (isEventCompleted() == false)
        {
            //������ ������ �����̺�Ʈ  �ش��� �Ű������� �޾Ƽ� ���븸
            //������ ��� ������ 
            //��¥����� �����ǥ�� �޼����� ���ϸ� �����
            StatusSystem.Instance.SetMinusDurability(data.RandomMinusDuraValue);
            StatusSystem.Instance.SetMinusDurability(data.MinusDurability);
            StatusSystem.Instance.SetMinusEnergy(data.MinusEnergy);
            StatusSystem.Instance.SetMinusOxygen(data.MinusOxygen);
            StatusSystem.Instance.SetMinusOxygenGainMultiplier(data.MinusOxygenEfficiency);
            StatusSystem.Instance.SetMinusEnergyGainMultiplier(data.MinusEnergyEfficiency);
        }

        if (isEventCompleted() == true)
        {
            StatusSystem.Instance.SetPlusOxygen(data.PlusDurability);
            StatusSystem.Instance.SetPlusOxygenGainMultiplier(data.PlusOxygenEfficiency);
            StatusSystem.Instance.SetPlusEnergyGainMultiplier(data.PlusEnergyEfficiency);
        }
    }

    public bool isEventCompleted()
    {
        return false;
    }
}
