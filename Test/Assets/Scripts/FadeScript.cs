using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public Image fade;
    float fades = 0.0f;
    float time = 0;

    private void Update()
    {
        StartCoroutine(FadeIn());

        if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Jump"))
        {
            fades = 0;
        }

        
    }
    IEnumerator FadeIn()
    {

        time += Time.deltaTime;
        if (fades < 1.0f && time >= 0.1f)
        {
            fades += 0.1f;
            fade.color = new Color(255, 255, 255, fades);
            time = 0;
        }
        else if (fades == 1.0f)
        {
            time = 0;
            yield return null;
        }
    }
}
