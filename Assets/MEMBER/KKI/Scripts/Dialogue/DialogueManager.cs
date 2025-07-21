using TMPro;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private DialogueDataBase dialogueDataBase;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterNameText;
    public GameObject dialoguePanel;

    private DialogueData currentDialogue;
    private int lineIndex = 0;

    public void StartDialogue(string dialogueID)
    {
        DialogueData data = dialogueDataBase.GetDialogueData(dialogueID);
        if (data == null) return;
        currentDialogue = data;
        lineIndex = 0;
        dialoguePanel.SetActive(true);
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (lineIndex < currentDialogue.lines.Length)
        {
            dialogueText.text = currentDialogue.lines[lineIndex];
            lineIndex++;
        }
        else
        {
            if (currentDialogue.nextDialogueId != null)
            {
                StartDialogue(currentDialogue.nextDialogueId);
            }
            else
            {
                EndDialogue();
            }
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentDialogue = null;
    }
}