using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        #region <Const>

        public const int DefaultBackGroundImageKey = 1000;
        public const float DefaultBackGroundImageSize = 1f;

        #endregion

        #region <Enum>

        public enum DialogueEventBackGroundImage
        {
            ChangeBackGroundImage,       // 배경 이미지 변경
            ResizeBackGroundImage,      // 배경 이미지 크기 변경
            MoveBackGroundImageLerp,    // 배경 이미지 이동
        }

        #endregion

        #region <Callbacks>

        public void OnInitiateBackGroundImage()
        {
            currentDialogueEventData.BackGroundImageSave.ImageKey = DefaultBackGroundImageKey;
            currentDialogueEventData.BackGroundImageSave.Scale = DefaultBackGroundImageSize;
        }

        #endregion

        #region <Method>

        public int ActionBackGroundImage(int p_Key)
        {
            var BackGroundImageEvent = DialogueBackGroundImagePresetData.GetInstanceUnSafe[p_Key].BackGroundImageEvent;

            int eventValue = 0;
            foreach (var backGroundImageEvent in BackGroundImageEvent)
            {
                eventValue = ActionBackGroundImage(backGroundImageEvent.Key, backGroundImageEvent.Value);

                if (eventValue < 0) return eventValue;
            }

            return eventValue;
        }

        public int ActionBackGroundImage(DialogueEventBackGroundImage p_BackGroundImageEvent, int p_Key)
        {
            int eventValue = 0;
            
            switch (p_BackGroundImageEvent)
            {
                case DialogueEventBackGroundImage.ChangeBackGroundImage:
                    eventValue = ChangeBackGroundImage(p_Key);
                    break;
                case DialogueEventBackGroundImage.ResizeBackGroundImage:
                    eventValue = ResizeBackGroundImage(p_Key);
                    break;
                case DialogueEventBackGroundImage.MoveBackGroundImageLerp:
                    eventValue = MoveBackGroundImageLerp(p_Key);
                    break;
                default:
                    eventValue = -99;
                    break;
            }

            return eventValue;
        }

        public int ChangeBackGroundImage(int p_Key)
        {
            var backGroundImageKey = ChangeBackGroundImagePresetData.GetInstanceUnSafe[p_Key].ImageKey;
            MainGameUI.Instance.mainUI.ChangeImage(ImageType.BG, backGroundImageKey);

            return backGroundImageKey;
        }

        public int ResizeBackGroundImage(int p_Key)
        {
            var size = ResizeBackGroundImagePresetData.GetInstanceUnSafe[p_Key].Size;
            MainGameUI.Instance.mainUI.ResizeImage(ImageType.BG, size);
            return p_Key;
        }

        public int MoveBackGroundImageLerp(int p_Key)
        {
            if (MoveBackGroundImageLerpPresetData.GetInstanceUnSafe.HasKey(p_Key))
            {
                MainGameUI.Instance.mainUI.MoveBackGroundImageLerp(p_Key);
                return p_Key;
            }
            else
            {
                return -p_Key;
            }
        }

        #endregion

    }
}