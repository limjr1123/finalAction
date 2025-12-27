using System.Collections.Generic;

public enum DialogueQuestStatus
{
    None,
    NotAccepted,
    InProgress,
    Completed,
}

[System.Serializable]
public class DialogueLine
{
    public string text;
}

[System.Serializable]
public class DialogueData
{
    public string dialogueID;
    public string npcName;
    public DialogueLine[] lines;
    public string nextDialogueID;

    public string requiredQuestID;
    public DialogueQuestStatus requiredQuestStatus;
}

[System.Serializable]
public class DialogueDatabase
{
    public List<DialogueData> dialogues;
}
