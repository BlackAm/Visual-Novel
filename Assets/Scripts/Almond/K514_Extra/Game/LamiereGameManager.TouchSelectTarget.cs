#if !SERVER_DRIVE
using System;
using UI2020;
using UnityEngine;

namespace k514
{
    public partial class LamiereGameManager
    {
        #region <Fields>

        private TouchEventReceiver _TouchEventReceiver;
        public Unit _TourchTarget;

        #endregion

        #region <Callbacks>

        private void OnAwakeTouchSelectTarget()
        {
            _TouchEventReceiver =
                TouchEventManager.GetInstance.GetEventReceiver<TouchEventReceiver>(
                    TouchEventRoot.TouchEventType.UnitSelected | TouchEventRoot.TouchEventType.PositionSelected, OnUnitTouched);
        }

        private void OnUnitTouched(TouchEventRoot.TouchEventType p_Type, TouchEventManager.TouchEventPreset p_Preset)
        {
            /*if (_ClientPlayer.IsValid())
            {
                if (IsQuest)
                {
                    ResetQuest(PlayerAutoMode.Passive);
                }
                
                switch (p_Type)
                {
                    case TouchEventRoot.TouchEventType.UnitSelected:
                    {
                        var targetUnit = p_Preset.SelectedUnit;
                        if (!ReferenceEquals(_ClientPlayer, targetUnit) &&
                            targetUnit.IsInteractValid(Unit.UnitStateType.UnitFightableFilterMask))
                        {
                            var unitGroupRelate = _ClientPlayer.GetGroupRelate(targetUnit);
                            switch (unitGroupRelate)
                            {
                                case UnitTool.UnitGroupRelateType.Enemy:
                                    if(ReferenceEquals(_TourchTarget, targetUnit))
                                    {
                                        SwitchAutoModeOneKill(targetUnit, _ClientPlayer._ActableObject._DefaultCommand);
                                    }
                                    else
                                    {
                                        _TourchTarget = targetUnit;
                                        SetEnemyTargetingDecalEnable((LamiereUnit)_TourchTarget);

                                    }
                                    break;
                                case UnitTool.UnitGroupRelateType.Ally:
                                    MainGameUI.Instance.mainUI.userInformation.Setting((LamiereUnit)targetUnit);
                                    break;
                                case UnitTool.UnitGroupRelateType.Neutral:
                                    break;
                            }
                        }
                    }
                        break;
                    case TouchEventRoot.TouchEventType.PositionSelected:
                    {
                        SetTouchMove(p_Preset.WorldVector);
                    }
                        break;
                }
            }*/
        }

        public Unit GetTourchTarget()
        {
            return _TourchTarget;
        }
        #endregion
    }
}
#endif