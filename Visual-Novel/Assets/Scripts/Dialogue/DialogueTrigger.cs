using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml;

public class DialogueTrigger : MonoBehaviour
{
    #region <Fiedls>

    public Dialogue dialogue;

    public string storyXml;

    private int _DialogueCount;
    private int _QuestionCount;

    public Transform ChoicePanel;

    public Button[] choiceButton;
    public Text[] choiceText;

    #endregion

    #region <Callbacks>

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    private void Start()
    {
        TriggerDialogue();
    }

    private void Awake()
    {
        storyXml = SceneManager.GetActiveScene().name;
        LoadXML(storyXml);
    }

    #endregion

    #region <Methods/Setter>

    private void SetQuestionArray()
    {
        choiceButton = new Button[_QuestionCount];
        choiceText = new Text[_QuestionCount];
        dialogue.quiestions = new string[_QuestionCount];

        var canvas = GameObject.Find($"Canvas");
        ChoicePanel = canvas.transform.Find("ChoicePanel");
        var buttons = ChoicePanel.gameObject.GetComponentsInChildren<Button>();
        var texts = ChoicePanel.gameObject.GetComponentsInChildren<Text>();
        var Count = 0;
        
        foreach (var button in buttons)
        {
            if (Count < _QuestionCount)
            {
                choiceButton[Count++] = button;
            }
        }
        Count = 0;
        foreach (var text in texts)
        {
            if (Count < _QuestionCount)
            {
                choiceText[Count++] = text;
            }
        }
    }
    
    private void SetQuestions()
    {
        for(var i = 0; i <= _QuestionCount -1; i++)
        {
            // 선택지 수정 필요
            SetChoiceButton(choiceText[_QuestionCount - 1 - i], i);
        }
    }
    private void SetName(XmlNode node)
    {
        var name = node.SelectSingleNode("Name")?.InnerText;
        dialogue.names[_DialogueCount] = Equals(name, "Player") ? GetPlayerName() : name;
    }

    #endregion

    #region <Methods/Getter>

    public string GetPlayerName()
    {
        return PlayerPrefs.GetString("Name");
    }

    #endregion
    
    #region <Methods/XML>

    private void LoadXML(string _storyFile)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/TestDialogue");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtAsset.text);
        
        XmlNodeList all_nodes = xmlDoc.SelectNodes("dataroot/" + _storyFile);
        dialogue.sentences = new string[all_nodes.Count];
        
        foreach (XmlNode node in all_nodes)
        {
            SetName(node);
            dialogue.sentences[_DialogueCount++] = node.SelectSingleNode("Dialogue").InnerText;
        }

        foreach (XmlNode node in all_nodes)
        {
            _QuestionCount = node.SelectNodes("Question").Count;
            if (_QuestionCount > 0)
            {
                SetQuestionArray();
                for (var i = 0; i < _QuestionCount; i++)
                {
                    var question = node.SelectNodes("Question")[i].InnerText;
                    dialogue.quiestions[i] = question;
                }
                SetQuestions();
            }
        }
    }

    #endregion

    #region <Methods/UI>
    
    public void EndDialogue()
    {
        ChoicePanel.gameObject.SetActive(true);
    }

    private void SetChoiceButton(Text P_ChoiceText, int p_Index)
    {
        P_ChoiceText.text = dialogue.quiestions[p_Index];
        Debug.LogWarning(dialogue.quiestions[p_Index]);
    }

    #endregion
}
