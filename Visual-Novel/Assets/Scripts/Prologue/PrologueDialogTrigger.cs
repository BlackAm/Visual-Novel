using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueDialogTrigger : MonoBehaviour
{
	public PrologueDialog prologueDialog;

    public string xmlFileName;

    private int dialogueCount = 0;

    public void TriggerDialogue()
    {
        FindObjectOfType<PrologueDialogManager>().StartDialogue(prologueDialog);
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

        // 하나씩 가져오기 테스트 예제.
        XmlNodeList cost_Table = xmlDoc.GetElementsByTagName("dialogue");
        
        foreach (XmlNode cost in cost_Table)
        {
            prologueDialog.sentences[dialogueCount] = cost.InnerText;
            dialogueCount++;
        }
        

        // 전체 아이템 가져오기 예제.
        /*XmlNodeList all_nodes = xmlDoc.SelectNodes("dataroot/TestDialogue");
        foreach (XmlNode node in all_nodes)
        {
            // 수량이 많으면 반복문 사용.
            Debug.Log("[at once] id :" + node.SelectSingleNode("id").InnerText);
            Debug.Log("[at once] dialogue : " + node.SelectSingleNode("dialogue").InnerText);
        }*/
    }
}
