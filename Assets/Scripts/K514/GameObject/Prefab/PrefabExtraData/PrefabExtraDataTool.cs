using System;
using System.Collections.Generic;

namespace BlackAm
{
    /// <summary>
    /// 제네릭스 클래스 내부의 테이블 클래스를 추상화 하기 위한 브릿지 인터페이스
    /// </summary>
    public interface PrefabExtraDataTableBridge : ITableBase
    {
    }

    /// <summary>
    /// 제네릭스 클래스 내부의 레코드 클래스를 추상화 하기 위한 브릿지 인터페이스
    /// </summary>
    public interface PrefabExtraDataRecordBridge : ITableBaseRecord
    {
        /// <summary>
        /// 해당 레코드가 참조하는 프리팹은 초기화시에 해당 리스트 안의 타입을 컴포넌트로 가지게 된다.
        /// </summary>
        Type ExtraComponentType { get; }
            
        /// <summary>
        /// 해당 추가데이터를 기술하는 레코드 필드
        /// </summary>
        string ExtraDataDescription { get; }
    }
    
    /// <summary>
    /// 프리팹 추가 데이터의 공통적인 기능을 모아놓는 정적 클래스
    /// </summary>
    public static class PrefabExtraDataTool
    {
    }
    
    /// <summary>
    /// 프리팹 추가 데이터 레코드의 루트
    /// </summary>
    public class PrefabExtraDataRoot : MultiTableProxy<PrefabExtraDataRoot, int, PrefabExtraDataRoot.PrefabExtraDataType, PrefabExtraDataTableBridge, PrefabExtraDataRecordBridge>
    {
        #region <Enums>

        /// <summary>
        /// 여러 프리팹에 맞게 분화된 각 테이블 클래스를 구분하는 열거형 상수
        /// </summary>
        public enum PrefabExtraDataType
        {
            AutoMutton_Zero,
            AutoMutton_NonZero,
            AutoMutton_Linear,
            AutoMutton_Square,
            
            ProjectorSimple,
            ProjectorAnimation,
            ProjectorPetPet,
            
            Lamiere,
            Slave,
            
            VFX,
            ProjectileVFX,
            ProjectileVFX_VectorMap,
            ProjectileVFX_Kinematic,
            ProjectileVFX_TimeInterpolate,
            
            UI,
        }

        #endregion
    
        #region <Methods>
        
        protected override MultiTableIndexer<int, PrefabExtraDataType, PrefabExtraDataRecordBridge> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<PrefabExtraDataType, PrefabExtraDataRecordBridge>();
        }

        #endregion
    }
}