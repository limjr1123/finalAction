using UnityEngine;


// 플레이어 스크립트에 이 코드 넣어주기
public class NPCTest : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var npc = other.GetComponent<NPC>();
        if (npc is INPCInteractable)
        {
            // 상호작용 키를 누르면
            npc.Interact();
        }
    }
}