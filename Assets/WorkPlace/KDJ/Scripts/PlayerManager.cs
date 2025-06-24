using DesignPattern;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public bool IsInIntercation = false;
    public WorldItem InteractableItem { get; set; }
    public ObseravableProperty<float> AirGauge = new();
    public ObseravableProperty<float> ElecticGauge = new();
    //�׽�Ʈ�� �κ��丮

    public bool IsInAirChamber { get; set; } = false;

    private void Awake()
    {
        SingletonInit();
        Init();
    }

    private void Update()
    {
        TestCode();
    }

    private void Init()
    {
        AirGauge.Value = 100f;
        ElecticGauge.Value = 100f;
    }

    /// <summary>
    /// �׽�Ʈ�� �ڵ�
    /// </summary>
    private void TestCode()
    {
        if (!IsInAirChamber)
            AirGauge.Value -= Time.deltaTime * 1f;
    }
    
}
