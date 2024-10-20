using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SystemApkVersionCheck : MonoBehaviour
{

    public GameObject _SystemBoot;
    private void Awake()
    {

        if(Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(CheckVersion());
        }
        else
        {
            _SystemBoot.SetActive(true);
        }
    }

    IEnumerator CheckVersion()
    {
        var www = UnityWebRequest.Get("http://127.0.0.1/ApkVersion.txt");

        yield return www.SendWebRequest();
        if(www.isNetworkError || www.isHttpError)
        {

        }
        else
        {
            if(float.Parse(www.downloadHandler.text) > float.Parse(Application.version))
            {
                ShowDialog();
            }
            else
            {
                _SystemBoot.SetActive(true);
            }
        }

    }

    void ShowDialog()
    {
        Application.OpenURL("http://127.0.0.1/");
        Application.Quit();
    }

}
