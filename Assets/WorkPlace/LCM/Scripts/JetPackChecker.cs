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
            return; // PlayerManager가 준비될 때까지 기다립니다.
        }

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
                Debug.Log("제트팩활성화");
                PlayerManager.Instance.CanUseJetpack = true;
            }
            else
            {
                Debug.Log("제트팩비활성화");
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
                // Storage도 초기화되지 않은 경우를 대비 (스크립트 실행 순서가 여전히 중요)
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
        // 모든 아이템-오브젝트 쌍을 다시 확인하여 업데이트
        // 특정 아이템만 확인하도록 최적화할 수도 있지만, 일단은 간단하게 전체 검사
        CheckAndJetPackUse();
    }
}
