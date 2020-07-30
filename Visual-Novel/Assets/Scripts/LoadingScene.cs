using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class LoadingScene : MonoBehaviour
{
    public Image progressBar;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation oper = SceneManager.LoadSceneAsync("");

        if (PlayerPrefs.GetInt("Prologue") == 1)
        {
            for (int i = 1; i < 9; i++)
            {
                if (PlayerPrefs.GetInt("Dialog" + i) != 1)
                {
                    oper = SceneManager.LoadSceneAsync("Dialog_" + i + "_Scene");
                    break;
                }
            }
        }
        else
        {
            oper = SceneManager.LoadSceneAsync("PrologueScene");
        }

        oper.allowSceneActivation = false;

        float timer = 0.0f;
        while (!oper.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (oper.progress >= 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount == 1.0f)
                    oper.allowSceneActivation = true;
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, oper.progress, timer);
                if (progressBar.fillAmount >= oper.progress)
                {
                    timer = 0f;
                }
            }
        }
    }
}