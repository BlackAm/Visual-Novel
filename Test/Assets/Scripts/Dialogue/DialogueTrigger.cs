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
        dialogue.sentences[0] = "마이크 체크 원투";
        dialogue.sentences[1] = "잘 나오나 테스트중";
        dialogue.sentences[2] = GameManager.instance.HeroName + " 되면 끼요옷";
    }
}
