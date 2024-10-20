using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// IControllerTrackRecorder 구현체를 보유한 오브젝트를 표시하는 브릿지 인터페이스
    /// </summary>
    public interface IControllerTrackerBridge
    {
        IControllerTracker ControllerTracker { get; }
    }

    /// <summary>
    /// 현재 컨트롤러에 입력중인 대략적인 방향 및 방향 UV를 기술하는 인터페이스
    /// </summary>
    public interface IControllerTracker
    {
        /// <summary>
        /// CurrentArrowType에 입력되어있었던 방향 타입
        /// </summary>
        ArrowType PrevArrowType { get; }
        
        /// <summary>
        /// 현재 해당 컨트롤러에 입력된 방향 타입
        /// </summary>
        ArrowType CurrentArrowType { get; }

        /// <summary>
        /// 해당 오브젝트의 메인컨텐츠 수명 타입
        /// </summary>
        ControllerTool.ControllerTrackerLifeSpanType ControllerTrackerLifeSpanType { get; }

        /// <summary>
        /// 현재 해당 컨트롤러에 입력된 UV
        /// </summary>
        Vector3 CurrentControllerUV { get; }

        /// <summary>
        /// 컨트롤러 추적 데이터 갱신 콜백
        /// </summary>
        void OnUpdateController();
        
        /// <summary>
        /// 지정한 구현체의 해당 인터페이스 멤버를 세트하는 메서드
        /// </summary>
        void SetControllerDirection(ArrowType p_CurrentArrowType);

        /// <summary>
        /// 지정한 구현체의 해당 인터페이스 멤버를 세트하는 메서드
        /// </summary>
        void SetControllerDirection(ArrowType p_CurrentArrowType, Vector3 p_CurrentControllerUV);

        /// <summary>
        /// 컨텐츠 수명을 세트하는 메서드
        /// </summary>
        void SetControllerTrackerLifeSpan(ControllerTool.ControllerTrackerLifeSpanType p_ControllerTrackerLifeSpanType);
    }

    /// <summary>
    /// IControllerTracker 기본 구현체
    /// </summary>
    public class ControllerTrackRecorderBase : IControllerTracker
    {
        #region <Fields>

        public ArrowType PrevArrowType { get; private set; }
        public ArrowType CurrentArrowType { get; private set; }
        public ControllerTool.ControllerTrackerLifeSpanType ControllerTrackerLifeSpanType { get; private set; }
        public Vector3 CurrentControllerUV { get; private set; }
        
        #endregion

        #region <Constructor>

        public ControllerTrackRecorderBase()
        {
            SetControllerDirection(ArrowType.None, Vector3.zero);
            SetControllerTrackerLifeSpan(ControllerTool.ControllerTrackerLifeSpanType.Remain);
        }

        #endregion
        
        #region <Callbacks>

        public void OnUpdateController()
        {
            switch (ControllerTrackerLifeSpanType)
            {
                case ControllerTool.ControllerTrackerLifeSpanType.Remain:
                    break;
                case ControllerTool.ControllerTrackerLifeSpanType.InitializeOnUpdate :
                    SetControllerDirection(ArrowType.None);
                    break;
            }
        }

        #endregion
        
        #region <Methods>
        
        public void SetControllerDirection(ArrowType p_CurrentArrowType)
        {
            SetControllerDirection(p_CurrentArrowType, CustomMath.ArrowWorldVectorCollection[p_CurrentArrowType]);
        }
        
        public void SetControllerDirection(ArrowType p_CurrentArrowType, Vector3 p_CurrentControllerUV)
        {
            PrevArrowType = CurrentArrowType;
            CurrentArrowType = p_CurrentArrowType;
            CurrentControllerUV = p_CurrentControllerUV;
        }

        public void SetControllerTrackerLifeSpan(ControllerTool.ControllerTrackerLifeSpanType p_ControllerTrackerLifeSpanType)
        {
            ControllerTrackerLifeSpanType = p_ControllerTrackerLifeSpanType;
        }

        #endregion
    }
}