using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	#region <Consts>

	float fades = 0.0f;
	float time = 0;

	#endregion
	
	#region <Fields>

	public Text nameText;
	public Text dialogueText;
	public Image imageBox;
	public Image NameScreen;

	private Queue<string> names;
	private Queue<string> sentences;
	private Queue<Sprite> sprites;

	private Sprite CashedImage;

	public DialogueTrigger dialogueTrigger;

	#endregion

	#region <Callbacks>

	private void Awake () {
		names = new Queue<string>();
		sentences = new Queue<string>();
		sprites = new Queue<Sprite>();

		dialogueTrigger = GetComponent<DialogueTrigger>();
	}
	
	// Update문 방식 바꾸기
	private void Update()
	{
		if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Jump"))
		{
			if (sentences.Count == 0)
			{
				EndDialogue();
			}
			else
			{
				DisplayNextSentence();
			}
		}
	}

	#endregion

	#region <Methods>

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

	private void DisplayNextSentence ()
	{
		SetUI();

		var sentence = sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	private IEnumerator TypeSentence (string sentence)
	{
		dialogueText.text = "";
		foreach (var letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}
	
	IEnumerator FadeIn()
	{
		time += Time.deltaTime;
		if (fades < 1.0f && time >= 0.1f)
		{
			fades += 0.1f;
			imageBox.color = new Color(255, 255, 255, fades);
			time = 0;
		}
		else if (fades == 1.0f)
		{
			time = 0;
			yield return null;
		}
	}

	private void EndDialogue()
	{
		dialogueTrigger.EndDialogue();
	}

	private void SetUI()
	{
		if (!Equals(null, imageBox.sprite))
		{
			CashedImage = imageBox.sprite;
		}

		nameText.text = names.Dequeue();
		imageBox.sprite = sprites.Dequeue();

		if (!Equals(imageBox.sprite, CashedImage))
		{
			fades = 0;
			// FadeIn 코루틴이 발생하지 않는다 Fade 수정하기
			/* imageBox.color = new Color(255, 255, 255, 0); */
			StartCoroutine(FadeIn());
		}

		SetNameText(!Equals(string.Empty, nameText.text));
	}

	private void SetNameText(bool p_Bool)
	{
		NameScreen.gameObject.SetActive(p_Bool);
	}

	#endregion
	
	
}
