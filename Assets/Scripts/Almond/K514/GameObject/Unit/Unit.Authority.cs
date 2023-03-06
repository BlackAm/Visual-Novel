using System;
using UnityEngine;

namespace k514
{
    public partial class Unit
    {
        #region <Fields>

        /// <summary>
        /// 유닛 권한 플래그 마스크
        /// </summary>
        [NonSerialized] public UnitTool.UnitAuthorityFlag UnitAuthorityFlagMask;
        
        #endregion

        #region <Enums>



        #endregion
        
        #region <Callbacks>

        private void OnAwakeAuthority()
        {
        }

        private void OnPoolingAuthority()
        {
            // 현재 유닛 권한 플래그와 None을 비교하여
            OnCompareAuthorityFlagChange(UnitTool.UnitAuthorityFlag.None, UnitAuthorityFlagMask);
            OnAuthorityFlagChange();
        }
        
        private void OnRetrieveAuthority()
        {
            ClearAuthority();
        }

        private void OnCompareAuthorityFlagChange(UnitTool.UnitAuthorityFlag p_PrevMaskState, UnitTool.UnitAuthorityFlag p_ChangedMaskState)
        {
            UnitAuthorityFlagMask = p_ChangedMaskState;
            // PrefabModelTool에서 프리팹을 생성하는 과정에서, 권한 플래그를 추가하는데 
            // 프리팹 생성 시(Spawned) 및 유닛 초기화 시(Pooled)에는 아직 UnitEventHandler가 풀링을 통해 초기화되지 않아서 아래 블록에 도달하면 안된다.
            if (PoolState == PoolState.Actived || PoolState == PoolState.Retrieving)
            {
                var flagChanged = false;
                var enumerator = UnitTool.UnitAuthorityFlagEnumerator;
                foreach (var unitAuthorityFlag in enumerator)
                {
                    var prevSetted = HasAnyAuthority(p_PrevMaskState, unitAuthorityFlag);
                    var flagSetted = HasAnyAuthority(p_ChangedMaskState, unitAuthorityFlag);
                    if (prevSetted != flagSetted)
                    {
                        flagChanged = true;
                        switch (unitAuthorityFlag)
                        {
                            case UnitTool.UnitAuthorityFlag.Player:
                                if (flagSetted)
                                {
#if SERVER_DRIVE
    #if UNITY_EDITOR
                                    SetServerTestRenderColor(Color.green);
    #endif
#else
                                    // 플레이어 매니저가 해당 유닛을 플레이어로 참조하게한다.
                                    PlayerManager.GetInstance.Player = this;
#endif
                                }
                                else
                                {
#if SERVER_DRIVE
    #if UNITY_EDITOR
                                    SetServerTestRenderColor(Color.cyan);
    #endif
#else
                                    // 플레이어 매니저의 참조 정보를 초기화 시켜준다.
                                    PlayerManager.GetInstance.OnPlayerReleased(this);
#endif
                                }
                                break;
                            
                            case UnitTool.UnitAuthorityFlag.OtherPlayer:
                                if (flagSetted)
                                {
#if SERVER_DRIVE
    #if UNITY_EDITOR
                                    SetServerTestRenderColor(Color.red);
    #endif
#endif
                                }
                                else
                                {
#if SERVER_DRIVE
    #if UNITY_EDITOR
                                    SetServerTestRenderColor(Color.cyan);
    #endif
#endif
                                }
                                break;
                        }
                    }
                }

                if (flagChanged)
                {
                    OnAuthorityFlagChange();
                }
            }
        }

        protected virtual void OnAuthorityFlagChange()
        {
            OnUpdatePriority();
        }

        #endregion
        
        #region <Methods>
        
        public bool IsPositionEventSender()
        {
            return HasAnyAuthority(UnitTool.UnitAuthorityFlag.DistanceEventSender);
        }

        public void AddAuthority(UnitTool.UnitAuthorityFlag p_TryMask)
        {
            OnCompareAuthorityFlagChange(UnitAuthorityFlagMask, UnitAuthorityFlagMask | p_TryMask);
        }

        public void RemoveAuthority(UnitTool.UnitAuthorityFlag p_TryMask)
        {
            OnCompareAuthorityFlagChange(UnitAuthorityFlagMask, UnitAuthorityFlagMask & ~p_TryMask);
        }

        public void ClearAuthority()
        {
            TurnAuthority(UnitTool.UnitAuthorityFlag.None);
        }

        public void TurnAuthority(UnitTool.UnitAuthorityFlag p_TryMask)
        {
            OnCompareAuthorityFlagChange(UnitAuthorityFlagMask, p_TryMask);
        }
        
        public bool HasAnyAuthority(UnitTool.UnitAuthorityFlag p_CompareFlag)
        {
            return HasAnyAuthority(UnitAuthorityFlagMask, p_CompareFlag);
        }
        
        public bool HasAnyAuthority(UnitTool.UnitAuthorityFlag p_TargetMask, UnitTool.UnitAuthorityFlag p_CompareFlag)
        {
            return (p_TargetMask & p_CompareFlag) != UnitTool.UnitAuthorityFlag.None;
        }
        
        #endregion
    }
}