using UnityEngine;
using TMPro;
using UnityEngine.Analytics;
using Unity.VisualScripting;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI npcNameText;


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
            DialogueLine line = currentDialogue.lines[lineIndex];

            // 조건부 대사 처리
            if (!string.IsNullOrEmpty(line.requiredQuest))
            {
                // bool questDone = QuestManager.Instance.IsQuestCompleted(line.requiredQuest);
                // if (line.requireComplete && !questDone)
                // {
                //     lineIndex ++;
                //     // 조건 불만족
                // }
            }

            npcNameText.text = currentDialogue.npcName;
            dialogueText.text = line.text;

            if (line.triggerType != DialogueTriggerType.None)
            {
                HandleTrigger(line);
            }

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
    void HandleTrigger(DialogueLine line)
    {
        switch (line.triggerType)
        {
            case DialogueTriggerType.OpenShop:
                // ShopManager.Instance.OpenShop(line.triggerParam);
                break;
            case DialogueTriggerType.AcceptQuest:
                // QuestManager.Instance.AcceptQuest(line.triggerParam);
                break;
                // ...필요시 확장
        }
    }

}