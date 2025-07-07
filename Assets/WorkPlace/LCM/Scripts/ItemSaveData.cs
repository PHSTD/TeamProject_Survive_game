using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSaveData
{
    public string itemName;
}


[System.Serializable]
public class InventoryItemSaveData
{
    public ItemSaveData itemData;
    public int currentQuantity;
}

[System.Serializable]
public class InventorySlotSaveData
{
    public InventoryItemSaveData itemInSlot;
}

[System.Serializable]
public class StorageSaveData
{
    public List <InventorySlotSaveData> slots;

    public StorageSaveData()
    {
        slots = new List<InventorySlotSaveData>();
    }
}

