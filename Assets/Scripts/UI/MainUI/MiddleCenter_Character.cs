using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class MainUI
    {
        private CharacterImagePoolingManager characterImagePoolingManager;
        private List<CharacterImage> characterImageList;
        
        public Dictionary<Character, CharacterImage> characterImageCollection;

        public void Initialize_MiddleCenter_CharacterImage()
        {
            var basePath = "MiddleCenter_Character";

            characterImageList = new List<CharacterImage>();

            characterImagePoolingManager = (CharacterImagePoolingManager) AddComponent<CharacterImagePoolingManager>($"{basePath}/CharacterImagePoolingManager").Initialize();

            var characterEnumerator = SystemTool.GetEnumEnumerator<Character>(SystemTool.GetEnumeratorType.GetAll);
            characterImageCollection = new Dictionary<Character, CharacterImage>();
            
            foreach (var character in characterEnumerator)
            {
                var characterImage = characterImagePoolingManager.GetObject();
                characterImage.transform.localPosition = Vector3.zero;
                characterImagePoolingManager.PoolObject(characterImage);
                
                characterImageCollection.Add(character, default);
            }
        }

        private void Update_MiddleCenter_CharacterImage(float p_DeltaTime)
        {
            foreach (var characterImage in characterImageCollection)
            {
                if (!ReferenceEquals(null, characterImage.Value))
                {
                    characterImage.Value.OnUpdateUI(p_DeltaTime);
                }
            }
        }

        public void ChangeCharacterImage(int p_Key)
        {
            var changeCharacterImage = ChangeCharacterImagePresetData.GetInstanceUnSafe[p_Key];
            var characterImage = ReferenceEquals(null, characterImageCollection[changeCharacterImage.Character])
                ? characterImagePoolingManager.GetObject()
                : characterImageCollection[changeCharacterImage.Character];

            var actionFade = ReferenceEquals(null, characterImageCollection[changeCharacterImage.Character]) && changeCharacterImage.ActionFade;
            
            characterImage.SetCharacterImage(changeCharacterImage.Character, changeCharacterImage.ImagePosition,
                changeCharacterImage.ImageKey, changeCharacterImage.ImageScale, actionFade,
                changeCharacterImage.FadeTuple);

            UpdateCharacterImage(changeCharacterImage.Character, characterImage);
            
            SetDialogueEventEnd(true);
        }

        public void ChangeCharacterImage(Character p_Character, SaveLoadManager.CharacterImageSaveData pCharacterImageSaveData)
        {
            var characterImage = ReferenceEquals(null, characterImageCollection[p_Character])
                ? characterImagePoolingManager.GetObject()
                : characterImageCollection[p_Character];
            
            characterImage.SetCharacterImage(p_Character, pCharacterImageSaveData.Position, 
                pCharacterImageSaveData.ImageKey, pCharacterImageSaveData.Scale,
                false, pCharacterImageSaveData.FadeTuple);

            UpdateCharacterImage(p_Character, characterImage);
        }

        public void UpdateCharacterImage(Character p_Character, CharacterImage p_CharacterImage)
        {
            if (!characterImageCollection.ContainsKey(p_Character))
            {
                characterImageCollection.Add(p_Character, default);
            }
            
            characterImageCollection[p_Character] = p_CharacterImage;
            characterImageList.Add(p_CharacterImage);
        }

        public async UniTask ClearCharacterImage()
        {
            while (characterImageList.Count > 0)
            {
                characterImagePoolingManager.PoolObject(characterImageList[characterImageList.Count - 1]);
                characterImageList.Remove(characterImageList[characterImageList.Count - 1]);
            }

            await Task.CompletedTask;
        }

        public void ResizeCharacterImage(int p_Key)
        {
            var resizeCharacterImage = ResizeCharacterImagePresetData.GetInstanceUnSafe[p_Key];

            if (!ReferenceEquals(null, characterImageCollection[resizeCharacterImage.Character]))
            {
                characterImageCollection[resizeCharacterImage.Character].InitializeAffine(resizeCharacterImage.Scale);
            }

            DialogueGameManager.GetInstance.currentDialogueEventData.CharacterImageSave[resizeCharacterImage.Character].Scale = resizeCharacterImage.Scale;
            SetDialogueEventEnd(true);
        }

        public void RemoveCharacterImage(int p_Key)
        {
            var character = RemoveCharacterImagePresetData.GetInstanceUnSafe[p_Key].Character;

            if (!ReferenceEquals(null, characterImageCollection[character]))
            {
                characterImageCollection[character].RetrieveObject();
                characterImageCollection[character] = default;
            }

            DialogueGameManager.GetInstance.currentDialogueEventData.CharacterImageSave[character] = default;
        }

        
    }
}
