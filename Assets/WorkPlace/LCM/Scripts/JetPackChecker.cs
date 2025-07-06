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
        // 각 아이템-오브젝트 쌍을 순회하며 로직 실행
        foreach (var pair in itemActivationPairs)
        {
            if (pair.JetPackToCheck == null)
            {
                Debug.LogWarning($"Item to check is not assigned for an entry in {gameObject.name}'s ItemChecker.");
                continue; // 다음 쌍으로 이동
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
            // OnEnable 시점에 초기 상태를 다시 확인하여 혹시 놓친 업데이트가 있는지 보완
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
        // 모든 아이템-오브젝트 쌍을 다시 확인하여 업데이트
        // 특정 아이템만 확인하도록 최적화할 수도 있지만, 일단은 간단하게 전체 검사
        CheckAndActivateAllObjects();
    }
}
