using UnityEngine;

public enum NPCType
{
    Merchant,       // 상인
    QuestGiver,     // 퀘스트 주는 사람?
    Villager,       // 마을 사람
}

[CreateAssetMenu(fileName = "NPCData", menuName = "Scriptable Objects/NPCData")]
public class NPCData : ScriptableObject
{
    public string npcID;            // NPC 식별자
    public string npcName;          // NPC 이름
    public string[] dialogueID;     // NPC dialogueID
    public NPCType npcType;         // NPC 종류
}

