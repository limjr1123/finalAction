using System.Collections;
using UnityEngine;

public class LoadingImage : Singleton<LoadingImage>
{

    [SerializeField] CanvasGroup canvas;
    [SerializeField] float speed;


    public void LoadingScene()
    {
        StartCoroutine(FadeInLoading());
        StartCoroutine(FadeOutLoading());

    }

    public IEnumerator FadeInLoading()
    {
        while (canvas.alpha < 1)
        {
            canvas.alpha = Mathf.Clamp01(canvas.alpha + speed * Time.deltaTime);
            yield return null;
        }
        canvas.alpha = 1f;
    }

    public IEnumerator FadeOutLoading()
    {
        while (canvas.alpha > 0)
        {
            canvas.alpha = Mathf.Clamp01(canvas.alpha - speed * Time.deltaTime);
            yield return null;
        }
        canvas.alpha = 0;
    }


}
