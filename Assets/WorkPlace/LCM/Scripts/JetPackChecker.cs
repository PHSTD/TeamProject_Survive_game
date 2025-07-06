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
        CheckAndActivateAllObjects();
    }

    public void CheckAndActivateAllObjects()
    {
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
                PlayerManager.Instance.CanUseJetpack = true;
            }
            else
            {
                PlayerManager.Instance.CanUseJetpack = false;
            }
        }
    }

    private void OnEnable()
    {
        if (Storage.Instance != null)
        {
            Storage.Instance.OnStorageSlotItemUpdated += OnStorageUpdated;
            // OnEnable ������ �ʱ� ���¸� �ٽ� Ȯ���Ͽ� Ȥ�� ��ģ ������Ʈ�� �ִ��� ����
            CheckAndActivateAllObjects();
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
        CheckAndActivateAllObjects();
    }
}
