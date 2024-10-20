using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class MainUI
    {
        public Text DialogueText;
        public Text NameText;

        private void Initialize_BottomCenter_Dialogue()
        {
            var basePath = "BottomCenter_Dialogue";

            DialogueText = GetComponent<Text>($"{basePath}/DialoguePanel/DialogueText");
            NameText = GetComponent<Text>($"{basePath}/NamePanel/NameText");
        }

        public void ResetDialogueText()
        {
            DialogueText.text = string.Empty;
        }

        public void UpdateDialogueText(string p_Text)
        {
            DialogueText.text = p_Text;
        }

        public void UpdateDialogueText(char p_Text)
        {
            DialogueText.text += p_Text;
        }

        public void UpdateNameText(string p_Text)
        {
            NameText.text = p_Text;
        }
    }
}