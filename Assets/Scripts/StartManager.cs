using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    public Text StartText;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("Name") == "")
        {
            StartText.text = "시작하기";
        }
        else
        {
            StartText.text = "이어하기";
        }
    }

    public void StartButtonClicked()
    {
        if (PlayerPrefs.GetString("Name") == "")
        {
            SceneController.instance.StartGameButtonClicked();
        }
        else
        {
            SceneController.instance.LoadSceneClicked();
        }
    }
}
