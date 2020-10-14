using System.Collections;
using System.Collections.Generic;
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

    public Button choiceButton01;
    public Button choiceButton02;
    public Button choiceButton03;
    public Text choiceButtonText01;
    public Text choiceButtonText02;
    public Text choiceButtonText03;

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
            if(node.SelectSingleNode("name").InnerText == "")
            {
                dialogue.names[dialogueCount] = PlayerPrefs.GetString("Name");
            }
            else
            {
                dialogue.names[dialogueCount] = node.SelectSingleNode("name").InnerText;
            }
            
            dialogue.sentences[dialogueCount] = node.SelectSingleNode("dialogue").InnerText;
            dialogueCount++;
        }

        foreach (XmlNode node in all_nodes)
        {
            if(node.SelectNodes("question").Count > 0)
            {
                for(int i = 0; i < 3; i++)
                {
                    dialogue.quiestions[i] = node.SelectNodes("question")[i].InnerText;
                }
            }
        }
    }


    private void SetQuestions()
    {
        if(dialogue.quiestions[2] == "")
        {
            choiceButton03.gameObject.SetActive(false);
        }
        else
        {
            choiceButtonText03.text = dialogue.quiestions[2];
        }

        if(dialogue.quiestions[1] == "")
        {
            choiceButton02.gameObject.SetActive(false);
        }
        else
        {
            choiceButtonText02.text = dialogue.quiestions[1];
        }

        if (dialogue.quiestions[0] == "")
        {
            choiceButton01.gameObject.SetActive(false);
        }
        else
        {
            choiceButtonText01.text = dialogue.quiestions[0];
        }
    }

    public void EndDialogue()
    {
        Debug.Log("Trigger EndDialogue Entered");
        choicePanel.gameObject.SetActive(true);
    }
}
