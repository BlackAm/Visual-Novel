using UnityEngine;

namespace k514
{
    public partial class AutonomyAIBase
    {
        #region <Methods>
        
        /*protected float GetSelfBoundRange()
        {
            return _CurrentAIPreset.Range + _MasterNode.GetRadius();
        }

        protected float GetSelfBoundSqrRange()
        {
            return Mathf.Pow(_CurrentAIPreset.Range + _MasterNode.GetRadius(), 2f);
        }
        
        protected float GetFocusBoundRange()
        {
            return _CurrentAIPreset.Range + _MasterNode.GetDistanceLowerBoundFromFocus();
        }

        protected float GetFocusBoundSqrRange()
        {
            return Mathf.Pow(_CurrentAIPreset.Range + _MasterNode.GetDistanceLowerBoundFromFocus(), 2f);
        }
        
        protected float GetSelfBoundRange(ThinkableTool.AIState p_RangeType)
        {
            return _StatePresetRecord[p_RangeType].Range + _MasterNode.GetRadius();
        }

        protected float GetSelfBoundSqrRange(ThinkableTool.AIState p_RangeType)
        {
            return Mathf.Pow(_StatePresetRecord[p_RangeType].Range + _MasterNode.GetRadius(), 2f);
        }
        
        protected float GetFocusBoundRange(ThinkableTool.AIState p_RangeType)
        {
            return _StatePresetRecord[p_RangeType].Range + _MasterNode.GetDistanceLowerBoundFromFocus();
        }

        protected float GetFocusBoundSqrRange(ThinkableTool.AIState p_RangeType)
        {
            return Mathf.Pow(_StatePresetRecord[p_RangeType].Range + _MasterNode.GetDistanceLowerBoundFromFocus(), 2f);
        }
        
        protected bool CompareSqrInRange(float p_CompareSqrValue, bool p_CalcBounds)
        {
            var tryRange = p_CalcBounds ? 
                Mathf.Pow(_CurrentAIPreset.Range + _MasterNode.GetDistanceLowerBoundFromFocus(), 2f) 
                : _CurrentAIPreset.SqrRange;

            return p_CompareSqrValue < tryRange;
        }
        
        protected bool CompareSqrInRange(ThinkableTool.AIState p_RangeType, float p_CompareSqrValue, bool p_CalcBounds)
        {
            var tryRange = p_CalcBounds ? 
                Mathf.Pow(_StatePresetRecord[p_RangeType].Range + _MasterNode.GetDistanceLowerBoundFromFocus(), 2f) 
                : _StatePresetRecord[p_RangeType].SqrRange;

            return p_CompareSqrValue < tryRange;
        }*/

        public override void SetAIRange(ThinkableTool.AIState p_State, float p_Value)
        {
            _StatePresetRecord[p_State] = new ThinkableTool.AIStatePreset(p_Value, _StatePresetRecord[p_State].SpeedRate);
        }
        
        #endregion   
    }
}