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
    public string questionXml;

    private int dialogueCount = 0;
    private int questionCount = 0;

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

        LoadXML(storyXml, questionXml);
        SetQuestions();
    }

    private void LoadXML(string _storyFile, string _questionFile)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/TestDialogue");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtAsset.text);


        // 전체 아이템 가져오기 예제.
        XmlNodeList all_nodes = xmlDoc.SelectNodes("dataroot/" + _storyFile);
        foreach (XmlNode node in all_nodes)
        {
            dialogue.names[dialogueCount] = node.SelectSingleNode("name").InnerText;
            dialogue.sentences[dialogueCount] = node.SelectSingleNode("dialogue").InnerText;
            dialogueCount++;
        }

        all_nodes = xmlDoc.SelectNodes("dataroot/" + _questionFile);
        foreach (XmlNode node in all_nodes)
        {
            Debug.Log(node.SelectSingleNode("question").InnerText);
            dialogue.quiestions[questionCount] = node.SelectSingleNode("question").InnerText;
            questionCount++;
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
