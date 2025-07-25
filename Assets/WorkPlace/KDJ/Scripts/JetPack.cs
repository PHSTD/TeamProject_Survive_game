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

    private void Start()
    {
        //PlayerManager.Instance.CanUseJetpack = _isJetPackOn;
        if (PlayerManager.Instance.CanUseJetpack)
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
            if (!AudioSystem.Instance.IsSFXPlaying)
                AudioSystem.Instance.PlaySFXByName("Sfx_JetpackSound");
        }
        else
        {
            _smokeEffect1.SetActive(false);
            _smokeEffect2.SetActive(false);
            AudioSystem.Instance.StopSFX();
        }
    }

    /// <summary>
    /// 입력으로 카메라의 정면 방향을 받아 해당 방향으로 제트팩을 사용합니다.
    /// </summary>
    /// <param name="camForward">카메라 정면 방향</param>
    public Vector3 UseUpgrade(Vector3 camForward)
    {
        if (PlayerManager.Instance.AirGauge.Value <= 0) return Vector3.zero;

        // 제트팩 사용 시 0.5초당 플레이어의 산소 1 감소

        _airUsage += 2f * Time.deltaTime;

        if (_airUsage >= 0.1f)
        {
            PlayerManager.Instance.AirGauge.Value -= 0.1f;
            _airUsage = 0f;
        }

        return camForward * 7.5f;
    }
}
