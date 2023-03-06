using System;
using UnityEngine;

namespace k514
{
    public static class PrefabInstanceTool
    {
        #region <Enums>
        
        [Flags]
        public enum MasterNodeRelateType
        {
            /// <summary>
            /// 종속하는 주인 유닛이 없음
            /// </summary>
            None = 0,
            
            /// <summary>
            /// 파티에 종속됨
            /// </summary>
            Party = 1 << 0,
            
            /// <summary>
            /// 어떤 유닛을 기준으로 생성됨
            /// </summary>
            Creation = 1 << 1,
            
            /// <summary>
            /// 어떤 유닛에 귀속되어 있음
            /// </summary>
            Slave = 1 << 2,
        }
        
        [Flags]
        public enum FocusNodeRelateType
        {
            /// <summary>
            /// 포커싱 없음
            /// </summary>
            None = 0,

            /// <summary>
            /// 선정된 유닛을 추적함
            /// </summary>
            Tracing = 1 << 0,
            
            /// <summary>
            /// 사고 모듈에 의해 선정된 적 유닛
            /// </summary>
            Enemy = 1 << 1,
            
            /// <summary>
            /// 사고 모듈에 의해 선정된 적 유닛
            /// Enemy 타입과 다르게 해당 타입으로 선정된 포커스는 사고 모듈 상태의 영향을 받아 갱신되거나, 리셋되지 않는다.
            /// </summary>
            ForceEnemy = 1 << 2,
        }
        
        #endregion
        
        #region <Methods>

        public static bool IsValid(this PrefabInstance p_TargetUnit)
        {
            return !ReferenceEquals(null, p_TargetUnit) && p_TargetUnit.PoolState == PoolState.Actived;
        }
        
        #endregion
    }
}