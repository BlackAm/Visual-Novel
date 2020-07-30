using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAllPrefs : MonoBehaviour
{
    public void DeleteAllPrefsButton()
    {
        PlayerPrefs.DeleteAll();
    }
}
