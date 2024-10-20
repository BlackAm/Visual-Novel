using System.Collections.Generic;

namespace BlackAm
{
    public interface UnitExtraDataRecordBridge : PrefabExtraDataRecordBridge
    {
        /// <summary>
        /// 유닛의 기본 레벨
        /// </summary>
        int Level { get; }

        /// <summary>
        /// 해당 프리팹에 등록된 모션 모듈 인덱스
        /// </summary>
        List<int> AnimationPresetIdList { get; }
        
        /// <summary>
        /// 해당 프리팹에 등록된 물리 연산 모듈 인덱스
        /// </summary>
        List<int> PhysicsPresetIdList { get; }

        /// <summary>
        /// 해당 프리팹에 등록된 사고 회로 모듈 인덱스
        /// </summary>
        List<int> MindPresetIdList { get; }

        /// <summary>
        /// 해당 유닛의 스탯 id
        /// </summary>
        int UnitInfoPresetId { get; }
        
        /// <summary>
        /// 해당 유닛의 액션 타입에 적용할 유닛액션 모듈 인덱스
        /// </summary>
        List<int> UnitActionRecordIdList { get; }
        
        /// <summary>
        /// 해당 프리팹에 등록된 랜더러 모듈 인덱스
        /// </summary>
        List<int> RenderPresetIdList { get; }
        
        /// <summary>
        /// 해당 프리팹에 등록된 역할 모듈 인덱스
        /// </summary>
        List<int> RolePresetIdList { get; }
       
        /// <summary>
        /// 해당 프리팹에 등록될 페이즈 인덱스 리스트
        /// </summary>
        List<int> PhaseRecordIndex { get; }
    }

    /// <summary>
    /// 기본 유닛이 가져야할 데이터를 포함하는 추상 테이블 클래스
    /// </summary>
    public abstract class PrefabExtraData_UnitBase<M, T> : PrefabExtraDataIntTable<M, T>, PrefabExtraDataTableBridge where M : PrefabExtraData_UnitBase<M, T>, new() where T : PrefabExtraData_UnitBase<M, T>.PrefabExtraDataUnitBaseRecord, new()
    {
        public abstract class PrefabExtraDataUnitBaseRecord : PrefabExtraDataRecord, UnitExtraDataRecordBridge
        {
            public int Level { get; protected set; }
            public List<int> AnimationPresetIdList { get; protected set; }
            public List<int> PhysicsPresetIdList { get; protected set; }
            public List<int> MindPresetIdList { get; protected set;  }
            public int UnitInfoPresetId { get; protected set;  }
            public List<int> UnitActionRecordIdList { get; protected set; }
            public List<int> RenderPresetIdList { get; protected set; }
            public List<int> RolePresetIdList { get; protected set; }
            public List<int> PhaseRecordIndex { get; protected set; }
        }
    }
}