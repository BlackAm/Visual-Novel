using System.Xml;
using UnityEngine;

public class PrologueTrigger : MonoBehaviour
{
    public Prologue prologue;

    public string xmlFileName;

    private int dialogueCount = 0;

    private void TriggerDialogue()
    {
        FindObjectOfType<PrologueManager>().StartDialogue(prologue);
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
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/TestDialogue");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtAsset.text);

        // 전체 아이템 가져오기 예제.
        XmlNodeList all_nodes = xmlDoc.SelectNodes("dataroot/" + _fileName);

        foreach (XmlNode node in all_nodes)
        {
            prologue.sentences[dialogueCount] = node.SelectSingleNode("dialogue").InnerText;
            dialogueCount++;
        }
    }
}