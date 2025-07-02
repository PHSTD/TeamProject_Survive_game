using DesignPattern;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private float _baseSpeed = 5f;

    [field: SerializeField] public GameObject SunGlasses;

    private Akimbo _akimboCheck;
    
    public float Speed;
    /// <summary>
    /// �÷��̾� ��ȭ ���¸� ǥ���ϴ� Bool �迭. ũ��� 3. 0 = ��Ʈ��, 1,2�� ���� �߰� ����
    /// </summary>
    public bool[] IsUpgraded { get; set; } = new bool[3];
    public bool IsInIntercation = false;
    public bool IsAkimbo { get; set; } = false; // ��Ŵ�� ���� ����
    public WorldItem InteractableItem { get; set; }
    public TestWorldItem InteractableTestItem { get; set; }
    public Structure InteractableStructure { get; set; }
    public GameObject InHandItem { get; set; }
    public GameObject InHandItem2 { get; set; } // ��Ŵ�� ������ �� �ι�° ������
    public GameObject InHeadItem { get; set; } // ���, ���۶� �� �Ӹ��� �����ϴ� ������
    public Item SelectItem { get; set; }
    public ObseravableProperty<float> AirGauge = new();
    public ObseravableProperty<float> ElecticGauge = new();
    public PlayerController Player { get; private set; }
    public float InteractDelay { get; set; }
    public float ItemDelay { get; set; }

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

        AkimboCheck(); // ��Ŵ�� ���� Ȯ��
    }

    private void Init()
    {
        AirGauge.Value = 100f;
        ElecticGauge.Value = 100f;
        Speed = _baseSpeed;
    }

    private void PlayerInit()
    {
        // �÷��̾� �ӽ� ���� �ڵ�
        GameObject player = Instantiate(_playerPrefab, new Vector3(255.86f, 10.24f, -123.64f), Quaternion.identity);
        Player = player.GetComponent<PlayerController>();
        Debug.Log("�÷��̾� ���� �Ϸ�");
    }

    private void AkimboCheck()
    {
        if (_akimboCheck != null) return; // �̹� ��Ŵ���� �޾ƿԴٸ� Ż��


        if (SelectItem != null)
        {
            IsAkimbo = SelectItem.HandleItem.TryGetComponent<Akimbo>(out _akimboCheck);
        }
    }

    public void AkimboReset()
    {
        // ��Ŵ�� ���� �ʱ�ȭ
        IsAkimbo = false;
        _akimboCheck = null;
    }
}
