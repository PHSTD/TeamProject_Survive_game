using UnityEngine;
using UnityEngine.UI;

public class ShelterMarker : MonoBehaviour
{
    [SerializeField] private Image _shelterMarker;

    private Vector2 _shelterPos;

    private void Awake()
    {
        // ���� �Ա� ��ġ ����. UI�󿡼� ������Ʈ �� ���̱⿡ ��ǥ 2���� �ʿ�
        _shelterPos = new Vector2(257.4693f, -124.8684f);
    }

    private void Update()
    {
        // ��Ŀ�� ��ġ�� �÷��̾��� ��ġ�� ���� ������Ʈ
        SetShelterMarker();
    }

    private void SetShelterMarker()
    {
        // ��Ŀ�� ��ġ�� �ű�� ����
        // ���� �÷��̾��� x,z�� �޾� vector2�� ��ȯ
        Vector2 playerPos = new Vector2(PlayerManager.Instance.Player.transform.position.x, PlayerManager.Instance.Player.transform.position.z);
        // �÷��̾�� ���� ��ġ�� ���� ���
        Vector2 direction = _shelterPos - playerPos;
        // �÷��̾�� ���ͱ��� �Ÿ� ���, ī�޶� 20��ŭ�� ���̸� ���� �� �����Ƿ� ������ �ִ� �Ÿ��� 20���� ���ѵǾ����
        float distance = Mathf.Clamp(direction.magnitude, 0, 20);

        Debug.Log("�÷��̾� ��ǥ: " + playerPos);
        Debug.Log("���� ��ǥ: " + _shelterPos);

        direction.Normalize();

        // ���� ���� �������� ui ��Ŀ�� ��ġ�� ������Ʈ
        // �̴ϸ��� �������� 128�̹Ƿ� �ش� ���� 20�� ���� ������ �����ؾߵ�
        _shelterMarker.rectTransform.anchoredPosition = direction * (distance / 20f) * 128f;
    }
}
