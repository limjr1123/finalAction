using UnityEngine;

public enum NPCType
{
    Merchant,
    QuestGiver,
    Villager,
    Enemy,
}

[CreateAssetMenu(fileName = "NPCData", menuName = "Scriptable Objects/NPCData")]
public class NPCData : ScriptableObject
{
    public string npcID;
    public string npcName;
    public string[] dialogueID;
    public NPCType npcType;
}

