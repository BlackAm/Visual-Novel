using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public abstract class UnitMindAutonomyPresetData<M, K> : UnitMindPresetDataBase<M, K>, IThinkableAutonomyTableBridge 
        where M : UnitMindAutonomyPresetData<M, K>, new() 
        where K : UnitMindAutonomyPresetData<M, K>.AutonomyMindTableBaseRecord, new()
    {
        public abstract class AutonomyMindTableBaseRecord : MindTableBaseRecord, IThinkableAutonomyTableRecordBridge
        {
            public ThinkableTool.AIExtraFlag AIExtraFlag { get; protected set; }
            public Dictionary<ThinkableTool.AIState, ThinkableTool.AIStatePreset> StatePresetRecord{ get; protected set; }
            public ThinkableTool.AITracePivotSelectType AITracePivotSelectType { get; protected set; }
            public ThinkableTool.AITracePivotSelectType AITracePivotSelectTypeWhenTargetMoving { get; protected set; }
            public float WanderingRadius { get; protected set; }
            public int WanderingIntervalDsec { get; protected set; }
            public float Carefulness{ get; protected set; }
            
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                Carefulness = Mathf.Clamp(Carefulness, 0.5f, 1f);
            }
        }
    }
}