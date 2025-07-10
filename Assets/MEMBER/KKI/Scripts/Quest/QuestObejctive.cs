public enum ObjectiveType
{
    Kill,
    Collect,
    Talk,
}

[System.Serializable]
public class QuestObjective
{
    public string desription;
    public ObjectiveType type;
    public string targetId; // 몬스터 ID, 아이템 ID 등등
    public int targetAmount;
}