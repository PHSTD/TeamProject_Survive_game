using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseUpgradeMonitor : MonoBehaviour
{
    [System.Serializable] // ����Ƽ �ν����Ϳ��� ����Ʈ/�迭�� ���� ���� ����ϴ�.
    public class UpgradeItemMapping
    {
        public PlayerUpgradeType upgradeType; // Item.cs�� ���ǵ� Enum
        public Item itemData;                 // �ν����Ϳ��� �巡�׾ص������ ������ Item ScriptableObject
        // public int playerManagerUpgradeIndex; // PlayerUpgradeType�� �̹� �ε����� ������ �ִٸ� �ʿ� ����
    }

    [SerializeField] private List<UpgradeItemMapping> _upgradeMappings;

    private Dictionary<PlayerUpgradeType, Item> _upgradeTypeToItemMap;

    private void Awake()
    {
        _upgradeTypeToItemMap = new Dictionary<PlayerUpgradeType, Item>();
        foreach (var mapping in _upgradeMappings)
        {
            if (mapping.itemData != null)
            {
                _upgradeTypeToItemMap[mapping.upgradeType] = mapping.itemData;
            }
            else
            {
                Debug.LogWarning($"WarehouseUpgradeMonitor: '{mapping.upgradeType}'�� ���� Item �����Ͱ� �Ҵ���� �ʾҽ��ϴ�.");
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (Storage.Instance != null)
        {
            // Storage�� OnStorageSlotItemUpdated �̺�Ʈ ����
            Storage.Instance.OnStorageSlotItemUpdated += OnStorageItemUpdated;
            // �ʱ� ���� ������Ʈ
            UpdateAllUpgradeStatuses();
        }
        else
        {
            Debug.LogError("WarehouseUpgradeMonitor: Storage.Instance�� ã�� �� �����ϴ�. Storage�� ���� �ִ��� Ȯ���ϼ���.");
        }
    }

    private void OnDestroy()
    {
        // ��ũ��Ʈ�� �ı��� �� �̺�Ʈ ���� ���� (�޸� ���� ����)
        if (Storage.Instance != null)
        {
            Storage.Instance.OnStorageSlotItemUpdated -= OnStorageItemUpdated;
        }
    }

    private void OnStorageItemUpdated(int slotIndex, Item changedItem, int newQuantity)
    {
        // ��� ��ȭ �������� ���¸� �ٽ� Ȯ���Ͽ� ������Ʈ
        // Ư�� �����۸� ����Ǿ�����, ��� ��ȭ ���¸� �ٽ� �˻��ϴ� ���� ���� �����ϰ� �ܼ��մϴ�.
        UpdateAllUpgradeStatuses();
    }

    private void UpdateAllUpgradeStatuses()
    {
        if (PlayerManager.Instance == null) return;
        if (Storage.Instance == null) return;

        foreach (PlayerUpgradeType type in Enum.GetValues(typeof(PlayerUpgradeType)))
        {
            if (type == PlayerUpgradeType.None) continue; // None�� �ǳʶٱ�

            // �ش� ��ȭ Ÿ�Կ� ���ε� ���� Item �����͸� �����ɴϴ�.
            if (!_upgradeTypeToItemMap.TryGetValue(type, out Item targetItem))
            {
                // ���ε��� ���� ��ȭ Ÿ���� �ǳʶݴϴ�.
                // Debug.LogWarning($"WarehouseUpgradeMonitor: '{type}'�� ���� Item ������ �����ϴ�. Inspector�� Ȯ���ϼ���.");
                continue;
            }

            // Storage�� �ش� �������� �ϳ��� �ִ��� Ȯ��
            bool hasUpgradeItem = Storage.Instance.GetItemCount(targetItem) > 0;

            // PlayerManager�� ��ȭ ���� ������Ʈ �Լ� ȣ��
            //PlayerManager.Instance.SetPlayerUpgradeState(type, hasUpgradeItem);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
