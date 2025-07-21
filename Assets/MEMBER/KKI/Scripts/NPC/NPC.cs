using UnityEngine;


// 나중에 NPC의 속성이 많아지면 상점 NPC, 퀘스트 NPC 등 역할별로 파생 클래스으로 확장
public class NPC : MonoBehaviour, INPCInteractable
{
    [SerializeField] private NPCData npcData;
    [SerializeField] private string startDialogueID;
    private bool isInteract;

    public virtual void Interact()
    {
        if (isInteract == true) return;

        isInteract = true;

        // 상호작용 : 기본 대화, 상점, 퀘스트 수락 등 NPCType에 따라 처리
        // 1. 대화 데이터 및 다른 데이터 보내기.
        // 1-1. 처음에 필요한 대화 데이터만 보내고, 나머지는 대화 박스(UI)에 넣어두기.
        DialogueManager.Instance.StartDialogue(startDialogueID);
        // 2. UI 켜기
    }

    public virtual void EndInteraction()
    {
        if (isInteract == false) return;

        isInteract = false;


        // UI 및 캐릭터에서 대화가 끝났다는 시그널이 오면 이 함수 실행
    }
}