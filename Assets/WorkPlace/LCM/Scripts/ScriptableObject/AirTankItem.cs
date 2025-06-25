using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/Air Tank")]
public class AirTankItem : ConsumableItem
{
    [Header("Air Tank Specifics")]
    [Tooltip("�� ������ ��� �� ȸ���� ��ҷ��Դϴ�.")]
    public float OxygenRestoreAmount;

    public override void Use(GameObject user)
    {
        base.Use(user); //����� ��

        Debug.Log($"{OxygenRestoreAmount}��Ҹ� ȸ���߽��ϴ�.");
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.AirGauge.Value += OxygenRestoreAmount;
        }
        else
        {
            Debug.LogWarning("PlayerManager.Instance�� ã�� �� �����ϴ�. ��� ȸ�� ����.");
        }
    }
}
