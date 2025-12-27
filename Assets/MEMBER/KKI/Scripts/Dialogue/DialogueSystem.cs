using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI npcNameText;


    private DialogueData currentDialogue;
    private int lineIndex = 0;
    private int length = 0;

    public void StartDialogue(DialogueData data)
    {
        currentDialogue = data;
        lineIndex = 0;
        length = currentDialogue.lines.Length;
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (lineIndex < length)
        {
            DialogueLine line = currentDialogue.lines[lineIndex];
            npcNameText.text = currentDialogue.npcName;
            dialogueText.text = line.text;

            lineIndex++;
            // 다른 함수 실행
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