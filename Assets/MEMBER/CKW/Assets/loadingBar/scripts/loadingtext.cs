using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadingtext : MonoBehaviour
{
    private RectTransform rectComponent;
    private Image imageComp;

    public float speed = 200f;
    public Text text;
    public Text textNormal;

    private Coroutine loadingTextCoroutine; // 코루틴 참조 저장용

    void Start()
    {
        rectComponent = GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();
        imageComp.fillAmount = 0.0f;
    }

    void Update()
    {
        int a = 0;
        if (imageComp.fillAmount != 1f)
        {
            imageComp.fillAmount = imageComp.fillAmount + Time.deltaTime * speed;
            a = (int)(imageComp.fillAmount * 100);

            if (a > 0 && a <= 100)
            {
                // 코루틴이 실행중이 아니라면 시작
                if (loadingTextCoroutine == null)
                {
                    loadingTextCoroutine = StartCoroutine(LoadingTextAnimation());
                }
            }
            text.text = a + "%";
        }
        else
        {
            // 로딩 완료 시 코루틴 정지
            if (loadingTextCoroutine != null)
            {
                StopCoroutine(loadingTextCoroutine);
                loadingTextCoroutine = null;
            }

            imageComp.fillAmount = 0.0f;
            text.text = "0%";
            textNormal.text = "Loading."; // 기본 텍스트로 리셋
        }
    }

    // 1초마다 Loading 텍스트를 변경하는 코루틴
    private IEnumerator LoadingTextAnimation()
    {
        string[] loadingTexts = { "Loading.", "Loading..", "Loading..." };
        int currentIndex = 0;

        while (true)
        {
            textNormal.text = loadingTexts[currentIndex];
            currentIndex = (currentIndex + 1) % loadingTexts.Length; // 0, 1, 2, 0, 1, 2...
            yield return new WaitForSeconds(1f); // 1초 대기
        }
    }
}