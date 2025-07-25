using DesignPattern;
using UnityEngine;

public class SamplePlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private GameObject _playerPrefab;

    public bool IsInIntercation = false;
    [field: SerializeField] public WorldItem InteractableItem { get; set; }
    [field: SerializeField] public TestWorldItem InteractableTestItem { get; set; }
    [field: SerializeField] public Structure InteractableStructure { get; set; }
    public Item SelectItem { get; set; }
    public ObseravableProperty<float> AirGauge = new();
    public ObseravableProperty<float> ElecticGauge = new();
    public PlayerController Player { get; private set; }
    [field: SerializeField] public float InteractDelay { get; set; }
    public float ItemDelay { get; set; }

    private void Awake()
    {
        SingletonInit();
        Init();
    }

    private void Start()
    {
        //PlayerInit();
    }

    private void Update()
    {
        //if (SceneSystem.Instance.GetCurrentSceneName() == SceneSystem.Instance.GetFarmingSceneName() && Player == null)
        //{
        //    PlayerInit();
        //}

    }

    private void Init()
    {
        AirGauge.Value = 100f;
        ElecticGauge.Value = 100f;
    }

    //private void PlayerInit()
    //{
    //    // 플레이어 임시 생성 코드
    //    GameObject player = Instantiate(_playerPrefab, new Vector3(237.29f, 10.225f, -110.03f), Quaternion.identity);
    //    Player = player.GetComponent<PlayerController>();
    //    Debug.Log("플레이어 생성 완료");
    //}
}
