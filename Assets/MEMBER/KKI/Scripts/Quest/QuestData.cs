using UnityEngine;

public enum QuestType
{
    Main,
    Sub,
}


[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    public string questID;
    public QuestType questType;
    public string title;
    public string desription;
    public QuestObjective[] questObjectives;
    public Reward[] rewards;
    public string nextQuestID;
}