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

        public enum DialogueEventBackGroundImage
        {
            None = 0,
            ChangeBackGroundImage, // 배경 이미지 변경
        }

        #endregion

        #region <Method>

        public int ActionBackGroundImage(int p_Key)
        {
            var BackGroundImageEvent = DialogueBackGroundImagePresetData.GetInstanceUnSafe[p_Key].BackGroundImageEvent;

            int EventValue = 0;
            foreach (var backGroundImageEvent in BackGroundImageEvent)
            {
                EventValue = ActionBackGroundImage(backGroundImageEvent.Key, backGroundImageEvent.Value);

                if (EventValue < 0) return EventValue;
            }

            return EventValue;
        }

        public int ActionBackGroundImage(DialogueEventBackGroundImage p_BackGroundImageEvent, int p_Key)
        {
            int EventValue = 0;
            
            switch (p_BackGroundImageEvent)
            {
                case DialogueEventBackGroundImage.ChangeBackGroundImage:
                    EventValue = ChangeBackGroundImage(p_Key);
                    break;
                default:
                    EventValue = -99;
                    break;
            }

            return EventValue;
        }

        public int ChangeBackGroundImage(int p_Key)
        {
            var BackGroundImageKey = ChangeBackGroundImagePresetData.GetInstanceUnSafe[p_Key].ImageKey;
            MainGameUI.Instance.functionUI.ChangeImage(ImageType.BG, BackGroundImageKey);

            return BackGroundImageKey;
        }

        #endregion

    }
}