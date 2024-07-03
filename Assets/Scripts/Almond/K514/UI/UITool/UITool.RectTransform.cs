#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.EventSystems;
      
namespace k514
{
    public partial class UITool
    {
        #region <Methods>
      
        /// <summary>
        /// 지정한 UI객체의 위치를 지정하는 메서드.
        /// <param name="p_MinXYAnchor">좌하단 앵커 좌표</param>
        /// <param name="p_MaxXYAnchor">우상단 앵커 좌표</param>
        /// <param name="p_Pivot">앵커 Rect 기준으로 회전축 및 SizeDelta 변환 축</param>
        /// <param name="p_SizeDelta">(Rect 너비 - Anchor Rect 너비, Rect 높이 - Anchor Rect 높이), pivot을 기준으로 sizeDelta가 되도록 Rect의 크기가 조정된다.</param>
        /// <param name="p_Offset">위치 오프셋</param>
        /// </summary>
        public static void SetPivotPreset(this RectTransform p_TargetRectTransform, Vector2 p_MinXYAnchor, Vector2 p_MaxXYAnchor, Vector2 p_Pivot, Vector2 p_SizeDelta, Vector2 p_Offset)
        {
            p_TargetRectTransform.anchorMin = p_MinXYAnchor;
            p_TargetRectTransform.anchorMax = p_MaxXYAnchor;
            p_TargetRectTransform.pivot = p_Pivot;
            p_TargetRectTransform.sizeDelta += p_SizeDelta;
            p_TargetRectTransform.anchoredPosition = p_Offset;
            

            //팝업 위치 오류로 인해 임시로 넣어두었습니다.
            p_TargetRectTransform.localScale = new Vector3(1,1,1);
            p_TargetRectTransform.localPosition = new Vector3(p_TargetRectTransform.localPosition.x, p_TargetRectTransform.localPosition.y, 0);
        }
              
        /// <summary>
        /// 지정한 UI객체의 위치를 지정하는 메서드.
        /// <param name="p_XAnchorType">너비 앵커 타입</param>
        /// <param name="p_YAnchorType">높이 앵커 타입</param>
        /// <param name="p_SizeDelta">(Rect 너비 - Anchor Rect 너비, Rect 높이 - Anchor Rect 높이), pivot을 기준으로 sizeDelta가 되도록 Rect의 크기가 조정된다.</param>
        /// <param name="p_Offset">위치 오프셋</param>
        /// </summary>
        public static void SetPivotPreset(this RectTransform p_TargetRectTransform, XAnchorType p_XAnchorType, YAnchorType p_YAnchorType, Vector2 p_SizeDelta, Vector2 p_Offset)
        {
            var anchorMinVector = Vector2.zero;
            var anchorMaxVector = Vector2.zero;
            var targetPivotVector = Vector2.zero;
                  
            switch (p_XAnchorType)
            {
                case XAnchorType.Left:
                    switch (p_YAnchorType)
                    {
                        case YAnchorType.Top:
                            anchorMinVector = new Vector2(0f, 1f);
                            anchorMaxVector = new Vector2(0f, 1f);
                            targetPivotVector = new Vector2(0f, 1f);
                            break;
                        case YAnchorType.Middle:
                            anchorMinVector = new Vector2(0f, 0.5f);
                            anchorMaxVector = new Vector2(0f, 0.5f);                      
                            targetPivotVector = new Vector2(0f, 0.5f);
                            break;
                        case YAnchorType.Bottom:
                            anchorMinVector = new Vector2(0f, 0f);
                            anchorMaxVector = new Vector2(0f, 0f);                  
                            targetPivotVector = new Vector2(0f, 0f);
                            break;
                        case YAnchorType.Stretch:
                            anchorMinVector = new Vector2(0f, 0f);
                            anchorMaxVector = new Vector2(0f, 1f);                 
                            targetPivotVector = new Vector2(0f, 0.5f);
                            break;
                    }
                    break;
                case XAnchorType.Center:
                    switch (p_YAnchorType)
                    {
                        case YAnchorType.Top:
                            anchorMinVector = new Vector2(0.5f, 1f);
                            anchorMaxVector = new Vector2(0.5f, 1f);
                            targetPivotVector = new Vector2(0.5f, 1f);
                            break;
                        case YAnchorType.Middle:
                            anchorMinVector = new Vector2(0.5f, 0.5f);
                            anchorMaxVector = new Vector2(0.5f, 0.5f);                      
                            targetPivotVector = new Vector2(0.5f, 0.5f);
                            break;
                        case YAnchorType.Bottom:
                            anchorMinVector = new Vector2(0.5f, 0f);
                            anchorMaxVector = new Vector2(0.5f, 0f);                  
                            targetPivotVector = new Vector2(0.5f, 0f);
                            break;
                        case YAnchorType.Stretch:
                            anchorMinVector = new Vector2(0.5f, 0f);
                            anchorMaxVector = new Vector2(0.5f, 1f);                 
                            targetPivotVector = new Vector2(0.5f, 0.5f);
                            break;
                    }
                    break;
                case XAnchorType.Right:
                    switch (p_YAnchorType)
                    {
                        case YAnchorType.Top:
                            anchorMinVector = new Vector2(1f, 1f);
                            anchorMaxVector = new Vector2(1f, 1f);
                            targetPivotVector = new Vector2(1f, 1f);
                            break;
                        case YAnchorType.Middle:
                            anchorMinVector = new Vector2(1f, 0.5f);
                            anchorMaxVector = new Vector2(1f, 0.5f);                      
                            targetPivotVector = new Vector2(1f, 0.5f);
                            break;
                        case YAnchorType.Bottom:
                            anchorMinVector = new Vector2(1f, 0f);
                            anchorMaxVector = new Vector2(1f, 0f);                  
                            targetPivotVector = new Vector2(1f, 0f);
                            break;
                        case YAnchorType.Stretch:
                            anchorMinVector = new Vector2(1f, 0f);
                            anchorMaxVector = new Vector2(1f, 1f);                 
                            targetPivotVector = new Vector2(1f, 0.5f);
                            break;
                    }
                    break;
                case XAnchorType.Stretch:
                    switch (p_YAnchorType)
                    {
                        case YAnchorType.Top:
                            anchorMinVector = new Vector2(0f, 1f);
                            anchorMaxVector = new Vector2(1f, 1f);
                            targetPivotVector = new Vector2(0.5f, 1f);
                            break;
                        case YAnchorType.Middle:
                            anchorMinVector = new Vector2(0f, 0.5f);
                            anchorMaxVector = new Vector2(1f, 0.5f);                      
                            targetPivotVector = new Vector2(0.5f, 0.5f);
                            break;
                        case YAnchorType.Bottom:
                            anchorMinVector = new Vector2(0f, 0f);
                            anchorMaxVector = new Vector2(1f, 0f);                  
                            targetPivotVector = new Vector2(0.5f, 0f);
                            break;
                        case YAnchorType.Stretch:
                            anchorMinVector = new Vector2(0f, 0f);
                            anchorMaxVector = new Vector2(1f, 1f);                 
                            targetPivotVector = new Vector2(0.5f, 0.5f);
                            break;
                    }
                    break;
            }
      
            p_TargetRectTransform.SetPivotPreset(anchorMinVector, anchorMaxVector, targetPivotVector, p_SizeDelta, p_Offset);
        }
      
        /// <summary>
        /// 지정한 UI객체의 위치를 지정하는 메서드.
        /// <param name="p_XAnchorType">너비 앵커 타입</param>
        /// <param name="p_YAnchorType">높이 앵커 타입</param>
        /// </summary>
        public static void SetPivotPreset(this RectTransform p_TargetRectTransform, XAnchorType p_XAnchorType, YAnchorType p_YAnchorType)
        {
            SetPivotPreset(p_TargetRectTransform, p_XAnchorType, p_YAnchorType, Vector2.zero, Vector2.zero);
        }
              
        /// <summary>
        /// 메인 월드 카메라 기준으로 지정한 Transform 위치에 해당 RectTransform을 겹치도록 위치시키는 메서드
        /// </summary>
        public static void SetScreenPos(this RectTransform p_TargetRectTransform, Transform p_TracingWorldTransform)
        {
            p_TargetRectTransform.SetScreenPos(p_TracingWorldTransform.position);
        }
              
        /// <summary>
        /// 메인 월드 카메라 기준으로 지정한 Transform 위치에 해당 RectTransform을 겹치도록 위치시키는 메서드
        /// </summary>
        public static void SetScreenPos(this RectTransform p_TargetRectTransform, Transform p_TracingWorldTransform, Vector3 p_Offset)
        {
            p_TargetRectTransform.SetScreenPos(p_TracingWorldTransform.position + p_Offset);
        }
      
        /// <summary>
        /// 메인 월드 카메라 기준으로 지정한 Transform 위치에 해당 RectTransform을 겹치도록 위치시키는 메서드
        /// </summary>
        public static void SetScreenPos(this RectTransform p_TargetRectTransform, Vector3 p_WorldPos)
        {
            var mainCamera = CameraManager.GetInstanceUnSafe.MainCamera;
            var targetScreenPos =
                Vector3.Scale(new Vector3(1f, 1f, 0f),
                    mainCamera.WorldToScreenPoint(p_WorldPos)) - 0.5f * SystemMaintenance.GetCurrentScreenScaleVector3;
            p_TargetRectTransform.localPosition = targetScreenPos;
        }
      
        public static void SetAddScreenPos(this RectTransform p_TargetRectTransform, Vector3 p_Offset)
        {
            p_TargetRectTransform.localPosition += p_Offset;
        }
      
        #endregion
      
        #region <RectTransformPreset>
      
        public static void SetTransformPreset(this RectTransform p_TargetTransform, RectTransformPreset p_Preset)
        {
            p_Preset.ApplyAffine(p_TargetTransform);
        }
      
        public static RectTransformPreset GetTransformPreset(this RectTransform p_TargetTransform)
        {
            return new RectTransformPreset(p_TargetTransform);
        }
              
        public struct RectTransformPreset
        {
            public RectTransform Wrapper;
            public Vector2 AnchorPosition;
            public Vector2 SizeDelta;
            public Vector2 AnchorMin, AnchorMax;
            public Vector2 Pivot;
      
            public RectTransformPreset(RectTransform p_RectTransform)
            {
                Wrapper = p_RectTransform;
                AnchorPosition = Wrapper.anchoredPosition;
                SizeDelta = Wrapper.sizeDelta;
                AnchorMin = Wrapper.anchorMin;
                AnchorMax = Wrapper.anchorMax;
                Pivot = Wrapper.pivot;
            }
                  
            public void ResetAffine()
            {
                ApplyAffine(Wrapper);
            }
                  
            public void SetAffine(RectTransformPreset p_TransformPreset)
            {
                Wrapper.anchoredPosition = p_TransformPreset.AnchorPosition;
                Wrapper.sizeDelta = p_TransformPreset.SizeDelta;
                Wrapper.anchorMin = p_TransformPreset.AnchorMin; 
                Wrapper.anchorMax = p_TransformPreset.AnchorMax; 
                Wrapper.pivot = p_TransformPreset.Pivot; 
            }
      
            public void ApplyAffine(RectTransform p_TargetWrapper)
            {
                p_TargetWrapper.anchoredPosition = AnchorPosition;
                p_TargetWrapper.sizeDelta = SizeDelta;
                p_TargetWrapper.anchorMin = AnchorMin;
                p_TargetWrapper.anchorMax = AnchorMax;
                p_TargetWrapper.pivot = Pivot;
            }
                  
        }
      
        #endregion
      
        #region <RectTransformPlane>
      
        /// <summary>
        /// CustomPlane을 응용하여, 화면의 UI에 가상의 평면을 설정하고
        /// 해당 평면에서의 입력 이벤트에 대한 검증을 기술하는 클래스
        /// </summary>
        public struct RectTransformPlane
        {
            #region <Fields>
      
            public RectTransform RectTransform;
            public CustomPlane CustomPlane;
      
            #endregion
      
            #region <Methods>
      
            public void InitPlane(RectTransform p_RectTransform)
            {
                RectTransform = p_RectTransform;
                UpdatePlane();
            }
      
            public void UpdatePlane()
            {
                CustomPlane = CustomPlane.Get_UI_Basis_Location(RectTransform);
            }
      
            public bool IsOverRectTransform(PointerEventData p_PointerEventData)
            {
                return CustomPlane.CheckPointInnerPlane(p_PointerEventData.position);
            }
                  
            public bool IsOverRectTransform(Vector3 p_Position)
            {
                return CustomPlane.CheckPointInnerPlane(p_Position);
            }
        }
      
        #endregion
      
        #endregion
    }
}
#endif