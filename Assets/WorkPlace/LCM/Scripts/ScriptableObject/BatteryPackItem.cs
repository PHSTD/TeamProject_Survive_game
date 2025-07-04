using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/Battery Pack")]
public class BatteryPackItem : ConsumableItem
{
    [Header("Battery Pack Specifics")]
    [Tooltip("�� ������ ��� �� ȸ���� ���ⷮ�Դϴ�.")]
    public double ElectricRestoreAmount;

    public override void Use(GameObject user)
    {
        base.Use(user); // ������

        Debug.Log($"{itemName}��(��) ����Ͽ� ���⸦ {ElectricRestoreAmount}��ŭ ȸ���߽��ϴ�.");

        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ElecticGauge.Value += ElectricRestoreAmount;
        }
        else
        {
            Debug.LogWarning("PlayerManager.Instance�� ã�� �� �����ϴ�. ���� ȸ�� ����.");
        }
    }
}
