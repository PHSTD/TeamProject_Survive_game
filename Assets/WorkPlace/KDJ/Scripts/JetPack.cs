using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : MonoBehaviour
{
    [SerializeField] GameObject _jetPackObject;
    [SerializeField] Material _noJetPack;
    [SerializeField] Material _jetPack;

    private float _airUsage = 0;

    private void Awake()
    {
        PlayerManager.Instance.IsUpgraded[0] = true;

        if (PlayerManager.Instance.IsUpgraded[0])
        {
            _jetPackObject.GetComponent<Renderer>().material = _jetPack;
        }
        else
        {
            _jetPackObject.GetComponent<Renderer>().material = _noJetPack;
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

        return camForward * 5f;
    }
     
}
