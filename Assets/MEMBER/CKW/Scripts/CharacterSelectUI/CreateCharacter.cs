using UnityEngine;
using UnityEngine.UI;

public class CreateCharacter : MonoBehaviour
{
    [SerializeField] GameObject nicknameWindow;


    [SerializeField] Button nicknameButton;
    [SerializeField] Button closeButton;


    void Start()
    {
        if (nicknameButton != null)
            nicknameButton.onClick.AddListener(CreateCharacters);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseNickname);
    }

    private void CreateCharacters()
    {
        //GameDataSaveLoadManager.Instance.CreateDummyData(); //함수 삭제로 인한 주석처리
    }


    private void CloseNickname()
    {
        nicknameWindow.SetActive(false);
    }

}
