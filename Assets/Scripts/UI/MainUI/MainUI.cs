#if !SERVER_DRIVE

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class MainUI : TouchEventSenderCluster
    {
        public MenuPanel menuPanel;
        // public DialogueTouchPanel dialogueTouchPanel;

        public GameObject topCenter, topLeft, middleLeft, bottomLeft, topRight, middleRight, bottomRight;

        // public Transform _ControlStickBase;

        public GameObject overlay,
            middleCenter,
            bottomCenter,
            other;

        // public GameObject eventCG;

        public Image eventCG;

        public enum UIList
        {
            Overlay,
            TopMiddle,
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight,
            Other,
            EventCG,
        }
        
        public override void OnSpawning()
        {
            base.OnSpawning();
            
            menuPanel = _Transform.Find("Overlay/MenuPanel").gameObject.AddComponent<MenuPanel>();
            eventCG = GetComponent<Image>("EventCG");

            Initialize_BottomCenter_Dialogue();
            Initialize_TopMiddle_SelectDialogue();
            Initialize_MiddleCenter_CharacterImage();
            Initialize_Fader();
            Initialize_BackGroundImage();
            Initialize_DialogueTouchPanel();
            
            // 메뉴
            overlay = Find("Overlay").gameObject;
            bottomCenter = Find("BottomCenter_Dialogue").gameObject;
            middleCenter = Find("MiddleCenter_Character").gameObject;
            topCenter = Find("TopCenter_SelectDialogue").gameObject;

            ActiveMainUI(UIList.Overlay, true);
            ActiveMainUI(UIList.TopCenter, false);
        }
        
        /// momo6346 - 활성화 시 1회성으로 서버에 현재 캐릭터 정보를 요청합니다.(serverFlag 초깃값을 true로...)
        /// 현재 레벨업을 위해 CHARACTER_INFO_REQUEST(600)을 보냅니다.
        private void OnEnable()
        {
            
        }

        protected override void DisposeUnManaged()
        {
            base.DisposeUnManaged();
        }

        public override void OnSceneTransition()
        {
            base.OnSceneTransition();
        }

        public void SetActiveMenu(bool active)
        {
            topLeft.SetActive(active);
            middleLeft.SetActive(active);
            bottomLeft.SetActive(active);
            topRight.SetActive(active);
            middleRight.SetActive(active);
            bottomRight.SetActive(active);
            bottomCenter.SetActive(active);
        }
        
        /// 기본 UI 출력.
        public void SetActiveUI(bool active)
        {
            overlay.SetActive(active);
            topLeft.SetActive(active);
            topRight.SetActive(active);
            middleLeft.SetActive(active);
            middleCenter.SetActive(active);
            middleRight.SetActive(active);
            bottomLeft.SetActive(active);
            bottomCenter.SetActive(active);
            bottomRight.SetActive(active);
            other.SetActive(active);
        }
        
        /// 기본적으로 숨겨져있는 UI.
        public void DefaultHide()
        {
            //chat.SetActive(false);
        }

        /// 해당 UI 활성화 여부 결정.
        public void ActiveMainUI(UIList item, bool active)
        {
            switch (item)
            {
                case UIList.Overlay:
                    overlay.SetActive(active);
                    break;
                case UIList.TopLeft:
                    topLeft.SetActive(active);
                    break;
                case UIList.TopCenter:
                    topCenter.SetActive(active);
                    break;
                case UIList.TopRight:
                    topRight.SetActive(active);
                    break;
                case UIList.MiddleLeft:
                    middleLeft.SetActive(active);
                    break;
                case UIList.MiddleCenter:
                    middleCenter.SetActive(active);
                    break;
                case UIList.MiddleRight:
                    middleRight.SetActive(active);
                    break;
                case UIList.BottomLeft:
                    bottomLeft.SetActive(active);
                    break;
                case UIList.BottomCenter:
                    bottomCenter.SetActive(active);
                    break;
                case UIList.BottomRight:
                    bottomRight.SetActive(active);
                    break;
                case UIList.Other:
                    other.SetActive(active);
                    break;
                case UIList.EventCG:
                    eventCG.gameObject.SetActive(active);
                    break;
            }
        }

        public bool IsUIActive(UIList p_Item)
        {
            switch (p_Item)
            {
                case UIList.Overlay:
                    return overlay.activeSelf;
                case UIList.TopLeft:
                    return topLeft.activeSelf;
                case UIList.TopCenter:
                    return topCenter.activeSelf;
                case UIList.TopRight:
                    return topRight.activeSelf;
                case UIList.MiddleLeft:
                    return middleLeft.activeSelf;
                case UIList.MiddleCenter:
                    return middleCenter.activeSelf;
                case UIList.MiddleRight:
                    return middleRight.activeSelf;
                case UIList.BottomLeft:
                    return bottomLeft.activeSelf;
                case UIList.BottomCenter:
                    return bottomCenter.activeSelf;
                case UIList.BottomRight:
                    return bottomRight.activeSelf;
                case UIList.Other:
                    return other.activeSelf;
                case UIList.EventCG:
                    return eventCG.gameObject.activeSelf;
            }

            return false;
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
            Update_MiddleCenter_CharacterImage(p_DeltaTime);
            UpdateFader(p_DeltaTime);
        }
        
        protected Transform Find(string path)
        {
            return transform.Find(path);
        }
        
        protected T GetComponent<T>(string path)
        {
            return transform.Find(path).GetComponent<T>();
        }
        
        protected T AddComponent<T>(string path) where T : Component
        {
            return Find(path).gameObject.AddComponent<T>();
        }
        
        public void GetSprite(int p_Index, Image p_Image)
        {
            var loadResult = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(
                ResourceType.Image, ResourceLifeCycleType.Scene,
                ImageNameTableData.GetInstanceUnSafe.GetTableData(p_Index).ResourceName);
            
            p_Image.sprite = loadResult.Item2;
        }

        public void SetDialogueEventEnd(bool p_Flag)
        {
            DialogueGameManager.GetInstance.SetDialogueEventEnd(p_Flag);
        }

        public Image GetImage(ImageType p_ImageType)
        {
            switch (p_ImageType)
            {
                case ImageType.BG:
                    return BackgroundImage;
                case ImageType.EventCG:
                    return eventCG;
            }

            return default;
        }

        public void ChangeImage(ImageType p_ImageType, int p_Key)
        {
            var targetImage = GetImage(p_ImageType);
            switch (p_ImageType)
            {
                case ImageType.BG:
                    DialogueGameManager.GetInstance.currentDialogueEventData.BackGroundImageSave.ImageKey = p_Key;
                    break;
                case ImageType.EventCG:
                    DialogueGameManager.GetInstance.currentDialogueEventData.EventCGSave.ImageKey = p_Key;
                    break;
            }
            
            ChangeImage(targetImage, p_Key);
            SetDialogueEventEnd(true);
        }

        public void ChangeImage(Image p_TargetImage, int p_Key)
        {
            GetSprite(p_Key, p_TargetImage);
        }

        public void ResizeImage(ImageType p_ImageType, float p_Size)
        {
            var targetImage = GetImage(p_ImageType);
            switch (p_ImageType)
            {
                case ImageType.BG:
                    DialogueGameManager.GetInstance.currentDialogueEventData.BackGroundImageSave.Scale = p_Size;
                    break;
                case ImageType.EventCG:
                    DialogueGameManager.GetInstance.currentDialogueEventData.EventCGSave.Scale = p_Size;
                    break;
            }
            
            targetImage.SetImageSize(p_Size);
            SetDialogueEventEnd(true);
        }

        public void SetImagePosition(ImageType p_ImageType, Vector2 p_OffsetPosition)
        {
            var targetImage = GetImage(p_ImageType);
            targetImage.rectTransform.offsetMin = targetImage.rectTransform.offsetMax = p_OffsetPosition;
        }
    }
}

#endif