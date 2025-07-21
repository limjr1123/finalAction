using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 각 토글의 UI 요소들을 묶는 클래스 (프리팹에 붙일 컴포넌트)
public class CharacterToggleUI : MonoBehaviour
{
    [Header("토글")]
    public Toggle toggle;

    [Header("캐릭터 정보 텍스트")]
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI characterClassText;

}
