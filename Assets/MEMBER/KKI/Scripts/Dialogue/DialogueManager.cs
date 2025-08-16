using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : Singleton<DialogueManager>
{
    private Dictionary<string, DialogueData> dialogueMap;
    private DialogueSystem dialoguePanel;

    // 현재 대화 중인 상호작용자 보관
    private INPCInteractable currentInteractor;

    protected override void Awake()
    {
        base.Awake();
        LoadDatabase();
    }

    void LoadDatabase()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("dialogues");
        var db = JsonUtility.FromJson<DialogueDatabase>(jsonText.text);
        dialogueMap = db.dialogues.ToDictionary(d => d.dialogueID);

        foreach (var kv in dialogueMap)
        {
            var d = kv.Value;
            Debug.Log($"[DialogueDB] {d.dialogueID} rq={d.requiredQuestID} rs={d.requiredQuestStatus}");
            break;
        }
    }

    public DialogueData GetDialogueData(string dialogueID)
    {
        dialogueMap.TryGetValue(dialogueID, out DialogueData data);
        return data;
    }

    public void StartDialogue(string dialogueID, INPCInteractable interactor = null)
    {
        if (dialoguePanel == null)
            dialoguePanel = FindAnyObjectByType<DialogueSystem>(FindObjectsInactive.Include);

        if (!dialogueMap.TryGetValue(dialogueID, out DialogueData data)) return;

        // interactor가 전달되면 현재 화자로 등록, null이면 기존 값 유지
        if (interactor != null)
            currentInteractor = interactor;

        dialoguePanel.gameObject.SetActive(true);
        dialoguePanel.StartDialogue(data);
    }

    public void EndDialogue()
    {
        // 대화 UI 닫기
        if (dialoguePanel != null)
            dialoguePanel.gameObject.SetActive(false);

        // 여기서 상호작용 종료 신호 보내기
        if (currentInteractor != null)
        {
            currentInteractor.EndInteraction();
            currentInteractor = null;
        }
    }
}
