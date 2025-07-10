using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_CharacterServerButton : MonoBehaviour
{

    [Header("Button Settings")]
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button serverChangeButton;
    [SerializeField] Button characterCreationButton;
    [SerializeField] Button characterDeleteButton;


    [SerializeField] GameObject characterSelectionWindow;



    void Start()
    {

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);

        if (serverChangeButton != null)
            serverChangeButton.onClick.AddListener(ServerChange);

        if (characterCreationButton != null)
            characterCreationButton.onClick.AddListener(CharacterCreate);

        if (characterDeleteButton != null)
            characterDeleteButton.onClick.AddListener(CharacterDelete);
    }


    private void StartGame()
    {

    }


    private void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    private void ServerChange()
    {
        SceneManager.LoadScene(1);
    }


    private void CharacterCreate()
    {
        characterSelectionWindow.SetActive(true);
    }


    private void CharacterDelete()
    {

    }

}
