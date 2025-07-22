using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI NPCNameText;


    private DialogueData currentDialogue;
    private int lineIndex = 0;

    public void StartDialogue(DialogueData data)
    {
        currentDialogue = data;
        lineIndex = 0;
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (lineIndex < currentDialogue.lines.Length)
        {
            NPCNameText.text = currentDialogue.npcName;
            dialogueText.text = currentDialogue.lines[lineIndex].text;
            lineIndex++;
        }
        else
        {
            if (!string.IsNullOrEmpty(currentDialogue.nextDialogueID))
            {
                DialogueManager.Instance.StartDialogue(currentDialogue.nextDialogueID);
            }
            else
            {
                DialogueManager.Instance.EndDialogue();
            }
        }
    }
}