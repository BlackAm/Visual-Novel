using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        #region <consts>

        public const int MAX_LIKING_VALUE = 100;
        public const int MIN_LIKING_VALUE = 0;

        #endregion
        
        
        public void OnCreatedLiking()
        {
            var enumerator = SystemTool.GetEnumEnumerator<Character>(SystemTool.GetEnumeratorType.GetAll);

            foreach (var character in enumerator)
            {
                currentDialogueEventData.Liking.Add(character, MIN_LIKING_VALUE);
            }
        }
    }
}