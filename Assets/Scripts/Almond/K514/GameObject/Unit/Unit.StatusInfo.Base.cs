using System;
using UnityEngine;

namespace k514
{
    public partial class Unit
    {
        #region <Const>

        /// <summary>
        /// 해당 카운트 경과 시, Hp/Mp 리젠
        /// </summary>
        private const int _DefaultRegenCount = 50;
        
        #endregion
        
        #region <Fields>
        
        /// <summary>
        /// 해당 유닛의 정보 레코드
        /// </summary>
        public UnitInfoPresetData.TableRecord _Default_UnitInfo { get; private set; }

        /// <summary>
        /// 현재 누적된 리젠 카운트
        /// </summary>
        private int _CurrentRegenCount;
        
        #endregion
        
        #region <Callbacks>

        private void OnAwakeStatusInfo()
        {
            if (!ReferenceEquals(null, _PrefabExtraDataRecord))
            {
                _Default_UnitInfo =
                    UnitInfoPresetData.GetInstanceUnSafe.GetTableData(_PrefabExtraDataRecord.UnitInfoPresetId);
            }
        }

        /// <summary>
        /// 전투능력치는 기본능력치에 영향을 받기 때문에
        /// 풀링 초기화 콜백에서는 전투능력치를 제외한 나머지 두 스테이터스 및 이벤트 초기화를 수행한다.
        /// </summary>
        private void OnPoolingStatusInfo()
        {
            SetUnitName();
            
        }

        private void OnRetrieveStatusInfo()
        {
        }

        private void OnUpdateRegenStatus()
        {
        }

        #endregion

        #region <Methods>
        
        /// <summary>
        /// 시스템에서 지정하는 이름으로 해당 오브젝트의 이름을 변경한다.
        /// </summary>
        public void SetUnitName()
        {
            SetUnitName(GetDefaultName());
        }

        /// <summary>
        /// 시스템에서 지정하는 이름으로 해당 오브젝트의 이름을 변경한다.
        /// !! 호출할 때마다, 힙에 메모리를 할당하는 메서드
        /// </summary>
        public void SetUnitNameWithTail(string p_Tail)
        {
            SetUnitName(GetDefaultName() + p_Tail);
        }
        
        /// <summary>
        /// 현재 이름을 리턴하는 메서드
        /// </summary>
        public string GetUnitName()
        {
            return name;
        }
        
        /// <summary>
        /// 파라미터로 지정한 이름을 해당 오브젝트의 이름으로 변경한다.
        /// </summary>
        public void SetUnitName(string p_Name)
        {
            if (!HasState_Or(UnitStateType.DEAD) && name != p_Name)
            {
                name = p_Name;

                if (EventHandlerValid)
                {
                    _UnitEventHandler.OnEventTriggered(UnitEventHandlerTool.UnitEventType.UnitNameChanged, new UnitEventMessage());
                }
            }
        }
        
        /// <summary>
        /// 해당 유닛의 이름을 호출한다.
        /// </summary>
        public string GetDefaultName()
        {
            if (ReferenceEquals(null, _RoleObject))
            {
                var tryDefaultName = LanguageManager.GetInstanceUnSafe?[_Default_UnitInfo.UnitNameId].content ?? "XXXX" ;
#if UNITY_EDITOR
                if (CustomDebug.AIStateName)
                { 
                    return $"{tryDefaultName} (id : {EventKey})";
                }
                else
                {
                    return tryDefaultName;
                }
#else
                return tryDefaultName;
#endif
            }
            else
            {
#if UNITY_EDITOR
                if (CustomDebug.AIStateName)
                {
                    return $"{_RoleObject.GetRoleName()} (id : {EventKey})";
                }
                else
                {
                    return _RoleObject.GetRoleName();
                }
#else
                return _RoleObject.GetRoleName();
#endif
            }
        }
        
        #endregion
    }
}