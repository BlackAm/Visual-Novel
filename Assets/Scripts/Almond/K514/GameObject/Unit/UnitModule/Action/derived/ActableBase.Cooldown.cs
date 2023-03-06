using System;
using System.Collections.Generic;
using UI2020;
using UnityEngine;

namespace k514
{
    public partial class ActableBase
    {
        #region <Methods>
        
        protected void UpdateCurrentActionCooldown()
        {
            var currentCommandType = _CurrentActivatedInputTriggerPreset._InputPreset.CommandType;
            var currentActionCooldown = _CurrentUnitAction._UnitActionPresetRecord.ActionCoolDown;
            SetCooldown(currentCommandType, currentActionCooldown, false);
        }

        /// <summary>
        /// 모든 스킬의 쿨타임을 초기화시킨다.
        /// </summary>
        public void ResetCooldown()
        {
            foreach (var coolDownTimerKV in _InputCommandMappedActionPresetTable)
            {
                var targetTimer = coolDownTimerKV.Value.ActionCooldownTimer;
                targetTimer.Progress(10000f);
            }
        }

        public void ProgressCooldown(float p_DeltaTime)
        {
            foreach (var coolDownTimerKV in _InputCommandMappedActionPresetTable)
            {
                var targetTimer = coolDownTimerKV.Value.ActionCooldownTimer;
                var commandType = coolDownTimerKV.Key;
#if !SERVER_DRIVE
                if (_MasterNode.IsPlayer)
                {
                    // momo6346 - 클릭한 스킬에 맞는 슬롯에 쿨타임 적용하기.
                    switch (commandType)
                    {
                        case ControllerTool.CommandType.Z:
                            targetTimer = _InputCommandMappedActionPresetTable[LamiereGameManager.GetInstanceUnSafe.buttonZ]
                                .ActionCooldownTimer;
                            break;
                        case ControllerTool.CommandType.V:
                            targetTimer = _InputCommandMappedActionPresetTable[LamiereGameManager.GetInstanceUnSafe.buttonV]
                                .ActionCooldownTimer;
                            break;
                        case ControllerTool.CommandType.B:
                            targetTimer = _InputCommandMappedActionPresetTable[LamiereGameManager.GetInstanceUnSafe.buttonB]
                                .ActionCooldownTimer;
                            break;
                        case ControllerTool.CommandType.L_Ctrl:
                            targetTimer =
                                _InputCommandMappedActionPresetTable[LamiereGameManager.GetInstanceUnSafe.buttonCtrl]
                                    .ActionCooldownTimer;
                            break;
                        case ControllerTool.CommandType.W:
                            targetTimer = _InputCommandMappedActionPresetTable[ControllerTool.CommandType.W]
                                .ActionCooldownTimer;
                            break;
                        case ControllerTool.CommandType.E:
                            targetTimer = _InputCommandMappedActionPresetTable[ControllerTool.CommandType.E]
                                .ActionCooldownTimer;
                            break;
                        case ControllerTool.CommandType.R:
                            targetTimer = _InputCommandMappedActionPresetTable[ControllerTool.CommandType.R]
                                .ActionCooldownTimer;
                            break;
                        case ControllerTool.CommandType.T:
                            targetTimer = _InputCommandMappedActionPresetTable[ControllerTool.CommandType.T]
                                .ActionCooldownTimer;
                            break;
                    }
                }
                /*if (targetTimer.CheckValid())
                {
                    _MasterNode.OnTriggerCoolDownOver(commandType);
                    // 효과 멈춤.
                    if (MainGameUI.Instance.mainUI.GetQuickSlotButton(commandType) == null)
                    {
                        var button =
                            MainGameUI.Instance.mainUI.GetMainSkillButton(
                                MainGameUI.Instance.mainUI.GetCommandState(commandType));
                        if (button._animation.isPlaying)
                        {
                            button.ResetCoolDown();
                            button.SetEffect(false);
                        }
                    }
                    else
                    {
                        var quickSlot = MainGameUI.Instance.mainUI.GetQuickSlotButton(commandType);
                        if (quickSlot.coolTime.enabled)
                        {
                            quickSlot.coolTime.enabled = false;
                        }
                    }
                }
                else
                {
                    targetTimer.Progress(p_DeltaTime);
                    _MasterNode.OnTriggerCoolDownRateChanged(commandType, targetTimer.GetProgressRate());
                }*/
#else
                if (targetTimer.CheckValid())
                {
                    _MasterNode.OnTriggerCoolDownOver(commandType);
                }
                else
                {
                    targetTimer.Progress(p_DeltaTime);
                    _MasterNode.OnTriggerCoolDownRateChanged(commandType, targetTimer.GetProgressRate());
                }
#endif
            }
        }

        public void SetCooldown(ControllerTool.CommandType p_Type, float p_Cooldown, bool p_ResetCooldown)
        {
/*#if UNITY_EDITOR
            if (CustomDebug.SkillCoolTimeZero)
            {
                return;
            }
#endif

#if !SERVER_DRIVE

            // 스킬 애니메이션 활성화...
            if (p_Type != ControllerTool.CommandType.X)
            {
                if (MainGameUI.Instance.mainUI.GetQuickSlotButton(p_Type) == null)
                {
                    var button = MainGameUI.Instance.mainUI.GetMainSkillButton(MainGameUI.Instance.mainUI.GetCommandState(p_Type));
                    if (!button._animation.isPlaying)
                    {
                        if(_MasterNode._UnitNetworkPreset.UnitUniqueKey == LamiereGameManager.GetInstanceUnSafe.PlayerInfoData.ullCharacterKey)
                        {
                            button.StartCoolDown();
                            button.SetEffect(true);
                        }
                    }   
                } 
                else
                {
                    MainGameUI.Instance.mainUI.GetQuickSlotButton(p_Type).coolTime.enabled = true;
                }
            }
#endif
            if (!_InputCommandMappedActionPresetTable.ContainsKey(p_Type)) return;
            var targetTimer = _InputCommandMappedActionPresetTable[p_Type].ActionCooldownTimer;
            targetTimer.RespawnTimer(p_Cooldown, p_ResetCooldown);
            _MasterNode.OnTriggerCoolDownStart(p_Type);*/
        }

        public bool IsCooldown(ControllerTool.CommandType p_Type)
        {
            var targetTimer = _InputCommandMappedActionPresetTable[p_Type].ActionCooldownTimer;
            return !targetTimer.IsOver();
        }

        #endregion
    }
}