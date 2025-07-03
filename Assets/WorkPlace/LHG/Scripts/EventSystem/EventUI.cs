using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    //�г�, ��ư, ��ũ�� �� �� ui���� ���� ����ȭ

    [Header("mainUI �Ϸ�Ұ�, �Ϸᰡ��, �Ϸ��")]
    public Button CanNotCompleteBtns;
    public Button CanCompleteBtns;
    public Button Completed;

    [Header("mainUI �̺�Ʈ Ÿ��Ʋ, ����, ȿ��, �Ϸ�����")]
    public TMP_Text mainUIEventTitle;
    public TMP_Text mainUIEventDesc;
    public TMP_Text mainUIEventEffectName; //�ΰ��� �̻��ΰ��?
    public TMP_Text mainUIEventEffectAmount;
    public TMP_Text mainUIEEventRequireItemName; //�ΰ��� �̻��ΰ��?
    public TMP_Text mainUIEEventRequireItemAmount;

    [Header("subUI ��ũ�Ѻ��� content")]
    public GameObject[] EventListContent;

    [Header("subUI �̺�Ʈ ����Ʈ�� ����")]
    public TMP_Text[] subUIEventListTitle; 

    public void SetEventListTitleText()
    {
        //subUIEventListTitle[0].SetText(GameEventData.)
    }




}
