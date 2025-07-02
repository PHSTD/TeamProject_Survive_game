using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//��� ���� �̺�Ʈ���� ������


[CreateAssetMenu(fileName = "GameEventData", menuName = "Event System/Game Event")]
public class GameEventData : ScriptableObject
{
    


    public static GameEventData Instance { get; private set; }

    public int id;
    public string title;
    [TextArea] public string description;


    [Header("�̺�Ʈ ȿ��(�㸶�� �ߵ��Ǵ� ������)")]
    public double minusCurDurability;
    public int curOxygen;
    public int curEnergy;
    public int BatteryEfficiency; 
    public int OxygenEfficiency;

    [Header("���� ����(�ʿ� ������ ��)")]
    public string requiredItem; //�䱸 �������� �ΰ��� �̻��ε�..�������
    public int requiredAmount; 

    [Header("ù�� + ���� �⺻������ ������Ǵ� �̺�Ʈ")]
    public bool reactiveAfterEnd;

    [Header("�̿Ϸ���·� ��¥��ȯ�� ��� ���")]
    [TextArea] public string dialogue;
}
