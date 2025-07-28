using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class OptionUI : BaseUI
{
    [SerializeField] Button closeButton;
    [SerializeField] Button characterSelectButton;
    [SerializeField] Button exitGameButton;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseOptionUI);
        if (characterSelectButton != null)
            characterSelectButton.onClick.AddListener(MoveCharacterSelect);
        if (exitGameButton != null)
            exitGameButton.onClick.AddListener(ExitGame);
    }


    private void CloseOptionUI()
    {
        CloseUI();
    }





    private void MoveCharacterSelect()
    {
        SceneManager.LoadScene(1);
    }

    private void ExitGame()
    {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}