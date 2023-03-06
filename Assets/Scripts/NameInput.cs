using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

public class NameInput : MonoBehaviour
{
    public InputField inputName; // 사용자에게서 입력받은 이름을 표시하는 공간
    public Text PlayerNameCheck; // 이름 입력시 나타날 텍스트
    public GameObject Panel;    // 이름 입력시 나타나는 패널

    bool PanelOn = false;   // 패널의 유무        

    public Button OkButton; // 이름 설정 완료후 OK버튼
    public Button CancelButton; // 이름 설정을 취소하는 버튼   
    string AllocatedName; // 설정된 이름을 저장하는 변수
    public void EnterName(InputField ip) // InputField
    {                
        if (Input.GetKey(KeyCode.Return) && PanelOn == false) // 이름 입력 확인시 확인 요소 확인
        {
            if (inputName.text.Length == 0) // 문장열의 공백여부 확인
            {
                PlayerNameCheck.text = "길이가 0 입니다";
                PanelButtonSetActiveFalse();
            }
            else
            {                
                string idChecker = Regex.Replace(inputName.text, @"[ ^0-9a-zA-Z가-힣あ-ろ ]{1,10}", "", RegexOptions.Singleline); // 이름 생성시 특수문자, 공백여부, 낱자 확인
                inputName.text.Equals(idChecker);
                bool FindBlank = inputName.text.Contains(" ");
                if (idChecker != "")                
                    NameCheckMiss();                
                else if(FindBlank)                
                    NameCheckMiss();                
                else
                {                    
                    PanelButtonSetActiveTrue();
                    string name = "주인공의 이름은 " + inputName.text + " 입니까?";
                    PlayerNameCheck.text = name;
                }
            }
            PanelSetActiveTrue();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && PanelOn == true) //빠른 판넬 닫기
            PanelSetActiveFalse();
    }

    void NameCheckMiss() // 이름에 낱자, 특수문자, 띄어쓰기가 있을시 오류메세지 출력
    {
        PlayerNameCheck.text = "이름에 낱자, 특수문자, 띄어쓰기를 넣을 수 없습니다.";
        Invoke("PanelSetActiveFalse", 3);
        PanelButtonSetActiveFalse();
    }

    void PanelButtonSetActiveFalse() // 판넬 ON
    {
        OkButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
    }

    void PanelButtonSetActiveTrue() // 버튼 ON
    {
        OkButton.gameObject.SetActive(true);
        CancelButton.gameObject.SetActive(true);
    }
    void PanelSetActiveTrue() // 판넬 OFF
    {
        Panel.gameObject.SetActive(true);
        PanelOn = true;
    }

    void PanelSetActiveFalse() // 버튼 OFF
    {
        Panel.gameObject.SetActive(false);
        PanelOn = false;
    }
    void Awake()
    {        
        OkButton.onClick.AddListener(OKButtonClicked);
        CancelButton.onClick.AddListener(CancelButtonClicked);
        PlayerPrefs.SetInt("Prologue", 0);
        PlayerPrefs.SetInt("Dialog1", 0);
    }


    void OKButtonClicked() // 이름 OK 버튼
    {
        // GameManager.instance.HeroName = inputName.text;
        PlayerPrefs.SetString("Name", inputName.text); // InputField에서 받은 값을 데이터로 저장     
        SceneController.instance.LoadSceneClicked();
    }

    void CancelButtonClicked() // 이름 Cancel 버튼
    {        
        Panel.gameObject.SetActive(false);
    }
}