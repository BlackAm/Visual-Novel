using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        
        #region <Enum>

        public enum DialogueEventCharacterImage
        {
            None = 0,
            ChangeCharacterImage,       // 캐릭터 이미지 변경
            ResizeCharacterImage,       // 캐릭터 이미지 크기 변경
            HighLightCharacterImage,    // 캐릭터 이미지 하이라이트
            FadeInCharacterImage,       // 캐릭터 이미지 페이드 인
            FadeOutCharacterImage,      // 캐릭터 이미지 페이드 아웃
            RemoveCharacterImage,       // 캐릭터 이미지 제거
            MoveCharacterImage,         // 캐릭터 이미지 위치 변경
        }

        #endregion

        #region <Callbacks>

        public void OnCreatedCharacterImage()
        {
            var characterEnumerator = SystemTool.GetEnumEnumerator<Character>(SystemTool.GetEnumeratorType.GetAll);
            currentDialogueEventData.CharacterImageSave = new Dictionary<Character, SaveLoadManager.CharacterImageSaveData>();

            foreach (var character in characterEnumerator)
            {
                currentDialogueEventData.CharacterImageSave.Add(character, default);
            }
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
                    eventValue = ChangeCharacterImage(p_Key);
                    break;
                case DialogueEventCharacterImage.ResizeCharacterImage:
                    eventValue = ResizeCharacterImage(p_Key);
                    break;
                case DialogueEventCharacterImage.HighLightCharacterImage:
                    break;
                case DialogueEventCharacterImage.FadeInCharacterImage:
                    break;
                case DialogueEventCharacterImage.FadeOutCharacterImage:
                    break;
                case DialogueEventCharacterImage.RemoveCharacterImage:
                    break;
                default:
                    break;
            }

            return eventValue;
        }

        public int ChangeCharacterImage(int p_Key)
        {
            MainGameUI.Instance.mainUI.ChangeCharacterImage(p_Key);
            
            return p_Key;
        }

        public int RemoveCharacterImage(int p_Key)
        {
            MainGameUI.Instance.mainUI.RemoveCharacterImage(p_Key);

            return p_Key;
        }

        public int ResizeCharacterImage(int p_Key)
        {
            MainGameUI.Instance.mainUI.ResizeCharacterImage(p_Key);
            
            return p_Key;
        }

        #endregion
    }

}
