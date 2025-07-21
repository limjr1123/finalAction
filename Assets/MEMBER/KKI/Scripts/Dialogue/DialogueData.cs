using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string dialogueID;
    public string characterName;
    [TextArea] public string[] lines;
    public string nextDialogueId;
}

