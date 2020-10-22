using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public DialogueManager dialogueManager;

    public string storyXml;

    private int dialogueCount = 0;

    public GameObject choicePanel;

    public Button[] choiceButton = new Button[3];
    public Text[] choiceText = new Text[3];

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
        dialogueManager = GetComponent<DialogueManager>();

        LoadXML(storyXml);
        SetQuestions();
    }

    private void LoadXML(string _storyFile)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/TestDialogue");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtAsset.text);

        // 전체 아이템 가져오기 예제.
        XmlNodeList all_nodes = xmlDoc.SelectNodes("dataroot/" + _storyFile);
        foreach (XmlNode node in all_nodes)
        {
            int charName = (node.SelectSingleNode("name").InnerText) == "" ? SetPlayerName() : SetCharacterName(node);

            dialogue.sentences[dialogueCount] = node.SelectSingleNode("dialogue").InnerText;
            dialogueCount++;
        }

        foreach (XmlNode node in all_nodes)
        {
            if (node.SelectNodes("question").Count > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    dialogue.quiestions[i] = node.SelectNodes("question")[i].InnerText;
                }
            }
        }
    }


    private void SetQuestions()
    {
        Debug.Log(dialogue.quiestions.Length);
        for(int i = 2; i >= 0; i--)
        {
            int dialogueCount = (dialogue.quiestions[i] == "") ? ChoicebuttonFalse(choiceButton[2 - i]) : ChoicebuttonTrue(choiceText[2 - i], i);
        }
    }

    public void EndDialogue()
    {
        choicePanel.gameObject.SetActive(true);
    }

    private int SetPlayerName()
    {
        dialogue.names[dialogueCount] = PlayerPrefs.GetString("Name");

        return 0;
    }

    private int SetCharacterName(XmlNode node)
    {
        dialogue.names[dialogueCount] = node.SelectSingleNode("name").InnerText;

        return 0;
    }

    private int ChoicebuttonFalse(Button choiceButton)
    {
        choiceButton.gameObject.SetActive(false);

        return 0;
    }

    private int ChoicebuttonTrue(Text choiceText, int index)
    {
        choiceText.text = dialogue.quiestions[index];

        return 0;
    }
}
