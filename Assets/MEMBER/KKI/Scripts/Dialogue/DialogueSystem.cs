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

            // 조건부 대사 처리
            if (!string.IsNullOrEmpty(line.requiredQuest))
            {
                bool questDone = QuestManager.Instance.IsQuestCompleted(line.requiredQuest);

                if (questDone == true)
                {
                    lineIndex++;
                }
                else
                {
                    // 다르게 대화 진행
                    return; // 이렇게 함수 스택이 계속 쌓임.
                }

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

    /// <summary>
    /// 대화 내용에서 나타나는 트리거에 라인이 있는지 확인하는 함수
    /// </summary>
    /// <param name="line"></param>
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