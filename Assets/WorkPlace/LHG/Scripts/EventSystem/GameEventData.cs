using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//��� ����Ʈ �����͸� ����


[CreateAssetMenu(fileName = "GameEventData", menuName = "Event System/Game Event")]
public class GameEventData :ScriptableObject
{
    public static GameEventData Instance { get; private set; }

    public int id;
    public string title;
    [TextArea] public string description;
    

    [Header("�̺�Ʈ ȿ��(�㸶�� �ߵ��Ǵ� ������)")]
    [TextArea] public string effectDescription;

    [Header("���� ����(�ʿ� ������ ��)")]
    public string requiredItem;
    public int requiredAmount;

    [Header("���� �� ó��(���� �Ǵ� ȸ��)")]
    [TextArea] public string endEffectDescription;
    public bool reactiveAfterEnd;

    [Header("�̿Ϸ� ��¥��ȯ�� ��� ���")]
    [TextArea] public string dialogue;
}
