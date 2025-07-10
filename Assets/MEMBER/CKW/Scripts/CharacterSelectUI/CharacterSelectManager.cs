using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 컴포넌트를 사용하기 위해 필요
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class CharacterSelectManager : MonoBehaviour
{
    // 각 캐릭터 슬롯에 연결할 Toggle 컴포넌트 리스트
    // 인스펙터에서 직접 드래그 앤 드롭으로 연결해주세요.
    public List<Toggle> characterSlotToggles;

    // 게임 시작 버튼
    // 인스펙터에서 "시작하기" 버튼의 Button 컴포넌트를 연결해주세요.
    public Button startGameButton;

    // 현재 선택된 캐릭터 슬롯의 인덱스 (0부터 시작)
    // -1은 아무것도 선택되지 않았음을 의미합니다.
    private int selectedCharacterSlotIndex = -1;

    void Start()
    {
        // 각 캐릭터 슬롯(Toggle)에 클릭 리스너 등록
        for (int i = 0; i < characterSlotToggles.Count; i++)
        {
            // 클로저(Closure) 이슈 방지를 위해 현재 인덱스를 지역 변수에 복사
            int index = i;
            characterSlotToggles[i].onValueChanged.AddListener((isOn) => OnCharacterSlotSelected(index, isOn));
        }

        // 게임 시작 버튼에 클릭 리스너 등록
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);

        // 초기 상태: 선택된 캐릭터가 없으므로 게임 시작 버튼 비활성화
        startGameButton.interactable = false;
    }

    /// <summary>
    /// 캐릭터 슬롯(Toggle)이 선택되거나 해제될 때 호출되는 메서드.
    /// </summary>
    /// <param name="index">선택/해제된 슬롯의 인덱스 (0부터 시작).</param>
    /// <param name="isOn">슬롯이 선택되었으면 true, 해제되었으면 false.</param>
    private void OnCharacterSlotSelected(int index, bool isOn)
    {
        if (isOn) // 슬롯이 선택되었을 때 (Toggle이 켜졌을 때)
        {
            selectedCharacterSlotIndex = index;
            Debug.Log($"캐릭터 슬롯 {index}번이 선택되었습니다.");

            // 선택된 슬롯이 비어있는지 여부는 외부 스크립트에서 관리하므로,
            // 여기서는 단순히 선택된 슬롯이 유효하면 게임 시작 버튼 활성화.
            // (나중에 빈 슬롯 클릭 시 게임 시작 버튼 비활성화 로직 추가 가능)
            startGameButton.interactable = true;
        }
        else // 슬롯이 해제되었을 때 (Toggle이 꺼졌을 때)
        {
            // Toggle Group을 사용하면 다른 토글이 켜질 때 자동으로 이전 토글이 꺼지므로,
            // 현재 꺼진 토글이 이전에 선택되었던 토글이었는지 확인하는 로직이 필요.
            if (selectedCharacterSlotIndex == index)
            {
                selectedCharacterSlotIndex = -1; // 선택 해제 상태로 변경
                startGameButton.interactable = false; // 게임 시작 버튼 비활성화
                Debug.Log($"캐릭터 슬롯 {index}번 선택이 해제되었습니다.");
            }
        }
    }

    /// <summary>
    /// "시작하기" 버튼이 클릭될 때 호출되는 메서드.
    /// </summary>
    private void OnStartGameButtonClicked()
    {
        // 유효한 캐릭터 슬롯이 선택되었는지 확인
        if (selectedCharacterSlotIndex != -1)
        {
            Debug.Log($"게임 시작! 선택된 캐릭터 슬롯: {selectedCharacterSlotIndex}번");

            // TODO: 이곳에서 실제 게임 시작 로직을 구현합니다.
            // 1. 선택된 selectedCharacterSlotIndex를 다른 스크립트(예: GameLoader, CharacterDataManager 등)에 전달.
            //    - 예시: GlobalGameManager.Instance.LoadCharacter(selectedCharacterSlotIndex);
            // 2. 해당 슬롯의 캐릭터 정보를 로드하거나, 캐릭터 생성 과정을 건너뛰고 게임 씬으로 바로 전환.

            // 씬 전환 예시 (실제 게임 씬 이름으로 변경하세요)
            SceneManager.LoadScene("MainGameScene");
        }
        else
        {
            Debug.LogWarning("게임을 시작하려면 먼저 캐릭터 슬롯을 선택해주세요.");
            // 사용자에게 알림 UI (팝업 등)를 띄울 수도 있습니다.
        }
    }

    // 외부에서 캐릭터 정보가 로드된 후 UI 상태를 업데이트해야 할 경우를 위한 더미 메서드 (참고용)
    // 실제 캐릭터 데이터 로딩 후, 이 Manager가 각 슬롯의 활성화/비활성화 및 텍스트 업데이트를 요청할 수 있습니다.
    public void NotifyCharactersLoaded()
    {
        // 예를 들어, 캐릭터가 있는 슬롯은 Toggle을 활성화하고, 빈 슬롯은 비활성화하거나,
        // "캐릭터 생성" 버튼만 보이게 하는 등의 UI 로직을 여기서 처리할 수 있습니다.
        // 현재는 Toggle이 기본적으로 모두 활성화된 상태를 가정합니다.

        // 캐릭터 정보 로드 후, 혹시 선택된 것이 있다면 초기화하고 버튼 비활성화
        selectedCharacterSlotIndex = -1;
        startGameButton.interactable = false;

        Debug.Log("외부에서 캐릭터 정보 로드 완료 알림을 받았습니다. 필요시 UI 업데이트 수행.");
    }
}