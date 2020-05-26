using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PrologueDialogManager : MonoBehaviour
{
	public Text dialogueText;


	private Queue<string> sentences;

	// Use this for initialization
	void Awake()
	{
		sentences = new Queue<string>();
	}

	public void StartDialogue(PrologueDialog prologueDialog)
	{
		sentences.Clear();

		foreach (string sentence in prologueDialog.sentences)
		{
			sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}

		string sentence = sentences.Dequeue();
		StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

	IEnumerator TypeSentence(string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return new WaitForSeconds(.1f);
		}
	}

	void EndDialogue()
	{
		PlayerPrefs.SetInt("Prologue", 1);
		SceneManager.LoadScene("Dialog_1_Scene");
	}

	private void Update()
	{
		if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Jump"))
			DisplayNextSentence();

	}
}
