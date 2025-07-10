using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    public string questID;
    public string title;
    public string desription;
    public QuestObjective[] questObjectives;
    public Reward[] rewards;
}

