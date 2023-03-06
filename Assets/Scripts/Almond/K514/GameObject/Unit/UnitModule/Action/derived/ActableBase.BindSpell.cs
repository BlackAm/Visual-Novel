using System;
using System.Collections.Generic;
using System.Linq;
using BDG;
using UI2020;
using UnityEngine;

namespace k514
{
    public partial class ActableBase
    {
        #region <Callbacks>

        /// momo6346 - 커맨드의 스킬을 바꾸는 테스트...
        /// 바꾸고 싶은 커맨드와 UnitActionPresetTable.xml의 키를 입력하세요.
        public void ChangeCommandSkill(ControllerTool.CommandType type, ActableTool.UnitActionCluster p_UnitActionCluster)
        {
/*#if UNITY_EDITOR
            //   Debug.LogError($"[{type}]의 스킬을 {SkillPropertyLanguage.GetInstance.GetTableData(p_UnitActionCluster.ActionList.First()._UnitActionPresetRecord.ActionLanguage).content}로 변경합니다.");
            if (CustomDebug.PrintUIObjectFlag) Debug.LogError($"[{type}] 스킬을 저장.");
#endif
            if(_MasterNode._UnitNetworkPreset.UnitUniqueKey == LamiereGameManager.GetInstanceUnSafe.PlayerInfoData.ullCharacterKey)
            {
                MainGameUI.Instance.SaveSkillCommand(type, p_UnitActionCluster.GetActionIndex());
            }
            
            // 만약 해당하는 커멘드에 스킬이 없다면 스킬 넣을 준비하기.
            if (!_CommandList.Contains(type))
            {
                _CommandList.Add(type);
                if (!_InputCommandMappedActionPresetTable.ContainsKey(type))
                {
                    _InputCommandMappedActionPresetTable.Add(
                        type,
                        new ActableTool.UnitActionSpellPreset(p_UnitActionCluster));
                }
            }

            // 스킬이 있다면...
            if (_InputCommandMappedActionPresetTable.TryGetValue(type, out var o_ActionSpellPreset))
            {
                o_ActionSpellPreset.OverlapActionCluster(p_UnitActionCluster);
#if !SERVER_DRIVE
                if (MainGameUI.Instance.IsValid() && _MasterNode._UnitNetworkPreset.UnitUniqueKey == LamiereGameManager.GetInstanceUnSafe.PlayerInfoData.ullCharacterKey ||
                    LamiereGameManager.GetInstanceUnSafe.isTutorial)
                {
                    if (MainGameUI.Instance.mainUI.GetCommandState(type) != -1)
                    {
                        MainGameUI.Instance.mainUI.GetMainSkillButton(MainGameUI.Instance.mainUI.GetCommandState(type))
                            .SetImage();
                    }
                }
#endif

                /// 쿨타임 초기화하기...
                // 쿨타임을 리셋할 뿐 따로 저장하지 않습니다. 추후 수정...
                foreach (var item in _InputCommandMappedActionPresetTable)
                {
                    if (item.Key == type)
                    {
                        if (IsCooldown(item.Key))
                        {
                            var targetTimer = item.Value.ActionCooldownTimer;
                            targetTimer.Progress(10000f);
                        }
                    }
                }
            }
            else
            {
#if UNITY_EDITOR
              //  Debug.LogError($"[>>> {type}]에 스킬이 없습니다.");
#endif
            }*/
        }

        public bool GetIsSkill(ControllerTool.CommandType type)
        {
            return _InputCommandMappedActionPresetTable.ContainsKey(type);
        }
        /// momo6346 - 해당 커맨드의 스킬을 제거합니다.
        public void DeleteCommandSkill(ControllerTool.CommandType type)
        {
            /*if (_CommandList.Contains(type))
            {
                //MainGameUI.Instance.SaveSkillCommand(type);
                _CommandList.Remove(type);
                _InputCommandMappedActionPresetTable.Remove(type);

#if !SERVER_DRIVE
                if (MainGameUI.Instance.IsValid() && _MasterNode._UnitNetworkPreset.UnitUniqueKey == LamiereGameManager.GetInstanceUnSafe.PlayerInfoData.ullCharacterKey 
                    || LamiereGameManager.GetInstanceUnSafe.isTutorial)
                {
                    if (MainGameUI.Instance.mainUI.GetCommandState(type) != -1)
                    {
                        MainGameUI.Instance.mainUI.GetMainSkillButton(MainGameUI.Instance.mainUI.GetCommandState(type)).SetImage();
                        MainGameUI.Instance.mainUI.GetMainSkillButton(MainGameUI.Instance.mainUI.GetCommandState(type))._animation.Pause();
                    }
                }
#endif
#if UNITY_EDITOR
                //Debug.LogError($"[{type}]의 스킬을 제거했습니다.");
#endif
            }
            else
            {
#if UNITY_EDITOR
                //Debug.LogError($"[{type}]에 스킬이 없습니다.");
#endif   
            }*/
        }

        public void DeleteCommandSkill(List<ControllerTool.CommandType> p_CommandType)
        {
            //if (!LamiereGameManager.GetInstanceUnSafeUnSafe.isClearCommand) return;
            for (var index = 0; index < p_CommandType.Count; index++)
            {
                var commandType = p_CommandType[index];
                if (commandType != ControllerTool.CommandType.X)
                {
                    DeleteCommandSkill(commandType);
                }
            }
        }

        /// momo6346 - 해당 스킬이 있는 커맨드를 리턴합니다.
        public ControllerTool.CommandType FindCommandSkill(ActableTool.UnitActionCluster p_UnitActionCluster)
        {
            foreach (var command in _CommandList)
            {
                if (_InputCommandMappedActionPresetTable.TryGetValue(command, out var preset))
                {
                    if (preset.UnitActionCluster.ActionList.First()._UnitActionPresetRecord.ActionLanguage ==
                        p_UnitActionCluster.ActionList.First()._UnitActionPresetRecord.ActionLanguage)
                    {
                        return command;
                    }
                }
            }
            return default;
        }
        
        /// 테스트 - 커맨드에 지정된 스킬들을 출력합니다...
        public Dictionary<ControllerTool.CommandType, ActableTool.UnitActionSpellPreset> GetSkill()
        {
#if !SERVER_DRIVE
            if (LamiereGameManager.GetInstanceUnSafe.saveCommandTable != null)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintUIObjectFlag) Debug.LogError(">>>> saveCommandTable을 리턴.");
#endif
                return LamiereGameManager.GetInstanceUnSafe.saveCommandTable;
            }
            else
#endif
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintUIObjectFlag) Debug.LogError(">>>> _InputCommandMappedActionPresetTable을 리턴.");
#endif
                return _InputCommandMappedActionPresetTable;
            }

            /*foreach (var skill in _InputCommandMappedActionPresetTable)
            {
                Debug.LogError(skill.Key);
            }
            return _InputCommandMappedActionPresetTable;*/
        }

        /// 커맨드에 지정된 스킬들을 저장합니다.(LamiereGameManager.saveCommandTable)
        /// 씬 이동 후 씬 시작 시 저장했던 스킬을 적용합니다.
        public void SaveSkill(bool moveScene = false)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintUIObjectFlag) Debug.LogError("============SaveSkill=============");
            foreach (var skill in _InputCommandMappedActionPresetTable)
            {
                if (CustomDebug.PrintUIObjectFlag) Debug.LogError($"저장내역: {skill.Key}");
            }
#endif
#if !SERVER_DRIVE
            LamiereGameManager.GetInstanceUnSafe.saveCommandTable = _InputCommandMappedActionPresetTable;
#endif
        }

        public void BindActionSpell(IActableTableRecordBridge p_ActionSpellSet)
        {
            var unitActionRecords = p_ActionSpellSet.UnitActionRecords;
            _DefaultCommand = p_ActionSpellSet.DefaultCommand;
            if (!ReferenceEquals(null, unitActionRecords))
            {
                foreach (var unitActionRecord in unitActionRecords)
                {
                    var commandType = unitActionRecord.Key;
                    var actionCluster = unitActionRecord.Value;
                    BindActionSpell(commandType, actionCluster);
                }
            }
        }

        private void BindActionSpell(ControllerTool.CommandType p_CommandType, ActableTool.UnitActionCluster p_UnitActionCluster)
        {
            _CommandList.Add(p_CommandType);
            if (_InputCommandMappedActionPresetTable.TryGetValue(p_CommandType, out var o_ActionSpellPreset))
            {
                o_ActionSpellPreset.OverlapActionCluster(p_UnitActionCluster);
            }
            else
            {
                _InputCommandMappedActionPresetTable.Add(
                    p_CommandType,
                    o_ActionSpellPreset = new ActableTool.UnitActionSpellPreset(p_UnitActionCluster)
                );
            }
        }

        private void BindActionSpell(ControllerTool.CommandType p_CommandType, int p_UnitActionIndex)
        {
            _CommandList.Add(p_CommandType);
            if (_InputCommandMappedActionPresetTable.TryGetValue(p_CommandType, out var o_ActionSpellPreset))
            {
                o_ActionSpellPreset.AddAction(p_UnitActionIndex);
            }
            else
            {
                _InputCommandMappedActionPresetTable.Add(
                    p_CommandType,
                    o_ActionSpellPreset = new ActableTool.UnitActionSpellPreset(p_UnitActionIndex)
                );  
            }
        }

        private void BindActionSpell(ControllerTool.CommandType p_CommandType, List<int> p_UnitActionIndexList)
        {
            _CommandList.Add(p_CommandType);
            if (_InputCommandMappedActionPresetTable.TryGetValue(p_CommandType, out var o_ActionSpellPreset))
            {
                o_ActionSpellPreset.AddAction(p_UnitActionIndexList);
            }
            else
            {
                _InputCommandMappedActionPresetTable.Add(
                    p_CommandType,
                    o_ActionSpellPreset = new ActableTool.UnitActionSpellPreset(p_UnitActionIndexList)
                );  
            }
        }

        #endregion
    }
}