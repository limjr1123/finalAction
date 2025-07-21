using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string dialogueID;
    public string NPCName;
    [TextArea] public string[] lines;
    public string nextDialogueId;
}

