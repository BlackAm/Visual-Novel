using System.Collections.Generic;

namespace BlackAm
{
    public interface ProjectorExtraDataRecordBridge : PrefabExtraDataRecordBridge
    {
        string ExtraDataDescription { get; }
        
        /// <summary>
        /// 프로젝터가 래퍼로부터 배치되는 높이
        /// </summary>
        float HeightOffset { get; }
        
        /// <summary>
        /// 프로젝터의 투사 거리
        /// </summary>
        float ProjectDistance { get; }
        
        /// <summary>
        /// 투사 시간
        /// </summary>
        float ProjectTime { get; }
        
        /// <summary>
        /// 페이드 인 시간
        /// </summary>
        float FadeInTime { get; }
        
        /// <summary>
        /// 페이드 아웃 시간
        /// </summary>
        float FadeOutTime { get; }

        /// <summary>
        /// 직교 투영 스케일
        /// </summary>
        float ProjectSize { get; }
        
        /// <summary>
        /// 프로젝터 박스가 지형에 충돌하는 경우, 프로젝트를 수행하지 않는데
        /// 실제 박스에서 몇 %의 비율로 충돌검증을 실행할지 정하는 인수
        /// XZ값에 대해서만 스케일이 적용된다.
        /// </summary>
        float ProjectCollisionCheckRate { get; }
        
        /// <summary>
        /// 프로젝터가 무시할 레이어마스크
        /// </summary>
        List<GameManager.GameLayerMaskType> IgnoreLayerFlagList { get; }
    }

    public abstract class PrefabExtraData_ProjectorBase<M, T> : PrefabExtraDataIntTable<M, T>, PrefabExtraDataTableBridge where M : PrefabExtraData_ProjectorBase<M, T>, new() where T : PrefabExtraData_ProjectorBase<M, T>.PrefabExtraDataProjectorBaseRecord, new()
    {
        public abstract class PrefabExtraDataProjectorBaseRecord : PrefabExtraDataRecord, ProjectorExtraDataRecordBridge
        {
            public string ExtraDataDescription { get; protected set; }
            public float HeightOffset { get; protected set; }
            public float ProjectDistance { get; protected set; }
            public float ProjectTime { get; protected set; }
            public float FadeInTime { get; protected set; }
            public float FadeOutTime { get; protected set; }
            public float ProjectSize { get; protected set; }
            public float ProjectCollisionCheckRate { get; protected set; }
            public List<GameManager.GameLayerMaskType> IgnoreLayerFlagList { get; protected set; }
        }
    }
}