using System.Collections.Generic;

public enum DialogueTriggerType
{
    None,
    OpenShop,
    AcceptQuest,
    GiveItem,
}


[System.Serializable]
public class DialogueLine
{
    public string text;
    public DialogueTriggerType triggerType;
    public string triggerParam;
    public string requiredQuest;
    public bool requireComplete;
}

[System.Serializable]
public class DialogueData
{
    public string dialogueID;       // 대화 식별 ID
    public string npcName;          // NPC 이름
    public DialogueLine[] lines;    // 대화 내용
    public string nextDialogueID;   // 다음 대화 ID
}

[System.Serializable]
public class DialogueDatabase
{
    public List<DialogueData> dialogues;
}