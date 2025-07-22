using UnityEngine;
using System.Collections.Generic;
using System.Linq;


// 대화 데이터를 불러오고 대화 실행 및 끝났을 때 사용하는 매니저.
public class DialogueManager : Singleton<DialogueManager>
{
    private Dictionary<string, DialogueData> dialogueMap;
    private DialogueSystem dialoguePanel;

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
    }


    void Start()
    {
        // UIManager에서 참조해야 할 듯
        dialoguePanel = FindAnyObjectByType<DialogueSystem>(FindObjectsInactive.Include);
    }

    public void StartDialogue(string dialogueID)
    {
        if (dialoguePanel == null) return;

        if (!dialogueMap.TryGetValue(dialogueID, out DialogueData data)) return;

        // 패널 활성화
        dialoguePanel.gameObject.SetActive(true);
        // 대화 시작
        dialoguePanel.StartDialogue(data);
    }

    public void EndDialogue()
    {
        dialoguePanel.gameObject.SetActive(false);
    }
}