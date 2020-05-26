using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToMainMenu : MonoBehaviour
{
    public GameObject BackPanel;
    public Button CancelButton;
    public Button GoToMainMenuButton;
    public Button MenuButton;
    bool GoToMainMenuBool = false;

    void Awake()
    {
        CancelButton.onClick.AddListener(CancelButtonClicked);
        MenuButton.onClick.AddListener(MenuButtonClicked);
        GoToMainMenuButton.onClick.AddListener(GoToMainMenuClicked);
    }

    void CancelButtonClicked() // 메인메뉴 Cancel 버튼시
    {
        BackPanel.gameObject.SetActive(false);
    }
    void MenuButtonClicked() // 메뉴버튼 클릭시
    {
        if(GoToMainMenuBool == false) { 
        GoToMainMenuButton.gameObject.SetActive(true);
            GoToMainMenuBool = true;
        }
        else
        {
            GoToMainMenuButton.gameObject.SetActive(false);
            GoToMainMenuBool = false;
        }
    }
    void GoToMainMenuClicked()
    {
        BackPanel.gameObject.SetActive(true);
    }
}
