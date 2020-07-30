using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public string HeroName;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        HeroName = PlayerPrefs.GetString("Name");

        DontDestroyOnLoad(transform.gameObject);
    }
}
