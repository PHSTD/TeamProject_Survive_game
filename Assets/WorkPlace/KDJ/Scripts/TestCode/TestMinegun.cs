using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Test;

public class TestMinegun : TestToolItem
{
    private bool _isMining;

    private TestDrillMining _drillMining;

    public override void Use(GameObject user)
    {
        Debug.Log($"��� �ִ� ������ : {PlayerManager.Instance.Player._testHandItem.name}");
        if (_drillMining == null) _drillMining = PlayerManager.Instance.Player._testHandItem.GetComponent<TestDrillMining>();
        _drillMining.Action((int)miningPower);
    }
}
