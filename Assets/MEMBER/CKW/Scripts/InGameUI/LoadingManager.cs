using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : Singleton<LoadingManager>
{
    [Header("Loading UI Components")]
    [SerializeField] CanvasGroup loadingCanvas; // 로딩 화면 전체 캔버스 그룹
    [SerializeField] Image progressBar; // 진행률을 보여줄 프로그레스바
    [SerializeField] Text percentText; // 퍼센트 텍스트 (0%, 50%, 100% 등)
    [SerializeField] Text loadingText; // "Loading..." 애니메이션 텍스트

    [Header("Animation Settings")]
    [SerializeField] float fadeInSpeed = 5f;  // 로딩화면 나타날 때 속도 (빠른 페이드인)
    [SerializeField] float fadeOutSpeed = 3f; // 로딩화면 사라질 때 속도 (적당한 페이드아웃)
    [SerializeField] float progressSmoothSpeed = 2f; // 진행바가 부드럽게 채워지는 속도

    private Coroutine loadingTextCoroutine; // "Loading..." 텍스트 애니메이션 코루틴
    private bool isLoading = false; // 현재 로딩 중인지 확인하는 플래그
    private float currentProgress = 0f; // 현재 진행률 (부드러운 애니메이션용)
    private float targetProgress = 0f; // 목표 진행률 (실제 로딩 진행률)

    void Start()
    {
        // 게임 시작할 때 로딩 UI 완전히 숨기기
        loadingCanvas.alpha = 0f; // 투명하게 만들기
        loadingCanvas.gameObject.SetActive(false); // 오브젝트 비활성화
    }

    void Update()
    {
        // 매 프레임마다 진행바를 부드럽게 업데이트
        if (isLoading && Mathf.Abs(currentProgress - targetProgress) > 0.01f) // 로딩 중이고 현재값과 목표값에 차이가 있으면
        {
            // 현재 진행률을 목표값으로 부드럽게 보간
            currentProgress = Mathf.Lerp(currentProgress, targetProgress,
                                       progressSmoothSpeed * Time.deltaTime);

            progressBar.fillAmount = currentProgress; // 진행바 채우기 (0~1 사이 값)
            int percentage = Mathf.RoundToInt(currentProgress * 100f); // 0~100 퍼센트로 변환
            percentText.text = percentage + "%"; // 퍼센트 텍스트 업데이트
        }
    }

    public void StartLoading()
    {
        if (isLoading) return; // 이미 로딩 중이면 중복 실행 방지

        isLoading = true; // 로딩 상태로 변경
        loadingCanvas.gameObject.SetActive(true); // 로딩 캔버스 활성화

        // 진행도 초기화
        currentProgress = 0f; // 현재 진행률 0으로 리셋
        targetProgress = 0f; // 목표 진행률 0으로 리셋
        progressBar.fillAmount = 0f; // 진행바 비우기
        percentText.text = "0%"; // 퍼센트 텍스트 0%로 설정

        // 페이드인 애니메이션과 텍스트 애니메이션 시작
        StartCoroutine(FadeIn()); // 로딩화면 서서히 나타내기
        loadingTextCoroutine = StartCoroutine(LoadingTextAnimation()); // "Loading..." 애니메이션 시작
    }

    public void UpdateProgress(float progress)
    {
        if (!isLoading) return; // 로딩 중이 아니면 무시
        targetProgress = Mathf.Clamp01(progress); // 0~1 사이 값으로 제한하여 목표 진행률 설정
    }

    public void EndLoading()
    {
        if (!isLoading) return; // 로딩 중이 아니면 무시

        // 텍스트 애니메이션 정지
        if (loadingTextCoroutine != null) // 텍스트 애니메이션이 실행 중이면
        {
            StopCoroutine(loadingTextCoroutine); // 코루틴 중지
            loadingTextCoroutine = null; // 레퍼런스 제거
        }

        // 페이드아웃 시작
        StartCoroutine(FadeOut()); // 로딩화면 서서히 사라지게 하기
    }

    private IEnumerator FadeIn()
    {
        while (loadingCanvas.alpha < 1f) // 완전히 불투명해질 때까지 반복
        {
            // 투명도를 점진적으로 증가 (deltaTime 사용으로 프레임레이트 독립적)
            loadingCanvas.alpha += fadeInSpeed * Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }
        loadingCanvas.alpha = 1f; // 완전 불투명으로 설정 (오버슈트 방지)
    }

    private IEnumerator FadeOut()
    {
        while (loadingCanvas.alpha > 0f) // 완전히 투명해질 때까지 반복
        {
            // 투명도를 점진적으로 감소
            loadingCanvas.alpha -= fadeOutSpeed * Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }
        loadingCanvas.alpha = 0f; // 완전 투명으로 설정
        loadingCanvas.gameObject.SetActive(false); // 오브젝트 비활성화 (성능 최적화)
        isLoading = false; // 로딩 상태 해제

        // 진행도 완전히 리셋
        currentProgress = 0f; // 현재 진행률 초기화
        targetProgress = 0f; // 목표 진행률 초기화
    }

    private IEnumerator LoadingTextAnimation()
    {
        string[] loadingTexts = { "Loading.", "Loading..", "Loading..." }; // 애니메이션에 사용할 텍스트들
        int currentIndex = 0; // 현재 표시할 텍스트의 인덱스

        while (true) // 무한 반복 (EndLoading에서 중지될 때까지)
        {
            loadingText.text = loadingTexts[currentIndex]; // 현재 인덱스의 텍스트 표시
            currentIndex = (currentIndex + 1) % loadingTexts.Length; // 다음 인덱스로 순환 (0, 1, 2, 0, 1, 2...)
            yield return new WaitForSeconds(0.5f); // 0.5초 대기 (좀 더 빠르게)
        }
    }
}