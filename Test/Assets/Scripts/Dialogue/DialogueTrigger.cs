using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    public Dialogue dialogue;

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
        dialogue.sentences[0] = "����ũ üũ ����";
        dialogue.sentences[1] = "�� ������ �׽�Ʈ��";
        dialogue.sentences[2] = GameManager.instance.HeroName + " �Ǹ� �����";
    }
}
