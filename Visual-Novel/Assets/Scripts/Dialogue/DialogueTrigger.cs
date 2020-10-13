using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public string xmlFileName;

    private int dialogueCount = 0;

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
        LoadXML(xmlFileName);
    }

    private void LoadXML(string _fileName)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/" + _fileName);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtAsset.text);

        // �ϳ��� �������� �׽�Ʈ ����.
        /* XmlNodeList cost_Table = xmlDoc.GetElementsByTagName("dialogue");

        foreach (XmlNode cost in cost_Table)
        {
            dialogue.sentences[dialogueCount] = cost.InnerText;
            dialogueCount++;
        } */


        // ��ü ������ �������� ����.
        XmlNodeList all_nodes = xmlDoc.SelectNodes("dataroot/TestName");
        foreach (XmlNode node in all_nodes)
        {
            dialogue.names[dialogueCount] = node.SelectSingleNode("name").InnerText;
            dialogue.sentences[dialogueCount] = node.SelectSingleNode("dialogue").InnerText;
            dialogueCount++;
        }
    }
}
