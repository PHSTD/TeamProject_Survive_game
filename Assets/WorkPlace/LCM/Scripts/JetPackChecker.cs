using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPackChecker : MonoBehaviour
{
    [System.Serializable]
    public class ItemActivationPair
    {
        public Item JetPackToCheck;
    }

    [SerializeField]
    private List<ItemActivationPair> itemActivationPairs;

    void Start()
    {
        CheckAndJetPackUse();
    }

    public void CheckAndJetPackUse()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogWarning("PlayerManager.Instance is not yet initialized. Cannot check jetpack use.");
            return; // PlayerManager�� �غ�� ������ ��ٸ��ϴ�.
        }

        // �� ������-������Ʈ ���� ��ȸ�ϸ� ���� ����
        foreach (var pair in itemActivationPairs)
        {
            if (pair.JetPackToCheck == null)
            {
                Debug.LogWarning($"Item to check is not assigned for an entry in {gameObject.name}'s ItemChecker.");
                continue; // ���� ������ �̵�
            }

            if (Storage.Instance.HasItem(pair.JetPackToCheck))
            {
                Debug.Log("��Ʈ��Ȱ��ȭ");
                PlayerManager.Instance.CanUseJetpack = true;
            }
            else
            {
                Debug.Log("��Ʈ�Ѻ�Ȱ��ȭ");
                PlayerManager.Instance.CanUseJetpack = false;
            }
        }
    }

    private void OnEnable()
    {
        if (Storage.Instance != null)
        {
            if (Storage.Instance != null)
            {
                Storage.Instance.OnStorageSlotItemUpdated += OnStorageUpdated;
            }
            else
            {
                // Storage�� �ʱ�ȭ���� ���� ��츦 ��� (��ũ��Ʈ ���� ������ ������ �߿�)
                Debug.LogError("JetPackChecker: Storage.Instance is null on OnEnable. Ensure Storage initializes first.");
            }
        }
    }

    private void OnDisable()
    {
        if (Storage.Instance != null)
        {
            Storage.Instance.OnStorageSlotItemUpdated -= OnStorageUpdated;
        }
    }

    private void OnStorageUpdated(int index, Item item, int quantity)
    {
        // ��� ������-������Ʈ ���� �ٽ� Ȯ���Ͽ� ������Ʈ
        // Ư�� �����۸� Ȯ���ϵ��� ����ȭ�� ���� ������, �ϴ��� �����ϰ� ��ü �˻�
        CheckAndJetPackUse();
    }
}
