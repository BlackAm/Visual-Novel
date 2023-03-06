using UnityEngine;

namespace k514
{
    /*public class BuffStatusResult
    {
        public int _Key;
        public int _effectIndex;
        public uint _Duration;
        public bool _BuffisActive;
        public Buff _Buff;
        public LamiereUnit _unit;
        public BaseStatusPreset _BaseStatusMain, _BaseStatusTotal;
        public BattleStatusPreset _BattleStatusMain, _BattleStatusTotal;
        public SkillStatusPreset _SkillStatusMain, _SkillStatusTotal;
        public GameEventTimerHandlerWrapper _SpawnBuffDuration;
        public Vector3 _effectRot;
        (bool, VFXUnit) _vfxUnit;

        public BuffStatusResult(LamiereUnit p_Unit, BaseStatusPreset p_BaseStatus, BattleStatusPreset p_BattleStatus, SkillStatusPreset p_SkillStatus, uint p_Duration, int p_effectIndex, Vector3 p_effectRot, Buff p_Buff){
            _Key = default;
            _unit = p_Unit;
            _BaseStatusMain = p_BaseStatus;
            _BattleStatusMain = p_BattleStatus;
            _SkillStatusMain = p_SkillStatus;
            _Duration = p_Duration;
            _SpawnBuffDuration = null;
            _BuffisActive = false;
            _effectIndex = p_effectIndex;
            _effectRot = p_effectRot;
            _Buff = p_Buff;
        }

        public void NewKey(int p_Key)
        {
            _Key = p_Key;
        }

        public void BuffOn()
        {
            _BaseStatusTotal = _BaseStatusMain;
            _BattleStatusTotal = _BattleStatusMain;
            _SkillStatusTotal = _SkillStatusMain;

            _unit.OnBuffAdded(_BaseStatusTotal, _BattleStatusTotal);
            _unit.OnBuffAdded(_SkillStatusTotal);
            ProgressBuffDuration();
            if(!_effectIndex.Equals(0))
            {
                _vfxUnit = (true, UnitRenderingManager.GetInstance.CastUnitAttachedVfx(_effectIndex, _unit, Vector3.zero).Item2);
                _vfxUnit.Item2.SetRotation(_effectRot);
            }
        }
        
#region <Methods/Timer>
        public void ProgressBuffDuration()
        {
            _BuffisActive = true;

            _SpawnBuffDuration = GameEventTimerHandlerManager.GetInstance.SpawnEventTimerHandler(SystemBoot.TimerType.GameTimer, false);
            
            _SpawnBuffDuration
                .AddEvent
                (
                    _Duration,
                    handler =>
                    {
                        handler.Arg1.OnTriggerBuffDurationOver();
                        return true;
                    }, 
                    null, this
                );
            _SpawnBuffDuration.StartEvent();
        }

        public void OnTriggerBuffDurationOver()
        {
            if(_BuffisActive)
            {

                _unit.OnBuffDeleted(_BaseStatusTotal, _BattleStatusTotal);
                _unit.OnBuffDeleted(_SkillStatusTotal);
#if UNITY_EDITOR
                Debug.Log($"({_Key}) 버프 종료");
#endif
                EffectDown();
                _BuffisActive = false;
            }
              
        }

        public void EffectDown()
        {
            if(_vfxUnit.Item1)
            {
                _vfxUnit.Item1 = false;
                _vfxUnit.Item2.SetRemove(false, 2);
            } 
        }

        public void Retrived()
        {
            if (!ReferenceEquals(_SpawnBuffDuration, null))
            {
                EventTimerTool.ReleaseEventHandler(ref _SpawnBuffDuration);
            }
            if(!_effectIndex.Equals(0)) _vfxUnit.Item2.Dispose();
        }

        public bool VoidBuffCheck(){
            //Debug.Log($"base : {_BaseStatusTotal.FlagMask}({(long)_BaseStatusTotal.FlagMask}) / battle : {_BattleStatusTotal.FlagMask}({(long)_BattleStatusTotal.FlagMask}) / skill : {_SkillStatusTotal.FlagMask}({(long)_SkillStatusTotal.FlagMask})");

            if(!(_BaseStatusTotal.FlagMask).Equals(BaseStatusPreset.BaseStatusType.None)) return false;
            if(!(_BattleStatusTotal.FlagMask).Equals(BattleStatusPreset.BattleStatusType.None)) return false;
            if(!(_SkillStatusTotal.FlagMask).Equals(SkillStatusPreset.SkillStatusType.None)) return false;

            if(!ReferenceEquals(_SpawnBuffDuration, null)) _SpawnBuffDuration.CancelEvent();
            OnTriggerBuffDurationOver();
            return true;
        }
    }
#endregion*/
}
