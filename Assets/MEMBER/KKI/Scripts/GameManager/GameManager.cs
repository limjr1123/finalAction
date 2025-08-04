using System.Runtime.InteropServices;
using GameSave;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();

        // 서비스 등록
        // ServiceLocator.Register<ICharacterCreateService>(new )
    }

    void Start()
    {

    }


    /// <summary>
    /// 저장시에 게임 데이터(gameData) 덮어쓰는 함수
    /// </summary>
    public void SaveGame()
    {
        var gameData = GameDataSaveLoadManager.Instance.GameData;
        if (gameData == null) return;

        int slotIndex = gameData.selectedCharacterSlotIndex; // 현재 선택된 캐릭터

        if (slotIndex < 0 || slotIndex >= gameData.characters.Count)
        {
            Debug.LogError("올바르지 않은 캐릭터 슬롯 인덱스!");
            return;
        }

        // 현재 플레이어 값을 덮어쓰기
        CharacterData characterData = gameData.characters[slotIndex];

        // 플레이어 데이터 저장
        characterData.playerSaveData = PlayerDataManager.Instance.SaveChracterData();

        // 인벤토리 데이터 저장
        characterData.inventorySaveData = InventoryManager.Instance.SaveInventoryData();

        // 퀘스트 데이터 저장
        characterData.questSaveData = QuestManager.Instance.SaveQuestData();

        // 제이슨으로 덮어쓰기.
        GameDataSaveLoadManager.Instance.SaveGameDataToJason();
    }

    /// <summary>
    /// 캐릭터 관련 나머지 데이터 로드할 때 사용 (인벤토리, 퀘스트 등등)
    /// </summary>
    public void LoadGame()
    {
        var gameData = GameDataSaveLoadManager.Instance.GameData;
        if (gameData == null) return;

        int slotIndex = gameData.selectedCharacterSlotIndex; // 현재 선택된 캐릭터

        if (slotIndex < 0 || slotIndex >= gameData.characters.Count)
        {
            Debug.LogError("올바르지 않은 캐릭터 슬롯 인덱스!");
            return;
        }

        CharacterData charData = gameData.characters[slotIndex];

        // 플레이어 데이터 인스턴스 생성 및 로드    
        PlayerDataManager.Instance.LoadCharacterSaveData(charData.playerSaveData);

        // 인벤토리 데이터 로드
        InventoryManager.Instance.LoadInventoryData(charData.inventorySaveData);

        // 퀘스트도 데이터 로드
        QuestManager.Instance.LoadQuestData(charData.questSaveData);

        // 게임 로드 완료 후 HUD와 MainMenu 활성화
        ActivateGameUI();

        // 캐릭터 데이터를 HUD에 반영
        OnCharacterLoadCompleted();
    }

    private void ActivateGameUI()
    {
        // UIManager 아래에서 HUD와 MainMenu UI 찾기
        var uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            // HUD UI 찾아서 활성화
            var hudUI = uiManager.transform.Find("UI_HUD(Heads-Up Display)");  // 또는 정확한 경로
            if (hudUI != null)
            {
                hudUI.gameObject.SetActive(true);
                Debug.Log("HUD UI 활성화 완료!");
            }
            else
            {
                Debug.LogWarning("HUD UI를 찾을 수 없습니다!");
            }

            // MainMenu UI 찾아서 활성화
            var mainMenuUI = uiManager.transform.Find("UI_MainMenu");  // 또는 정확한 경로
            if (mainMenuUI != null)
            {
                mainMenuUI.gameObject.SetActive(true);
                Debug.Log("MainMenu UI 활성화 완료!");
            }
            else
            {
                Debug.LogWarning("MainMenu UI를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogError("UIManager를 찾을 수 없습니다!");
        }
    }

    private void OnCharacterLoadCompleted()
    {
        Debug.Log("캐릭터 로드 완료! UIManager에 알림");

        // UIManager에 캐릭터 로드 완료 알림
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnCharacterLoaded();
        }
        else
        {
            Debug.LogError("UIManager 인스턴스를 찾을 수 없습니다!");
        }
    }
}