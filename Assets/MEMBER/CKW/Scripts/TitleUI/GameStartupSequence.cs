using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartupSequence : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup developerLogo;    // 개발자 로고
    public CanvasGroup ratingInfo;       // 심의 등급 로고

    [Header("Settings")]
    public float fadeSpeed;        // 페이드 속도
    public float displayDuration;  // 지속 시간
    [SerializeField] AudioClip clip1;
    [SerializeField] AudioClip clip2;
    [SerializeField] AudioSource audioSource;



    void Start()
    {
        StartCoroutine(StartupSequence());
    }


    IEnumerator StartupSequence()
    {
        yield return StartCoroutine(ShowAndHide(developerLogo, clip1));
        yield return StartCoroutine(ShowAndHide(ratingInfo, clip2));
        SceneManager.LoadScene("CKW_TitleScene");
    }


    IEnumerator ShowAndHide(CanvasGroup canvas, AudioClip audioClip)
    {
        // 페이드 인
        yield return StartCoroutine(FadeIn(canvas));

        // 오디오 재생
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        // 지속 시간 대기
        yield return new WaitForSeconds(displayDuration);

        // 오디오 정지 (필요한 경우)
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
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




}
