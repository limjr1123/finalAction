using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameSave;


public class InventorySlot
{
    public ItemData data;
    public int count;

    public InventorySlot(ItemData data, int count = 1)
    {
        this.data = data;
        this.count = count;
    }

    public bool IsFull => count >= data.MaxStack;
    public int SpaceLeft => data.MaxStack - count;

    public void Add(int amount)
    {
        count = Mathf.Min(count + amount, data.MaxStack);
    }

    public void Remove(int amount)
    {
        count = Mathf.Max(count - amount, 0);
    }
}

public class InventoryManager : Singleton<InventoryManager>, IInventory
{
    [SerializeField] private ItemDatabase itemDatabase;
    private List<InventorySlot> equipmentInventroy = new();
    private List<InventorySlot> consumableInventroy = new();
    private List<InventorySlot> etcInventroy = new();

    // 각각 슬롯 20개.

    public List<InventorySlot> GetAllEquipmentInventory => equipmentInventroy;
    public List<InventorySlot> GetAllConsumableInventory => consumableInventroy;
    public List<InventorySlot> GetAllEtcInventory => etcInventroy;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            AddItem("Sword");
            AddItem("Axe");
            AddItem("Cheese");
            AddItem("Armor");
            Debug.Log("아이템 추가");
        }
    }

    #region 인벤토리 전용 함수(아이템 추가/삭제)
    public void AddItem(string itemID, int amount = 1)
    {
        // 1. ItemDatabase에 해당 아이템이 있는지 확인
        ItemData itemData = itemDatabase.GetItemData(itemID);
        if (itemData == null)
        {
            Debug.LogWarning($"[Inventory] {itemID}에 해당하는 ItemData를 찾을 수 없습니다.");
            return;
        }

        // 2. 아이템 종류에 따라 인벤토리 선택
        var inventory = GetInventoryListByType(itemData.ItemType);

        int remaining = amount;
        // 3. 스택 가능한 슬롯 채우기
        foreach (var slot in inventory)
        {
            if (slot.data.ItemID == itemID && !slot.IsFull)
            {
                int add = Mathf.Min(slot.SpaceLeft, remaining);
                slot.Add(add);
                remaining -= add;
                if (remaining <= 0) break;
            }
        }

        // 4. 새 슬롯 추가
        while (remaining > 0)
        {
            int add = Mathf.Min(itemData.MaxStack, remaining);
            inventory.Add(new InventorySlot(itemData, add));
            remaining -= add;
        }


        // // UI 최신화
        // if (remaining > 0) Debug.LogWarning($"{remaining}개는 추가되지 않음");
    }

    public void RemoveItem(string itemID, int amount = 1)
    {
        // 1. ItemDatabase에 해당 아이템이 있는지 확인
        ItemData itemData = itemDatabase.GetItemData(itemID);
        if (itemData == null)
        {
            Debug.LogWarning($"[Inventory] {itemID}에 해당하는 ItemData를 찾을 수 없습니다.");
            return;
        }

        // 2. 아이템 종류에 따라 인벤토리 선택
        var inventory = GetInventoryListByType(itemData.ItemType);


        // 3. 아이템 제거하기
        int remaining = amount;

        for (int i = 0; i < inventory.Count && remaining > 0; i++)
        {
            var slot = inventory[i];
            if (slot.data.ItemID != itemID) continue;

            int removeCount = Mathf.Min(slot.count, remaining);
            slot.Remove(removeCount);
            remaining -= removeCount;

            if (slot.count == 0)
            {
                inventory.RemoveAt(i);
                i--;
            }
        }

        if (remaining > 0)
        {
            Debug.LogWarning($"{remaining}개 제거 실패");
        }

        // UI 최신화
    }

    public bool HasItem(string itemID, int amount)
    {
        // 1. ItemDatabase에 해당 아이템이 있는지 확인
        ItemData itemData = itemDatabase.GetItemData(itemID);
        if (itemData == null)
        {
            Debug.LogWarning($"[Inventory] {itemID}에 해당하는 ItemData를 찾을 수 없습니다.");
            return false;
        }

        // 2. 아이템 종류에 따라 인벤토리 선택
        var inventory = GetInventoryListByType(itemData.ItemType);

        // 3. 개수가 맞는지 확인
        int total = inventory.Where(slot => slot.data.ItemID == itemID).Sum(slot => slot.count);
        return total >= amount;
    }

    private List<InventorySlot> GetInventoryListByType(ItemType itemType)
    {
        // switch 식(expression) 구문
        return itemType switch
        {
            ItemType.Equipment => equipmentInventroy,
            ItemType.Consumable => consumableInventroy,
            ItemType.Etc => etcInventroy,
            _ => null
        };
    }

    #endregion

    #region 아이템 인터페이스 (아이템 사용, 장비 장착/해제)

    public void UseItem(string itemID, GameObject user, int amount = 1)
    {
        // 1. ItemDatabase에 해당 아이템이 있는지 확인
        ItemData itemData = itemDatabase.GetItemData(itemID);
        if (itemData == null)
        {
            Debug.LogWarning($"[Inventory] {itemID}에 해당하는 ItemData를 찾을 수 없습니다.");
            return;
        }

        // 2. 아이템 종류에 따라 인벤토리 선택
        var inventory = GetInventoryListByType(itemData.ItemType);

        // 3. 아이템 사용하기
        int remaining = amount;

        for (int i = 0; i < inventory.Count && remaining > 0; i++)
        {
            var slot = inventory[i];
            if (slot.data.ItemID != itemID) continue;

            if (slot.data is IUsable usable)
            {
                usable.Use(user);
                int useAmount = Mathf.Min(slot.count, remaining);
                slot.Remove(useAmount);
                remaining -= useAmount;

                if (slot.count == 0)
                {
                    inventory.RemoveAt(i);
                    i--;
                }
            }
        }

        if (remaining > 0)
        {
            Debug.LogWarning($"[Inventory] {itemID}는 전부 사용되지 않았습니다. 남은 수량: {remaining}");
        }

        // UI 최신화
    }
    public void EquipItem(string itemID, GameObject user)
    {
        TryEquipAction(itemID, user, equip: true);
    }

    public void UnEquipItem(string itemID, GameObject user)
    {
        TryEquipAction(itemID, user, equip: false);
    }

    private void TryEquipAction(string itemID, GameObject user, bool equip)
    {
        // 1. ItemDatabase에 해당 아이템이 있는지 확인
        ItemData itemData = itemDatabase.GetItemData(itemID);
        if (itemData == null)
        {
            Debug.LogWarning($"[Inventory] {itemID}에 해당하는 ItemData를 찾을 수 없습니다.");
            return;
        }

        // 2. 아이템 종류에 따라 인벤토리 선택
        var inventory = GetInventoryListByType(itemData.ItemType);

        // 3. 아이템 장착/해제
        foreach (var slot in inventory)
        {
            if (slot.data.ItemID != itemID || slot.count <= 0) continue;

            if (slot.data is IEquipable equipable)
            {
                if (equip)
                {
                    equipable.Equip(user);
                    RemoveItem(itemID);
                    Debug.Log($"[Inventory] {itemID} 장착 완료.");
                }
                else
                {
                    equipable.Unequip(user);
                    AddItem(itemID);
                    Debug.Log($"[Inventory] {itemID} 장착 해제 완료.");
                }
                return;
            }
        }

        Debug.LogWarning($"[Inventory] {itemID}는 장착 가능한 아이템이 아닙니다.");
    }
    #endregion 

    #region 아이템 저장 및 로드

    public InventorySaveData SaveInventoryData()
    {
        InventorySaveData data = new InventorySaveData();

        foreach (var slot in equipmentInventroy)
        {
            data.equipSlotSaveData.Add(new InventorySlotSaveData { itemID = slot.data.ItemID, count = slot.count });
        }

        foreach (var slot in consumableInventroy)
        {
            data.consumableSlotSaveData.Add(new InventorySlotSaveData { itemID = slot.data.ItemID, count = slot.count });
        }

        foreach (var slot in etcInventroy)
        {
            data.etcSlotSaveData.Add(new InventorySlotSaveData { itemID = slot.data.ItemID, count = slot.count });
        }
        return data;
    }

    public void LoadInventoryData(InventorySaveData inventorySaveData)
    {
        equipmentInventroy.Clear();
        consumableInventroy.Clear();
        etcInventroy.Clear();

        foreach (var item in inventorySaveData.equipSlotSaveData)
        {
            var itemData = itemDatabase.GetItemData(item.itemID);
            if (itemData != null)
                equipmentInventroy.Add(new InventorySlot(itemData, item.count));
        }

        foreach (var item in inventorySaveData.consumableSlotSaveData)
        {
            var itemData = itemDatabase.GetItemData(item.itemID);
            if (itemData != null)
                consumableInventroy.Add(new InventorySlot(itemData, item.count));
        }

        foreach (var item in inventorySaveData.etcSlotSaveData)
        {
            var itemData = itemDatabase.GetItemData(item.itemID);
            if (itemData != null)
                etcInventroy.Add(new InventorySlot(itemData, item.count));
        }
    }


    void B()
    {
        // slot.SetActive();
    }
    #endregion
}
