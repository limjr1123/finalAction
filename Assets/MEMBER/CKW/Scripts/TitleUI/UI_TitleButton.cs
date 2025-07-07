using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_TitleButton : MonoBehaviour
{
    [SerializeField] Button preyServerButton;
    [SerializeField] GameObject loadingServerMessage;



    void Start()
    {
        if (preyServerButton != null)
            preyServerButton.onClick.AddListener(OnEnterServer);
    }



    private void OnEnterServer()
    {
        loadingServerMessage.SetActive(true);
    }

}
