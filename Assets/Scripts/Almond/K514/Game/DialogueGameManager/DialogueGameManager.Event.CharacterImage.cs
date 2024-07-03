using System;
using System.Collections.Generic;
using BDG;
using k514;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    public partial class DialogueGameManager
    {
        #region <Enum>

        public enum DialogueEventCharacterImage
        {
            None = 0,
            ChangeCharacterImage,       // 캐릭터 임이지 변경
            ExpandCharacterImage,       // 캐릭터 이미지 확대
            ReduceCharacterImage,       // 캐릭터 이미지 축소
            HighLightCharacterImage,    // 캐릭터 이미지 하이라이트
            FadeInCharacterImage,       // 캐릭터 이미지 페이드 인
            FadeOutCharacterImage,      // 캐릭터 이미지 페이드 아웃
        }

        #endregion

        #region <Method>

        public int ActionCharacterImage(int p_DialogueEvnetKey)
        {
            var CharacterImageEvent = CharacterImagePresetData.GetInstanceUnSafe[p_DialogueEvnetKey].CharacterImageEvent;

            int eventValue = 0;
            foreach (var characterImage in CharacterImageEvent)
            {
                eventValue = ActionCharacterImage(characterImage.Key, characterImage.Value);

                if (eventValue < 0) return eventValue;
            }

            return eventValue;
        }

        public int ActionCharacterImage(DialogueEventCharacterImage p_CharacterImageEvent, int p_Key)
        {
            int eventValue = 0;
            switch (p_CharacterImageEvent)
            {
                case DialogueEventCharacterImage.ChangeCharacterImage:
                    break;
                case DialogueEventCharacterImage.ExpandCharacterImage:
                    break;
                case DialogueEventCharacterImage.ReduceCharacterImage:
                    break;
                case DialogueEventCharacterImage.HighLightCharacterImage:
                    break;
                case DialogueEventCharacterImage.FadeInCharacterImage:
                    break;
                case DialogueEventCharacterImage.FadeOutCharacterImage:
                    break;
                default:
                    break;
            }

            return eventValue;
        }

        #endregion
    }

}
