// EquipmentState.cs
using System.Collections.Generic;

public static class EquipmentState
{
    // EquipType별 장착된 슬롯 UID 저장
    private static readonly Dictionary<EquipType, string> equippedUidByType = new();

    public static void SetEquipped(EquipType type, string slotUid) => equippedUidByType[type] = slotUid;
    public static void Clear(EquipType type) => equippedUidByType.Remove(type);
    public static bool IsEquipped(EquipType type, string slotUid) => equippedUidByType.TryGetValue(type, out var u) && u == slotUid;
    public static string GetEquippedUid(EquipType type) => equippedUidByType.TryGetValue(type, out var u) ? u : null;
}
