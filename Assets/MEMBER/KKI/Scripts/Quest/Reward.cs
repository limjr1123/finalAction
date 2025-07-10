public enum RewardType
{
    Gold,
    Exp,
    Item,
}

[System.Serializable]
public class Reward
{
    public RewardType type;
    public int amount;
    public string itemId;
}

