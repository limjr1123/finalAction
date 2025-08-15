using System;

public static class GameEvents
{
    public static event Action<string> OnEnemyKilled;
    public static event Action<string, int> OnItemGet;
    public static event Action<string> OnDungeonClear;
    public static event Action<string> OnDungeonReach;

    public static event Action<string> OnNPCTalked;


    public static void EnemyKilled(string enemyID) => OnEnemyKilled?.Invoke(enemyID);
    public static void ItemGet(string itemID, int amount = 1) => OnItemGet?.Invoke(itemID, amount);
    public static void DungeonReach(string dungeonID) => OnDungeonReach?.Invoke(dungeonID);
    public static void DungeonClear(string dungeonID) => OnDungeonClear?.Invoke(dungeonID);

    public static void NPCTalked(string npcID) => OnNPCTalked?.Invoke(npcID);
}