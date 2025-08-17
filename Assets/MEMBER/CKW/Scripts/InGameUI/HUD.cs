using TMPro;           // TextMeshPro UI 텍스트 컴포넌트 사용을 위한 네임스페이스
using UnityEngine;     // Unity 엔진의 기본 클래스들 사용을 위한 네임스페이스
using UnityEngine.UI;  // Unity UI 시스템 사용을 위한 네임스페이스
using System;          // C# 기본 시스템 클래스들 사용을 위한 네임스페이스
using GameSave;        // 게임 저장 시스템 관련 네임스페이스
using UnityEngine.SceneManagement; // 씬 관리 관련 네임스페이스

public class HUD : MonoBehaviour  // HUD 클래스 정의, MonoBehaviour를 상속받아 Unity 컴포넌트로 사용 가능
{
    // === 스킬 버튼들을 Inspector에서 할당할 수 있도록 SerializeField로 선언 ===
    [Header("Skill Buttons")]                    // Inspector에서 "Skill Buttons" 헤더로 그룹화
    [SerializeField] Button jumpButton;          // 점프 버튼 참조
    [SerializeField] Button evasionButton;       // 회피 버튼 참조
    [SerializeField] Button counterButton;       // 반격 버튼 참조
    [SerializeField] Button attackButton;        // 공격 버튼 참조
    [SerializeField] Button skill_1;             // 스킬 1 버튼 참조
    [SerializeField] Button skill_2;             // 스킬 2 버튼 참조

    // === 소모품 퀵슬롯 버튼들 ===
    [Header("Consume Quick Slot")]               // Inspector에서 "Consume Quick Slot" 헤더로 그룹화
    [SerializeField] Button consume_1;           // 소모품 1 버튼 참조
    [SerializeField] Button consume_2;           // 소모품 2 버튼 참조

    // === 캐릭터 정보를 표시할 UI 요소들 ===
    [Header("Character Info Display")]           // Inspector에서 "Character Info Display" 헤더로 그룹화
    [SerializeField] TextMeshProUGUI characterNameText;  // 캐릭터 이름 텍스트 UI
    [SerializeField] TextMeshProUGUI levelText;          // 레벨 텍스트 UI
    [SerializeField] Image healthBar;                    // 체력 바 이미지 UI (fillAmount로 체력 비율 표시)
    [SerializeField] TextMeshProUGUI healthText;         // 체력 수치 텍스트 UI
    [SerializeField] Image manaBar;                      // 마나 바 이미지 UI (fillAmount로 마나 비율 표시)
    [SerializeField] TextMeshProUGUI manaText;           // 마나 수치 텍스트 UI
    [SerializeField] Image staminaBar;                   // 스태미나 바 이미지 UI (fillAmount로 스태미나 비율 표시)
    [SerializeField] TextMeshProUGUI staminaText;        // 스태미나 수치 텍스트 UI
    [SerializeField] Image expBar;                       // 경험치 바 이미지 UI (fillAmount로 경험치 비율 표시)
    [SerializeField] TextMeshProUGUI expText;            // 경험치 수치 텍스트 UI


    // === 내부 변수들 ===
    private PlayerStats playerStats;             // 현재 연결된 플레이어 스탯 컴포넌트 참조
    private PlayerStateMachine playerStateMachine;  // 플레이어 상태 머신 참조
    private bool isInitialized = false;          // HUD가 초기화되었는지 확인하는 플래그

    void Start()  // Unity의 Start 메서드, 게임 시작 시 한 번 호출됨
    {
        RegisterButtonEvents();  // 모든 버튼에 클릭 이벤트 등록

        // PlayerStats가 아직 연결되지 않았다면 자동으로 찾아서 연결 시도
        if (playerStats == null)
        {
            StartCoroutine(AutoFindPlayerStats());  // 코루틴으로 비동기적으로 PlayerStats 찾기
        }
        playerStateMachine = playerStats.GetComponent<PlayerStateMachine>();
    }

    // PlayerStats 컴포넌트를 자동으로 찾는 코루틴
    private System.Collections.IEnumerator AutoFindPlayerStats()
    {
        yield return new WaitForSeconds(0.1f);  // 0.1초 대기 (다른 컴포넌트들이 초기화될 시간 확보)

        PlayerStats foundStats = FindAnyObjectByType<PlayerStats>();  // 씬에서 PlayerStats 컴포넌트 찾기
        if (foundStats != null)  // PlayerStats를 찾았다면
        {
            Debug.Log("HUD: PlayerStats 자동 검색으로 발견됨");  // 로그 출력
            InitializeWithPlayer(foundStats);  // 찾은 PlayerStats로 HUD 초기화
        }
    }

    // 모든 버튼에 클릭 이벤트를 등록하는 메서드
    private void RegisterButtonEvents()
    {
        // 각 버튼이 null이 아닌 경우에만 클릭 이벤트 리스너 추가
        if (jumpButton != null)
            jumpButton.onClick.AddListener(OnJumpButton);      // 점프 버튼 클릭 시 OnJumpButton 메서드 호출
        if (evasionButton != null)
            evasionButton.onClick.AddListener(OnEvasionButton);  // 회피 버튼 클릭 시 OnEvasionButton 메서드 호출
        if (counterButton != null)
            counterButton.onClick.AddListener(OnCounterButton);  // 반격 버튼 클릭 시 OnCounterButton 메서드 호출
        if (attackButton != null)
            attackButton.onClick.AddListener(OnAttackButton);    // 공격 버튼 클릭 시 OnAttackButton 메서드 호출
        if (skill_1 != null)
            skill_1.onClick.AddListener(OnSkillButton1);         // 스킬1 버튼 클릭 시 OnSkillButton1 메서드 호출
        if (skill_2 != null)
            skill_2.onClick.AddListener(OnSkillButton2);         // 스킬2 버튼 클릭 시 OnSkillButton2 메서드 호출
        if (consume_1 != null)
            consume_1.onClick.AddListener(OnConsumeButton1);     // 소모품1 버튼 클릭 시 OnConsumeButton1 메서드 호출
        if (consume_2 != null)
            consume_2.onClick.AddListener(OnConsumeButton2);     // 소모품2 버튼 클릭 시 OnConsumeButton2 메서드 호출
    }

    void OnDestroy()  // Unity의 OnDestroy 메서드, 오브젝트가 파괴될 때 호출됨
    {
        UnsubscribeFromEvents();  // 이벤트 구독 해제하여 메모리 누수 방지
    }

    void OnEnable()  // Unity의 OnEnable 메서드, 오브젝트가 활성화될 때 호출됨
    {
        SubscribeToEvents();  // PlayerStats 이벤트 구독
    }

    void OnDisable()  // Unity의 OnDisable 메서드, 오브젝트가 비활성화될 때 호출됨
    {
        UnsubscribeFromEvents();  // PlayerStats 이벤트 구독 해제
    }

    // 플레이어 스탯 컴포넌트로 HUD를 초기화하는 공개 메서드
    public void InitializeWithPlayer(PlayerStats stats)
    {
        Debug.Log($"HUD: InitializeWithPlayer 호출됨. 전달받은 PlayerStats: {stats != null}");  // 초기화 시작 로그

        // 이미 초기화되어 있고 같은 PlayerStats라면 재초기화하지 않음 (중복 초기화 방지)
        if (isInitialized && playerStats == stats)
        {
            Debug.Log("HUD: 이미 같은 PlayerStats로 초기화되어 있음");  // 중복 초기화 방지 로그
            return;  // 메서드 종료
        }

        UnsubscribeFromEvents();  // 기존 이벤트 구독 해제

        playerStats = stats;  // 새로운 PlayerStats 참조 저장

        if (playerStats == null)  // PlayerStats가 null인 경우
        {
            Debug.LogWarning("HUD: PlayerStats 컴포넌트가 전달되지 않았습니다.");  // 경고 로그 출력
            isInitialized = false;  // 초기화 상태를 false로 설정
            return;  // 메서드 종료
        }

        SubscribeToEvents();  // 새로운 PlayerStats의 이벤트 구독

        UpdateAllUI();  // 모든 UI 요소 업데이트

        isInitialized = true;  // 초기화 완료 플래그 설정
        Debug.Log("HUD: 초기 UI 업데이트 완료");  // 초기화 완료 로그
    }

    // PlayerStats의 이벤트에 구독하는 메서드
    private void SubscribeToEvents()
    {
        if (playerStats != null)  // PlayerStats가 null이 아닌 경우에만 실행
        {
            Debug.Log("HUD: PlayerStats 이벤트 구독 시작");  // 이벤트 구독 시작 로그
            // PlayerStats의 각 스탯 변경 이벤트에 해당 UI 업데이트 메서드 연결
            playerStats.OnHealthChanged += UpdateHealthUI;    // 체력 변경 시 체력 UI 업데이트
            playerStats.OnManaChanged += UpdateManaUI;        // 마나 변경 시 마나 UI 업데이트
            playerStats.OnStaminaChanged += UpdateStaminaUI;  // 스태미나 변경 시 스태미나 UI 업데이트
            playerStats.OnEXPChanged += UpdateEXPUI;          // 경험치 변경 시 경험치 UI 업데이트
            playerStats.OnLevelChanged += UpdateLevelUI;      // 레벨 변경 시 레벨 UI 업데이트
            Debug.Log("HUD: PlayerStats 이벤트 구독 완료");  // 이벤트 구독 완료 로그
        }
    }

    // PlayerStats의 이벤트 구독을 해제하는 메서드
    private void UnsubscribeFromEvents()
    {
        if (playerStats != null)  // PlayerStats가 null이 아닌 경우에만 실행
        {
            Debug.Log("HUD: PlayerStats 이벤트 구독 해제");  // 이벤트 구독 해제 로그
            // PlayerStats의 각 이벤트에서 해당 UI 업데이트 메서드 연결 해제
            playerStats.OnHealthChanged -= UpdateHealthUI;    // 체력 변경 이벤트 구독 해제
            playerStats.OnManaChanged -= UpdateManaUI;        // 마나 변경 이벤트 구독 해제
            playerStats.OnStaminaChanged -= UpdateStaminaUI;  // 스태미나 변경 이벤트 구독 해제
            playerStats.OnEXPChanged -= UpdateEXPUI;          // 경험치 변경 이벤트 구독 해제
            playerStats.OnLevelChanged -= UpdateLevelUI;      // 레벨 변경 이벤트 구독 해제
        }
    }

    // 모든 UI 요소를 업데이트하는 메서드 (초기화 시에 호출)
    private void UpdateAllUI()
    {
        if (playerStats == null) return;  // PlayerStats가 null이면 실행하지 않음

        Debug.Log("HUD: 전체 UI 업데이트 시작");  // 전체 UI 업데이트 시작 로그

        // 캐릭터 이름 텍스트 업데이트 (null 체크 후)
        if (characterNameText != null)
            characterNameText.text = playerStats.characterName;

        // 각 UI 요소들을 개별적으로 업데이트
        UpdateLevelUI();     // 레벨 UI 업데이트
        UpdateHealthUI();    // 체력 UI 업데이트
        UpdateManaUI();      // 마나 UI 업데이트
        UpdateStaminaUI();   // 스태미나 UI 업데이트
        UpdateEXPUI();       // 경험치 UI 업데이트

        Debug.Log("HUD: 전체 UI 업데이트 완료");  // 전체 UI 업데이트 완료 로그
    }

    // === 개별 UI 업데이트 메서드들 ===

    // 체력 UI를 업데이트하는 메서드
    private void UpdateHealthUI()
    {
        if (playerStats == null) return;  // PlayerStats가 null이면 실행하지 않음

        // 체력 바 업데이트 (fillAmount는 0.0 ~ 1.0 사이의 값)
        if (healthBar != null)
        {
            // 최대 체력으로 현재 체력을 나누어 비율 계산 (0으로 나누기 방지)
            float fillAmount = playerStats.maxHealth.GetValue() > 0 ?
                (float)playerStats.currentHealth / playerStats.maxHealth.GetValue() : 0f;
            healthBar.fillAmount = fillAmount;  // 체력 바의 채움 정도 설정
        }

        // 체력 텍스트 업데이트 ("현재체력 / 최대체력" 형식)
        if (healthText != null)
        {
            healthText.text = $"{playerStats.currentHealth} / {playerStats.maxHealth.GetValue()}";
        }

        // 체력 업데이트 로그 (디버깅용)
        Debug.Log($"HUD: HP 업데이트 - {playerStats.currentHealth} / {playerStats.maxHealth.GetValue()}");
    }

    // 마나 UI를 업데이트하는 메서드
    private void UpdateManaUI()
    {
        if (playerStats == null) return;  // PlayerStats가 null이면 실행하지 않음

        // 마나 바 업데이트 (fillAmount는 0.0 ~ 1.0 사이의 값)
        if (manaBar != null)
        {
            // 최대 마나로 현재 마나를 나누어 비율 계산 (0으로 나누기 방지)
            float fillAmount = playerStats.maxMana.GetValue() > 0 ?
                (float)playerStats.currentMana / playerStats.maxMana.GetValue() : 0f;
            manaBar.fillAmount = fillAmount;  // 마나 바의 채움 정도 설정
        }

        // 마나 텍스트 업데이트 ("현재마나 / 최대마나" 형식)
        if (manaText != null)
        {
            manaText.text = $"{playerStats.currentMana} / {playerStats.maxMana.GetValue()}";
        }

        // 마나 업데이트 로그 (디버깅용)
        Debug.Log($"HUD: MP 업데이트 - {playerStats.currentMana} / {playerStats.maxMana.GetValue()}");
    }

    // 스태미나 UI를 업데이트하는 메서드
    private void UpdateStaminaUI()
    {
        if (playerStats == null) return;  // PlayerStats가 null이면 실행하지 않음

        // 스태미나 바 업데이트 (fillAmount는 0.0 ~ 1.0 사이의 값)
        if (staminaBar != null)
        {
            // 최대 스태미나로 현재 스태미나를 나누어 비율 계산 (0으로 나누기 방지)
            float fillAmount = playerStats.maxStamina.GetValue() > 0 ?
                (float)playerStats.currentStamina / playerStats.maxStamina.GetValue() : 0f;
            staminaBar.fillAmount = fillAmount;  // 스태미나 바의 채움 정도 설정
        }

        // 스태미나 텍스트 업데이트 ("현재스태미나 / 최대스태미나" 형식)
        if (staminaText != null)
        {
            staminaText.text = $"{playerStats.currentStamina} / {playerStats.maxStamina.GetValue()}";
        }
    }

    // 레벨 UI를 업데이트하는 메서드
    private void UpdateLevelUI()
    {
        if (playerStats == null) return;  // PlayerStats가 null이면 실행하지 않음

        // 레벨 텍스트 업데이트 ("Lv.레벨" 형식)
        if (levelText != null)
        {
            levelText.text = $"Lv.{playerStats.level}";
        }
    }

    // 경험치 UI를 업데이트하는 메서드
    private void UpdateEXPUI()
    {
        if (playerStats == null) return;  // PlayerStats가 null이면 실행하지 않음

        // 경험치 바 업데이트 (fillAmount는 0.0 ~ 1.0 사이의 값)
        if (expBar != null)
        {
            // 최대 경험치로 현재 경험치를 나누어 비율 계산 (0으로 나누기 방지)
            float fillAmount = playerStats.maxEXP.GetValue() > 0 ?
                (float)playerStats.currentEXP / playerStats.maxEXP.GetValue() : 0f;
            expBar.fillAmount = fillAmount;  // 경험치 바의 채움 정도 설정
        }

        // 경험치 텍스트 업데이트 ("현재경험치 / 최대경험치" 형식)
        if (expText != null)
        {
            expText.text = $"{playerStats.currentEXP} / {playerStats.maxEXP.GetValue()}";
        }
    }

    // === 테스트용 메서드 ===

    // 체력 감소 테스트용 메서드 (Unity Inspector의 우클릭 메뉴에서 실행 가능)
    [ContextMenu("Test Health Decrease")]
    public void TestHealthDecrease()
    {
        if (playerStats != null)  // PlayerStats가 연결되어 있다면
        {
            playerStats.TakePhysicalDamage(10);  // 10의 물리 데미지를 입힘
        }
    }

    // === 버튼 클릭 이벤트 메서드들 ===
    // 각 버튼이 클릭되었을 때 호출되는 메서드들 (현재는 로직이 비어있음)

    private void OnJumpButton()     // 점프 버튼 클릭 시 호출
    {
        playerStateMachine?.currentState?.OnJump();
    }

    private void OnEvasionButton()  // 회피 버튼 클릭 시 호출
    {
        playerStateMachine?.currentState?.OnDodge();
    }

    private void OnCounterButton()  // 반격 버튼 클릭 시 호출
    {
        playerStateMachine?.currentState?.OnGuard();
    }

    private void OnAttackButton()   // 공격 버튼 클릭 시 호출
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Dungeon") // 던전에선 공격
        {
            playerStateMachine.currentState?.OnAttack();
        }
        else if (currentSceneName == "Field") // 필드에선 상호작용
        {
            playerStateMachine.Interact();
        }

    }

    private void OnSkillButton1()   // 스킬 1 버튼 클릭 시 호출
    {
        playerStateMachine?.currentState?.OnSkill(1);
    }

    private void OnSkillButton2()   // 스킬 2 버튼 클릭 시 호출
    {
        playerStateMachine?.currentState?.OnSkill(0);
    }

    private void OnConsumeButton1() // 소모품 1 버튼 클릭 시 호출
    {
        // TODO: 소모품 1 사용 로직 구현
    }

    private void OnConsumeButton2() // 소모품 2 버튼 클릭 시 호출
    {
        // TODO: 소모품 2 사용 로직 구현
    }
}