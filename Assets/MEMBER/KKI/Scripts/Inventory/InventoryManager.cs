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

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private List<InventorySlot> inventory = new();

    public List<InventorySlot> AllSlots => inventory;

    #region 아이템 추가/삭제/사용
    public void AddItem(string itemID, int amount = 1)
    {
        // 1. ItemDatabase에 해당 아이템이 있는지 확인
        ItemData itemData = itemDatabase.GetItemData(itemID);
        if (itemData == null)
        {
            Debug.LogWarning($"[Inventory] {itemID}에 해당하는 ItemData를 찾을 수 없습니다.");
            return;
        }


        int remaining = amount;
        // 2. 스택 가능한 슬롯 채우기
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

        // 3. 새 슬롯 추가
        while (remaining > 0)
        {
            int add = Mathf.Min(itemData.MaxStack, remaining);
            inventory.Add(new InventorySlot(itemData, add));
            remaining -= add;
        }


        // UI 최신화
        if (remaining > 0) Debug.LogWarning($"{remaining}개는 추가되지 않음");
    }

    public void RemoveItem(string itemID, int amount = 1)
    {
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
        int total = inventory.Where(slot => slot.data.ItemID == itemID).Sum(slot => slot.count);
        return total >= amount;
    }

    public void UseItem(string itemID, GameObject user, int amount = 1)
    {
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
    #endregion

    #region 장비 장착
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
        foreach (var slot in inventory)
        {
            if (slot.data.ItemID != itemID || slot.count <= 0) continue;

            if (slot.data is IEquipable equipable)
            {
                if (equip)
                {
                    equipable.Equip(user);
                    Debug.Log($"[Inventory] {itemID} 장착 완료.");
                }
                else
                {
                    equipable.Unequip(user);
                    Debug.Log($"[Inventory] {itemID} 장착 해제 완료.");
                }
                // UI 최신화
                return;
            }
        }

        Debug.LogWarning($"[Inventory] {itemID}는 장착 가능한 아이템이 아닙니다.");
    }
    #endregion 

    #region 아이템 저장 및 로드

    public InventorySaveData GetInventorySaveData()
    {
        InventorySaveData data = new();
        foreach (var slot in inventory)
        {
            data.items.Add(new InventorySlotSaveData { itemID = slot.data.ItemID, count = slot.count });
        }
        return data;
    }

    public void LoadInventory(InventorySaveData inventorySaveData)
    {
        inventory.Clear();
        foreach (var item in inventorySaveData.items)
        {
            var itemData = itemDatabase.GetItemData(item.itemID);
            if (itemData != null)
                inventory.Add(new InventorySlot(itemData, item.count));
        }
    }
    #endregion
}
