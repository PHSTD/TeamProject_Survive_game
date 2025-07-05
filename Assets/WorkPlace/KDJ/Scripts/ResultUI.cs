using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [Header("UI �⺻���")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TMP_Text _remainO2;
    [SerializeField] private TMP_Text _O2TankMulti;
    [SerializeField] private TMP_Text _allO2Tank;
    [SerializeField] private TMP_Text _allGainO2;
    [SerializeField] private TMP_Text _shelterRemainO2;
    [SerializeField] private TMP_Text _shelterResultO2;
    [SerializeField] private TMP_Text _batteryMulti;
    [SerializeField] private TMP_Text _allBattery;
    [SerializeField] private TMP_Text _allGainElec;
    [SerializeField] private TMP_Text _shelterRemainElec;
    [SerializeField] private TMP_Text _shelterResultElec;
    [SerializeField] private TMP_Text _itemGainList;
    [SerializeField] private TMP_Text _itemGainCount;

    [Header("������ ����Ʈ ��ȸ�� ���� ����Ʈ")]
    [SerializeField] private List<Item> _itemList;

    private void Awake()
    {
        if (_canvas.enabled == true)
        {
            _canvas.enabled = false;
        }
    }

    public void OnResultUI()
    {
        if (_canvas != null)
        {
            _canvas.enabled = true;
            UpdateResultUI();
            Cursor.lockState = CursorLockMode.None; // Ŀ�� ��� ����
        }
        else
        {
            Debug.LogError("ResultUI: Canvas is not assigned.");
        }
    }

    public void OffResultUI()
    {
        if (_canvas != null)
        {
            //_canvas.enabled = false;
            ResetGainItemList();
            // Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ���
            // �׽�Ʈ ������ �Ʒ� �ڵ�� ��ü
            SceneSystem.Instance.LoadShelterScene();
        }
        else
        {
            Debug.LogError("ResultUI: Canvas is not assigned.");
        }
    }

    public void UpdateResultUI()
    {
        SetResultUI();
        SetGainItemList();
    }

    private void SetResultUI()
    {
        // ������ ����Ʈ �ε��� �����ũ 7, ���͸� 8
        int oxygenTankCount = Inventory.Instance.GetItemCount(_itemList[7]);
        int batteryCount = Inventory.Instance.GetItemCount(_itemList[8]);

        // ��� ���
        double remainO2 = PlayerManager.Instance.AirGauge.Value;
        double shelterO2 = StatusSystem.Instance.GetOxygen();
        
        _remainO2.text = remainO2.ToString("F1");
        _O2TankMulti.text = oxygenTankCount.ToString();
        _allO2Tank.text = (oxygenTankCount * 10).ToString("F1"); // �����ũ �ϳ��� 10�� ��Ҹ� ����
        _allGainO2.text = (remainO2 + oxygenTankCount * 10).ToString("F1");
        _shelterRemainO2.text = shelterO2.ToString("F1");
        _shelterResultO2.text = (shelterO2 + (remainO2 + oxygenTankCount * 10)).ToString("F1");

        // ���� ���
        double shelterElec = StatusSystem.Instance.GetEnergy();

        _batteryMulti.text = batteryCount.ToString();
        _allBattery.text = (batteryCount * 10).ToString("F1"); // ���͸� �ϳ��� 10�� ������ ����
        _allGainElec.text = (batteryCount * 10).ToString("F1");
        _shelterRemainElec.text = shelterElec.ToString("F1");
        _shelterResultElec.text = (shelterElec + (batteryCount * 10)).ToString("F1");

        // ��Ҹ� ����
        StatusSystem.Instance.SetPlusOxygen(remainO2);
    }

    private void SetGainItemList()
    {
        for(int i = 0; i < 7; i++)
        {
            int itemCount = Inventory.Instance.GetItemCount(_itemList[i]);

            if (itemCount > 0)
            {
                _itemGainList.text += _itemList[i].itemName + "\n";
                _itemGainCount.text += itemCount + " ��\n";
            }
        }
    }

    private void ResetGainItemList()
    {
        _itemGainList.text = string.Empty;
        _itemGainCount.text = string.Empty;
    }
}
