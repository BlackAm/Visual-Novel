using UnityEngine;

namespace k514
{
    /// <summary>
    /// 유닛 인공지능 모듈 기저 클래스
    /// </summary>
    public abstract partial class ThinkableBase : UnitModuleBase, IThinckable
    {
        ~ThinkableBase()
        {
            Dispose();
        }
        
        #region <Fields>

        /// <summary>
        /// 해당 모듈의 타입
        /// </summary>
        public UnitAIDataRoot.UnitMindType _MindType { get; protected set; }
        
        /// <summary>
        /// 해당 모듈을 기술하는 테이블 레코드
        /// </summary>
        public IThinkableTableRecordBridge _MindRecord { get; protected set; }
        
        /// <summary>
        /// 해당 사고모듈이 최초에 스폰된 pivotPosition
        /// </summary>
        protected Vector3 _OriginPivot;

        #endregion

        #region <Callbacks>

        public virtual IThinckable OnInitializeAI(UnitAIDataRoot.UnitMindType p_MindType, Unit p_TargetUnit, IThinkableTableRecordBridge p_MindPreset)
        {
            UnitModuleType = UnitModuleDataTool.UnitModuleType.AI;
            _MindType = p_MindType;
            _MasterNode = p_TargetUnit;
            _MindRecord = p_MindPreset;

            return this;
        }

        public override void OnMasterNodePooling()
        {
            _OriginPivot = _MasterNode.GetPivotPosition();

            // 이후에 OnModuleNotify 콜백이 호출되므로, 위의 필드들을 먼저 초기화시켜준다.
            base.OnMasterNodePooling();
        }
        
        protected override void OnModuleNotify()
        {
        }
        
        protected override void OnModuleSleep()
        {
            StopMove(_MasterNode._ActableObject._CurrentIdleState);
            _MasterNode._ActableObject.ClearReserveCommandInput();
        }

        public override void OnPreUpdate(float p_DeltaTime)
        {
        }

        public override void OnUnitDead(bool p_Instant)
        {
        }

        public override void OnFocusUnitChanged(Unit p_PrevFocusUnit, Unit p_FocusUnit)
        {
        }

        public override void OnPivotChanged(PositionStatePreset p_PositionStatePreset, bool p_SyncPosition)
        {
        }
        
        public override void OnCheckOverlapObject(UnitStampPreset p_UnitStampPreset)
        {
        }
        
        /// <summary>
        /// 해당 유닛이 지면에 도달한 경우 호출되는 인공지능 로직
        /// </summary>
        public override void OnReachedGround(UnitStampPreset p_UnitStampPreset)
        {
            // 길찾기 중이 아닐 때,
//            if (!_MasterNode.IsAIThroughPath())
//            {
//                // 다른 유닛 위에 착지하거나 겹친 경우 경우 네브메쉬 에이전트를 잠시동안 활성화 시켜서
//                // 유닛 배치를 적당하게 해준다.
//                if (p_UnitStampPreset.ResultFlag.HasAnyFlagExceptNone(UnitTool.UnitStampResultFlag.UnitStamped))
//                {
//                    var stamping = p_UnitStampPreset.Stamping;
//                    var stamped = p_UnitStampPreset.Stamped;
//                    var tryRadius = stamping.GetRadius();
//                    var targetRadius = stamped.GetRadius();
//                    var xzRandomOffset = CustomMath.RandomSymmetricVector(CustomMath.XYZType.ZX, tryRadius + targetRadius, tryRadius + 2f * targetRadius);
//                    stamped.OrderAIMoveTo(stamping._Transform.position + xzRandomOffset, false);
//                }
//            }
        }
        
        #endregion

        #region <Methods>

        public abstract void LoadAIPresetFromTableRecord();

        public abstract ThinkableTool.AIStatePreset GetAIPreset(ThinkableTool.AIState p_State);

        /// <summary>
        /// 현재 인공지능 상태를 리턴하는 메서드
        /// </summary>
        public abstract ThinkableTool.AIState GetCurrentAIState();
        
        /// <summary>
        /// 펫 주인 유닛을 지정하는 메서드
        /// </summary>
        public abstract void SetSlaveMasterUnit(Unit p_Target);

        /// <summary>
        /// 인공지능 속도 배율을 리턴하는 메서드
        /// </summary>
        public abstract float GetAISpeedRate();

        /// <summary>
        /// 예약된 커맨드를 초기화 시키는 메서드
        /// </summary>
        public abstract void ClearReserveCommand();
        
        public abstract void SetAIRange(ThinkableTool.AIState p_State, float p_Value);

        #endregion
    }
}