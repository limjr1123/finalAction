using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogueDataBase", menuName = "Scriptable Objects/Database/DialogueDataBase")]
public class DialogueDataBase : ScriptableObject
{
    [SerializeField] private List<DialogueData> allDialogueDatas;

    private Dictionary<string, DialogueData> dialogueMap;

    public void Initialize()
    {
        dialogueMap = allDialogueDatas.ToDictionary(dialogueDatas => dialogueDatas.dialogueID);
    }

    public DialogueData GetDialogueData(string DialogueID)
    {
        if (dialogueMap == null)
        {
            Initialize();
        }

        return dialogueMap.TryGetValue(DialogueID, out var data) ? data : null;
    }

}