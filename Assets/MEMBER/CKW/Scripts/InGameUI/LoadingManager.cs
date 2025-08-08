using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : Singleton<LoadingManager>
{
    [Header("Loading UI Components")]
    [SerializeField] CanvasGroup loadingCanvas;
    [SerializeField] Image progressBar;
    [SerializeField] Text percentText;
    [SerializeField] Text loadingText;

    [Header("Animation Settings")]
    [SerializeField] float fadeInSpeed = 5f;  // 빠른 페이드인
    [SerializeField] float fadeOutSpeed = 3f; // 적당한 페이드아웃
    [SerializeField] float progressSmoothSpeed = 2f; // 진행바 부드러움

    private Coroutine loadingTextCoroutine;
    private bool isLoading = false;
    private float currentProgress = 0f;
    private float targetProgress = 0f;

    void Start()
    {
        // 시작할 때 로딩 UI 숨기기
        loadingCanvas.alpha = 0f;
        loadingCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        // 진행바를 부드럽게 업데이트
        if (isLoading && Mathf.Abs(currentProgress - targetProgress) > 0.01f)
        {
            currentProgress = Mathf.Lerp(currentProgress, targetProgress,
                                       progressSmoothSpeed * Time.deltaTime);

            progressBar.fillAmount = currentProgress;
            int percentage = Mathf.RoundToInt(currentProgress * 100f);
            percentText.text = percentage + "%";
        }
    }

    public void StartLoading()
    {
        if (isLoading) return;

        isLoading = true;
        loadingCanvas.gameObject.SetActive(true);

        // 진행도 초기화
        currentProgress = 0f;
        targetProgress = 0f;
        progressBar.fillAmount = 0f;
        percentText.text = "0%";

        // 즉시 페이드인 시작
        StartCoroutine(FadeIn());
        loadingTextCoroutine = StartCoroutine(LoadingTextAnimation());
    }

    public void UpdateProgress(float progress)
    {
        if (!isLoading) return;
        targetProgress = Mathf.Clamp01(progress);
    }

    public void EndLoading()
    {
        if (!isLoading) return;

        // 텍스트 애니메이션 정지
        if (loadingTextCoroutine != null)
        {
            StopCoroutine(loadingTextCoroutine);
            loadingTextCoroutine = null;
        }

        // 페이드아웃
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        while (loadingCanvas.alpha < 1f)
        {
            loadingCanvas.alpha += fadeInSpeed * Time.deltaTime;
            yield return null;
        }
        loadingCanvas.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        while (loadingCanvas.alpha > 0f)
        {
            loadingCanvas.alpha -= fadeOutSpeed * Time.deltaTime;
            yield return null;
        }
        loadingCanvas.alpha = 0f;
        loadingCanvas.gameObject.SetActive(false);
        isLoading = false;

        // 진행도 리셋
        currentProgress = 0f;
        targetProgress = 0f;
    }

    private IEnumerator LoadingTextAnimation()
    {
        string[] loadingTexts = { "Loading.", "Loading..", "Loading..." };
        int currentIndex = 0;

        while (true)
        {
            loadingText.text = loadingTexts[currentIndex];
            currentIndex = (currentIndex + 1) % loadingTexts.Length;
            yield return new WaitForSeconds(0.5f); // 좀 더 빠르게
        }
    }
}
