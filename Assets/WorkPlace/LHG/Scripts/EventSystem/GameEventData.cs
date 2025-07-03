using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//��� ���� �̺�Ʈ���� ������


[CreateAssetMenu(fileName = "GameEventData", menuName = "Event System/Game Event")]
public class GameEventData : ScriptableObject
{
    public int id;
    public string title;
    [TextArea] public string description;
    private int minDura = 10;
    private int maxDura = 20;

    public void GenerateRandomDuraValue()
    {
        RandomMinusDuraValue = Random.Range(minDura, maxDura + 1);
    }

    public bool isComplete;

    [Header("�̺�Ʈ ȿ��(�㸶�� �ߵ��Ǵ� ������)")]
    public double RandomMinusDuraValue;
    public double PlusDurability;
    public double MinusDurability;
    public double MinusOxygen;
    public double MinusEnergy;
    public float PlusEnergyEfficiency;
    public float MinusEnergyEfficiency;
    public float PlusOxygenEfficiency;
    public float MinusOxygenEfficiency;

    [Header("���� ����(�ʿ� ������ ��)")]
    public Item requiredItemA;
    public int requiredAmountA;
    public Item requiredItemB;
    public int requiredAmountB;

    [Header("ù�� + ���� �⺻������ ������Ǵ� �̺�Ʈ")]
    public bool reactiveAfterEnd;

    [Header("�̿Ϸ���·� ��¥��ȯ�� ��� ���")]
    [TextArea] public string dialogue;

    
    public enum EventState
    {
        //���� ������ ���߿� �ٽ� ����
        CanNotComplete,
        CanComplete,
        AlreadyComplted
    }

    public enum EventIsActivated //��ư? �̺�Ʈ?
    {
        NotExist,
        Activated,
        Disabled
    }

    public EventIsActivated isActivated;//�̺�ƮȰ��ȭ����?
}