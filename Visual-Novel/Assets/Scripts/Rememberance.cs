using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rememberance : MonoBehaviour
{
    public Button PrologueButton;
    public Button SeAButton;
    public Button ChorongButton;
    public GameObject ProloguePanel;
    public GameObject ChorongPanel;
    public GameObject SeAPanel;
    public Text Text;
    void Awake()
    {
        PrologueButton.onClick.AddListener(PrologueButtonClicked);
        SeAButton.onClick.AddListener(SeAButtonClicked);
        ChorongButton.onClick.AddListener(ChorongButtonClicked);
    }

    void PrologueButtonClicked()
    {
        ChorongPanel.gameObject.SetActive(false);
        SeAPanel.gameObject.SetActive(false);
        ProloguePanel.gameObject.SetActive(true);
        Text.text = "프롤로그";
    }
    void SeAButtonClicked()
    {
        ProloguePanel.gameObject.SetActive(false);
        ChorongPanel.gameObject.SetActive(false);
        SeAPanel.gameObject.SetActive(true);
        Text.text = "세아";
    }
    void ChorongButtonClicked()
    {       
        SeAPanel.gameObject.SetActive(false);
        ProloguePanel.gameObject.SetActive(false);
        ChorongPanel.gameObject.SetActive(true);
        Text.text = "전초롱";
    }
}