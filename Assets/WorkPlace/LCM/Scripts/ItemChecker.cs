using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChecker : MonoBehaviour
{
    [System.Serializable]
    public class ItemActivationPair
    {
        public Item itemToCheck;
        public GameObject objectToActivate;
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
            if (pair.itemToCheck == null)
            {
                Debug.LogWarning($"Item to check is not assigned for an entry in {gameObject.name}'s ItemChecker.");
                continue; // ���� ������ �̵�
            }
            if (pair.objectToActivate == null)
            {
                Debug.LogWarning($"Object to activate is not assigned for '{pair.itemToCheck.itemName}' in {gameObject.name}'s ItemChecker.");
                continue; // ���� ������ �̵�
            }

            if (Storage.Instance.HasItem(pair.itemToCheck))
            {
                if (!pair.objectToActivate.activeSelf) // �̹� Ȱ��ȭ�Ǿ� ���� ���� ���� ����
                {
                    pair.objectToActivate.SetActive(true);
                    Debug.Log($"â�� '{pair.itemToCheck.itemName}'��(��) �����Ƿ� '{pair.objectToActivate.name}'�� Ȱ��ȭ�մϴ�.");
                }
            }
            else
            {
                if (pair.objectToActivate.activeSelf) // �̹� ��Ȱ��ȭ�Ǿ� ���� ���� ���� ����
                {
                    pair.objectToActivate.SetActive(false);
                    Debug.Log($"â�� '{pair.itemToCheck.itemName}'��(��) �����Ƿ� '{pair.objectToActivate.name}'�� ��Ȱ��ȭ�մϴ�.");
                }
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
