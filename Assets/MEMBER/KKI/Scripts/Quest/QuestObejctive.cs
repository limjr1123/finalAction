public enum ObjectiveType
{
    Clear,
    Kill,
    Collect,
    Talk,
    Reach,
}

[System.Serializable]
public class QuestObjective
{
    public string description;
    public ObjectiveType type;
    public string targetId;     // 몬스터 ID, 아이템 ID, NPC ID 등등
    public int targetAmount;
}