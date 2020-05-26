using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueDialogTrigger : MonoBehaviour
{
	public PrologueDialog prologueDialog;

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
		prologueDialog.sentences[0] = "1234567890";
		prologueDialog.sentences[1] = "abcdefghijklm";
		prologueDialog.sentences[2] = "ㄱㄴㄷㄻㅄㅇㅈㅊ";
    }
}
