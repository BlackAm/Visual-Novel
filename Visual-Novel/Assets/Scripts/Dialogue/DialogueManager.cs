using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	public Text nameText;
	public Text dialogueText;
	public Image imageBox;

	private Queue<string> names;
	private Queue<string> sentences;
	private Queue<Sprite> sprites;

    public DialogueTrigger dialogueTrigger;

	// Use this for initialization
	private void Awake () {
		names = new Queue<string>();
		sentences = new Queue<string>();
		sprites = new Queue<Sprite>();

        dialogueTrigger = GetComponent<DialogueTrigger>();
	}
	private void Update()
    {
		if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Jump"))
        {
			DisplayNextSentence();
		}
	}

	public void StartDialogue (Dialogue dialogue)
	{
		names.Clear();
		sentences.Clear();
		sprites.Clear();

		foreach(string name in dialogue.names)
        {
			names.Enqueue(name);
        }

		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}
		foreach(Sprite image in dialogue.sprites)
        {
			sprites.Enqueue(image);
        }

		DisplayNextSentence();
	}

	public void DisplayNextSentence ()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
		}

		nameText.text = names.Dequeue();
		imageBox.sprite = sprites.Dequeue();
		imageBox.color = new Color(255, 255, 255, 0);
		string sentence = sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	private IEnumerator TypeSentence (string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	private void EndDialogue()
	{
        dialogueTrigger.EndDialogue();
    }
}
