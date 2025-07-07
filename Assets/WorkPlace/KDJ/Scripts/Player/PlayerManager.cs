using DesignPattern;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private float _baseSpeed = 5f;

    [field: SerializeField] public GameObject SunGlasses;

    private Akimbo _akimboCheck;

    public float Speed;
    public int EventCount { get; set; } = 0; // 이벤트 카운트

    public bool CanUseJetpack { get; set; } = false; // 제트팩 사용 가능 여부
    public bool IsInIntercation = false;
    public bool IsAkimbo { get; set; } = false;
    public WorldItem InteractableItem { get; set; }
    public TestWorldItem InteractableTestItem { get; set; }
    public Structure InteractableStructure { get; set; }
    public GameObject InHandItem { get; set; }
    public GameObject InHandItem2 { get; set; }
    public GameObject InHeadItem { get; set; } // 헬멧, 선글라스 등 머리에 착용하는 아이템
    public Item SelectItem { get; set; }
    public ObseravableProperty<double> AirGauge = new();
    public ObseravableProperty<double> ElecticGauge = new();
    public PlayerController Player { get; private set; }
    public RaycastHit HitInfo { get; set; }
    public float InteractDelay { get; set; }
    public float ItemDelay { get; set; }


    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Start()
    {
        PlayerInit();
        Debug.Log("플레이어 입장!!");
    }

    private void Update()
    {
        if (SceneSystem.Instance?.GetCurrentSceneName() == SceneSystem.Instance?.GetFarmingSceneName() && Player == null)
        {
            PlayerInit();
        }

        AirConsume();
        AkimboCheck(); // 아킴보 상태 확인

        Debug.Log("InteractableItem: " + InteractableItem);
        Debug.Log("InteractableStructure: " + InteractableStructure);
    }

    private void Init()
    {
        Speed = _baseSpeed;
    }

    private void PlayerInit()
    {
        AirGaugeInit();
        GameObject player = Instantiate(_playerPrefab, new Vector3(255.087f, 10.225f, -123.6639f), Quaternion.identity);

        Player = player.GetComponent<PlayerController>();

    }

    private void AkimboCheck()
    {
        if (_akimboCheck != null) return;


        if (SelectItem != null)
        {
            IsAkimbo = SelectItem.HandleItem.TryGetComponent<Akimbo>(out _akimboCheck);
        }
    }

    private void AirConsume()
    {
        if (SceneSystem.Instance?.GetCurrentSceneName() != SceneSystem.Instance?.GetFarmingSceneName() || Player == null)
            return;

        if (AirGauge.Value <= 0)
        {
            AirGauge.Value = 0;
            GameSystem.Instance.CheckGameOver();
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        AirGauge.Value -= 0.2f * Time.deltaTime;
    }

    public void AkimboReset()
    {
        // 아킴보 상태 초기화
        IsAkimbo = false;
        _akimboCheck = null;
    }

    private void AirGaugeInit()
    {
        double shelterAir = StatusSystem.Instance.GetOxygen();

        Debug.Log($"쉘터 산소량: {shelterAir}");

        if (shelterAir > 100)
        {
            AirGauge.Value = 100;
            StatusSystem.Instance.SetMinusOxygen(100);
            Debug.Log("플레이어 산소 초기화: 100");
            return;
        }

        AirGauge.Value = shelterAir;
        StatusSystem.Instance.SetMinusOxygen(shelterAir);
        Debug.Log($"플레이어 산소 초기화: {shelterAir}");
    }
}
