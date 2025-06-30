using DesignPattern;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private GameObject _playerPrefab;

    public bool IsInIntercation = false;
    public WorldItem InteractableItem { get; set; }
    public TestWorldItem InteractableTestItem { get; set; }
    public Structure InteractableStructure { get; set; }
    public Item SelectItem { get; set; }
    public ObseravableProperty<float> AirGauge = new();
    public ObseravableProperty<float> ElecticGauge = new();
    public PlayerController Player { get; private set; }
    public float InteractDelay { get; set; }
    public float ItemDelay { get; set; }
    /// <summary>
    /// �÷��̾� ��ȭ. �迭 ũ��� 3. 0 = ��Ʈ��, 1,2�� ���� �߰� ����
    /// </summary>
    public bool[] PlayerUpgrade { get; set; } = new bool[3];


    private void Awake()
    {
        SingletonInit();
        Init();
    }

    private void Start()
    {
        PlayerInit();
    }

    private void Update()
    {
        if (SceneSystem.Instance.GetCurrentSceneName() == SceneSystem.Instance.GetFarmingSceneName() && Player == null)
        {
            PlayerInit();
        }

    }

    private void Init()
    {
        AirGauge.Value = 100f;
        ElecticGauge.Value = 100f;
    }

    private void PlayerInit()
    {
        // �÷��̾� �ӽ� ���� �ڵ�
        GameObject player = Instantiate(_playerPrefab, new Vector3(237.29f, 10.225f, -110.03f), Quaternion.identity);
        Player = player.GetComponent<PlayerController>();
        Debug.Log("�÷��̾� ���� �Ϸ�");
    }
}
