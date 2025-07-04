using UnityEngine;

public class JetPack : MonoBehaviour
{
    [SerializeField] private GameObject _jetPackObject;
    // [SerializeField] Material _noJetPack;
    // [SerializeField] Material _jetPack;
    [SerializeField] private bool _isJetPackOn;
    [SerializeField] private GameObject _smokeEffect1;
    [SerializeField] private GameObject _smokeEffect2;

    private float _airUsage = 0;
    private GameObject _smokeEffectInstance1;
    private GameObject _smokeEffectInstance2;

    private void Awake()
    {
        PlayerManager.Instance.IsUpgraded[0] = _isJetPackOn;

        if (PlayerManager.Instance.IsUpgraded[0])
        {
            _jetPackObject.SetActive(true);
        }
        else
        {
            _jetPackObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (PlayerManager.Instance.Player.IsUsingJetPack)
        {
            _smokeEffect1.SetActive(true);
            _smokeEffect2.SetActive(true);
        }
        else
        {
            _smokeEffect1.SetActive(false);
            _smokeEffect2.SetActive(false);
        }
    }

    /// <summary>
    /// �Է����� ī�޶��� ���� ������ �޾� �ش� �������� ��Ʈ���� ����մϴ�.
    /// </summary>
    /// <param name="camForward">ī�޶� ���� ����</param>
    public Vector3 UseUpgrade(Vector3 camForward)
    {
        if (!PlayerManager.Instance.IsUpgraded[0] || PlayerManager.Instance.AirGauge.Value <= 0) return Vector3.zero;

        // ��Ʈ�� ��� �� 0.5�ʴ� �÷��̾��� ��� 1 ����
        
        _airUsage += 2f * Time.deltaTime;

        if (_airUsage >= 1f)
        {
            PlayerManager.Instance.AirGauge.Value -= 1f;
            _airUsage = 0f;
        }

        return camForward * 7.5f;
    }
     
}
