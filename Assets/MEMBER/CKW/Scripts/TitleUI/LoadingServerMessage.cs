using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingServerMessage : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI text;

    void Start()
    {
        StartCoroutine(LoadingServerMessageText());
    }



    IEnumerator LoadingServerMessageText()
    {
        for (int i = 0; i < 3; i++)
        {

            text.text = "서버에 접속중입니다.  ";
            yield return new WaitForSeconds(0.3f);
            text.text = "서버에 접속중입니다.. ";
            yield return new WaitForSeconds(0.3f);
            text.text = "서버에 접속중입니다...";
            yield return new WaitForSeconds(0.3f);

        }
        SceneManager.LoadScene(2);
    }


}
