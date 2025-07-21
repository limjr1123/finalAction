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
            NPCNameText.text = currentDialogue.NPCName;
            dialogueText.text = currentDialogue.lines[lineIndex];
            lineIndex++;
        }
        else
        {
            // 다음 대화가 있으면 진행
            if (currentDialogue.nextDialogueId != null)
            {
                DialogueManager.Instance.StartDialogue(currentDialogue.nextDialogueId);
            }
            // 다음 대화가 없으면 종료
            else
            {
                DialogueManager.Instance.EndDialogue();
            }
        }
    }
}