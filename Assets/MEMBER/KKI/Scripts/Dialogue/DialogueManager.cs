using TMPro;
using UnityEngine;


// 대화 데이터를 불러오고 대화 실행 및 끝났을 때 사용하는 매니저.
public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private DialogueDatabase dialogueDataBase;
    [SerializeField] private DialogueSystem dialoguePanel;


    public void StartDialogue(string dialogueID)
    {
        DialogueData data = dialogueDataBase.GetDialogueData(dialogueID);
        if (data == null) return;

        if (dialoguePanel == null) return;
        Debug.Log("다이로그 패널 열림");
        dialoguePanel.gameObject.SetActive(true);
        dialoguePanel.StartDialogue(data);
    }

    public void EndDialogue()
    {
        dialoguePanel.gameObject.SetActive(false);
    }
}