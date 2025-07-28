using System.Collections;
using UnityEngine;

public class GameStartupSequence : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup developerLogo;    // 개발자 로고
    public CanvasGroup ratingInfo;       // 심의 등급 로고
    public CanvasGroup serverSelect;     // 서버 선택창 캔벌~스

    [Header("Settings")]
    public float fadeSpeed;        // 페이드 속도
    public float displayDuration;  // 지속 시간



    void Start()
    {
        StartCoroutine(StartupSequence());
    }


    IEnumerator StartupSequence()
    {
        yield return StartCoroutine(ShowAndHide(developerLogo));
        yield return StartCoroutine(ShowAndHide(ratingInfo));
        yield return StartCoroutine(FadeIn(serverSelect));
    }


    IEnumerator ShowAndHide(CanvasGroup canvas)
    {
        yield return StartCoroutine(FadeIn(canvas));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeOut(canvas));
    }

    IEnumerator FadeIn(CanvasGroup canvas)
    {
        canvas.gameObject.SetActive(true);
        while (canvas.alpha < 1f)
        {
            canvas.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvas.alpha = 1f;
    }


    IEnumerator FadeOut(CanvasGroup canvas)
    {
        if (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvas.alpha = 0f;
        canvas.gameObject.SetActive(false);
    }

}
