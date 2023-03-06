using UnityEngine;

namespace k514
{
    /// <summary>
    /// 제네릭스 클래스 내부의 테이블 클래스를 추상화 하기 위한 브릿지 인터페이스
    /// </summary>
    public interface PrefabModelDataTableBridge : ITableBase
    {
    }
    
    /// <summary>
    /// 제네릭스 클래스 내부의 레코드 클래스를 추상화 하기 위한 브릿지 인터페이스
    /// </summary>
    public interface PrefabModelDataRecordBridge : ITableBaseRecord
    {
        float PrefabModelScale { get; }
        string GetPrefabName();
    }
    
    /// <summary>
    /// 프리팹 모델 중, 유닛 모델의 추상화 인터페이스
    /// </summary>
    public interface UnitModelDataRecordBridge
    {
        int AttachPointIndex { get; }
        float FallbackRadius { get; }
        float FallbackHeights { get; }
        Vector3 FallbackCenterOffset { get; }
        UnitTool.UnitSkinType UnitSkinType { get; }
    }
    
    /// <summary>
    /// 프리팹 모델 데이터의 공통적인 기능을 모아놓는 정적 클래스
    /// </summary>
    public static class PrefabModelDataTool
    {
    }

    /// <summary>
    /// 프리팹 모델 데이터 레코드의 루트
    /// </summary>
    public class PrefabModelDataRoot : MultiTableProxy<PrefabModelDataRoot, int, PrefabModelDataRoot.PrefabModelDataType, PrefabModelDataTableBridge, PrefabModelDataRecordBridge>
    {
        #region <Enums>

        /// <summary>
        /// 프리팹 모델 데이터를 구분하기 위한 열거형 상수
        /// </summary>
        public enum PrefabModelDataType
        {
            AutoMutton,
            Projector,
        
            PlayerModel,
            ComputerModel,
            
            VFX,
            UI,
        }

        #endregion
    
        #region <Methods>

        protected override MultiTableIndexer<int, PrefabModelDataType, PrefabModelDataRecordBridge> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<PrefabModelDataType, PrefabModelDataRecordBridge>();
        }

        #endregion
    }
}