using System.Collections;
using TMPro;
using UnityEngine;

public class TitleTextAlphaChange : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI text;


    void Start()
    {

        StartCoroutine(TextLightChange());
    }


    IEnumerator TextLightChange()   // 화면 클릭 텍스트 반짝임 효과
    {

        Color originColor = text.color;

        float duration = 1f;
        while (true)
        {

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(1, 0, t / duration);
                text.color = new Color(originColor.r, originColor.g, originColor.b, alpha);
                yield return null;
            }


            text.color = new Color(originColor.r, originColor.g, originColor.b, 0);
            yield return new WaitForSeconds(0.3f);


            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(0, 1, t / duration);
                text.color = new Color(originColor.r, originColor.g, originColor.b, alpha);
                yield return null;
            }
        }
    }
}
