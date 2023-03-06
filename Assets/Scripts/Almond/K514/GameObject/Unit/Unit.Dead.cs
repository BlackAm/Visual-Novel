using System;
using UnityEngine;

namespace k514
{
    public partial class Unit
    {
        #region <Fields>

        /// <summary>
        /// 해당 유닛이 처치한 유닛 숫자
        /// </summary>
        [NonSerialized] public int KillPoint;
        
        #endregion

        #region <Callbacks>

        private void OnAwakeDead()
        {
        }

        private void OnPoolingDead()
        {
            KillPoint = 0;
        }
        
        private void OnRetrieveDead()
        {
            ClearState();
            ClearTimerEvent();
        }

        /// <summary>
        /// 유닛 사망시 호출되는 콜백
        /// </summary>
        private void OnUnitDead(bool p_Instant)
        {
            if (!HasState_Or(UnitStateType.DEAD))
            {
#if UNITY_EDITOR
                if (CustomDebug.AIStateName)
                {
                    SetUnitNameWithTail($"(Dead)");
                }
#endif 
                
                ClearState();
                ClearTimerEvent();
                
                // 사망 플래그를 세운다.
                AddState(UnitStateType.DEAD);
                
                // 정상적인 사망 연출을 위해 유닛을 활성화 시켜준다.
                SetUnitDisable(false);
                
#if UNITY_EDITOR
                if (CustomDebug.PrintGameSystemLog)
                {
                    Debug.Log($"[{name}] / Dead");
                }
#endif
                OnUnitStateChangedDead();
                
                // 서브 유닛 모듈에 이벤트를 전파한다.
                for (int i = 0; i < _ModuleCount; i++)
                {
                    _ModuleList[i].CurrentSelectedModule.OnUnitDead(p_Instant);
                }
            }
        }

        protected virtual void OnUnitStateChangedDead()
        {
#if !SERVER_DRIVE
            if (IsPlayer)
            {
                PlayerManager.GetInstance.OnPlayerDead();
            }
#endif
            _UnitEventHandler.OnEventTriggered(UnitEventHandlerTool.UnitEventType.UnitDead, new UnitEventMessage(this));

#if !SERVER_DRIVE
            if (IsFocused)
            {
                CameraManager.GetInstanceUnSafe.OnCameraFocusDead(this);
            }
#endif
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 해당 유닛을 파괴한다.
        /// </summary>
        public void SetDead(bool p_Instant, bool NetworkDead = false)
        {
#if SERVER_DRIVE
            UnitInteractManager.GetInstance.RemoveUnitViweKey(this);
            NetworkPacketManager.CharacterStateInfo(_UnitNetworkPreset.UnitUniqueKey, Almond.Network.StateAction.DEAD, 0, this);
            OnUnitDead(p_Instant);
#else
            if (!HasState_Or(UnitStateType.NETWORK_DEAD) || HasState_Or(UnitStateType.NETWORK_DEAD) && NetworkDead)
            {
                OnUnitDead(p_Instant);
            }
#endif

        }

        #endregion
    }
}