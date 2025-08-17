using UnityEngine;
using UnityEngine.EventSystems;

// 이 스크립트를 UI의 가드 버튼 게임 오브젝트에 붙여주세요.
public class GuardButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (PlayerStateMachine.Instance != null)
        {
            PlayerStateMachine.Instance.currentState?.OnGuard();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (PlayerStateMachine.Instance != null)
        {
            PlayerStateMachine.Instance.currentState?.OnGuardUp();
        }
    }
}