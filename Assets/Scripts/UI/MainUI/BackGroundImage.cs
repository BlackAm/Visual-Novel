using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class MainUI
    {
        public Image BackgroundImage;
        
        private SafeReference<object, GameEventTimerHandlerWrapper> _TimerEventHandler;
        private Vector2 OffsetEndPosition;
        
        public void Initialize_BackGroundImage()
        {
            BackgroundImage = GetComponent<Image>("BackGround");
        }

        public void MoveBackGroundImageLerp(int p_Key)
        {
            var moveBackgroundImage = MoveBackGroundImageLerpPresetData.GetInstanceUnSafe[p_Key];
            SetBackGroundImagePosition(moveBackgroundImage.StartPosition);
            SetBackGroundImageEndPosition(moveBackgroundImage.EndPosition);
            
            GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _TimerEventHandler, this,
                SystemBoot.TimerType.GameTimer, true);
            var (_, eventHandler) = _TimerEventHandler.GetValue();
            eventHandler.AddEvent(EventTimerTool.EventTimerIntervalType.UpdateEveryFrame, handler =>
            {
                handler.Arg3.SetBackGroundImagePosition(Vector2.MoveTowards(handler.Arg1.rectTransform.offsetMin,
                    handler.Arg2.EndPosition, handler.Arg2.Speed));
                return !handler.Arg3.CheckMoveBackGroundImageOver();
            }, null, BackgroundImage, moveBackgroundImage, this);
            eventHandler.StartEvent();
        }

        public void SetBackGroundImagePosition(Vector2 p_OffsetPosition)
        {
            BackgroundImage.rectTransform.offsetMin = BackgroundImage.rectTransform.offsetMax = p_OffsetPosition;
        }

        public void SetBackGroundImageEndPosition(Vector2 p_OffsetEndPosition)
        {
            OffsetEndPosition = p_OffsetEndPosition;
        }

        public bool CheckMoveBackGroundImageOver()
        {
            return BackgroundImage.rectTransform.offsetMin == OffsetEndPosition &&
                   BackgroundImage.rectTransform.offsetMax == OffsetEndPosition;
        }
    }
}