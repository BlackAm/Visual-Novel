using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private void Awake()
    {
        instance = this;
    }

    public void StartGameButtonClicked()
    {
        Invoke("GotoNameInput", .3f);
    }
    private void GotoNameInput()
    {
        SceneManager.LoadScene("NameInputScene");
    }

    public void RememberenceButtonClicked()
    {
        Invoke("GotoRememberence", .3f);
    }
    private void GotoRememberence()
    {
        SceneManager.LoadScene("Rememberance");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadSceneClicked()
    {
        Invoke("GotoLoadScene", .3f);
    }
    private void GotoLoadScene()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void MainMenuButtonClicked()
    {
        Invoke("GoToMainMenu", .3f);
    }
    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
}
