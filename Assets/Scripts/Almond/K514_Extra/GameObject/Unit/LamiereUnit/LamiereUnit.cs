using System;
using BDG;
using UnityEngine;
using UI2020;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace k514
{
    public partial class LamiereUnit : Unit
    {
        #region <Consts>

        public static int UnitNameID = 1;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 해당 유닛 프리팹의 추가 데이터
        /// </summary>
        public new PrefabExtraData_LamiereUnit.PrefabExtraDataLamiereUnitRecord _PrefabExtraDataRecord;

#if !SERVER_DRIVE
        /// <summary>
        /// 유닛 메쉬를 제어하는 오브젝트
        /// </summary>
        // private UnitMeshChanger _MeshChangeObject;
#endif

        /// <summary>
        /// 해당 유닛의 구분 타입
        /// </summary>
        public Vocation Vocation;

        private int _ThisUnitNameID;
        
        public Dictionary<int, bool> _PassiveActive;
        public Dictionary<int, bool> _PassiveCooldown;
        private GameEventTimerHandlerWrapper _SpawnIntervalTimer;

        /// <summary>
        /// 펫 슬롯
        /// </summary>
        private UnitIKControl _IKControlObject;

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            //Debug.Log($"[{name}]캐릭터 생성 LV {_Main_SpecialStatusPreset.CurrentLevel}");
            // 해당 프로젝트의 모델 공통 규격인 Root 오브젝트를 찾아 AttachPoint에 등록한다.
            /*
            var lamiereModelCommonRootNode = _Transform.Find("Bip001");
            SetAttachPoint(UnitAttachPoint.RootNode, lamiereModelCommonRootNode);
            SetAttachPoint(UnitAttachPoint.ModelWrapper, lamiereModelCommonRootNode);
            */
            if (_PrefabExtraDataRecord.HasMeshChanger)
            {
#if !SERVER_DRIVE
                // UnitMeshChanger 초기화
                /*_MeshChangeObject = new UnitMeshChanger(this);
                SetAttachPoint(UnitAttachPoint.MainWeapon, _MeshChangeObject.UnitPartWrapperAffine(UnitPartModelTool.LamiereUnitPart.Weapon));*/
#endif
            }

#if UNITY_EDITOR
            _ActableObject.SetMaxJumpCount(1);
#endif

            _PassiveActive = new Dictionary<int, bool>();
            _PassiveCooldown = new Dictionary<int, bool>();
            
            /*_IKControlObject = new UnitIKControl(this);*/
        }

        public override void OnPooling()
        {
            base.OnPooling();
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
        }

        public override void OnLateUpdate(float p_DeltaTime)
        {
        }

        public override void OnTriggerCoolDownRateChanged(ControllerTool.CommandType p_CommandType, float p_ProgressRate01)
        {
            base.OnTriggerCoolDownRateChanged(p_CommandType, p_ProgressRate01);
        }

        public void OnTriggerCooldownOver(int p_Index)
        {
            _PassiveCooldown[p_Index] = false;
        }

        public void OnTriggerDurationOver(int p_Index)
        {
            _PassiveActive[p_Index] = false;
        }

        #endregion

        #region <Methods>
        
        protected override void InitPrefabExtraData()
        {
            base.InitPrefabExtraData();
            _PrefabExtraDataRecord = base._PrefabExtraDataRecord as PrefabExtraData_LamiereUnit.PrefabExtraDataLamiereUnitRecord;
            Vocation = _PrefabExtraDataRecord.Vocation;
        }

        #endregion
        // 퀵슬롯 관련 코드를 Unit, LamiereGameManager로부터 LamiereUnit으로 옮기기

    }
}