using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//��� ����Ʈ �����͸� ����

public enum TriggerPhase {Start, Early, Mid}
[CreateAssetMenu(fileName = "GameEventData", menuName = "Event System/Game Event")]
public class GameEventData :ScriptableObject
{
    public int id;
    public string title;
    [TextArea] public string description;
    public TriggerPhase triggerphase;

    [Header("�̺�Ʈ ȿ��(�㸶�� �ߵ��Ǵ� ������")]
    [TextArea] public string effectDescription;

    [Header("���� ����(�ʿ� ������ ��")]
    public string requiredItem;
    public int requiredAmount;

    [Header("���� �� ó��(���� �Ǵ� ȸ��")]
    [TextArea] public string endEffectDescription;
    public bool reactiveAfterEnd;

    [Header("��¥ ��ȯ �� ���")]
    [TextArea] public string dialogue;
}
