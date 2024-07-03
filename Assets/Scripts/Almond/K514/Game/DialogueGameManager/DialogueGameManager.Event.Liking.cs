using System.Collections;
using System.Collections.Generic;
using k514;
using UnityEngine;

namespace UI2020
{
    public partial class DialogueGameManager
    {
        #region <consts>

        public const int MAX_LIKING_VALUE = 100;
        public const int MIN_LIKING_VALUE = 0;

        #endregion
        
        public Dictionary<Character, int> CharacterLiking;
        
        public void OnCreatedLiking()
        {
            CharacterLiking = new Dictionary<Character, int>();
            
            var enumerator = SystemTool.GetEnumEnumerator<Character>(SystemTool.GetEnumeratorType.GetAll);

            foreach (var character in enumerator)
            {
                if (!PlayerPrefs.HasKey(character.ToString()))
                {
                    PlayerPrefs.SetInt(character.ToString(), MIN_LIKING_VALUE);
                }
                
                CharacterLiking.Add(character, PlayerPrefs.GetInt(character.ToString()));
            }
        }
    }
}