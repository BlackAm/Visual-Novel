namespace k514
{
    public interface VfxExtraDataRecordBridge : PrefabExtraDataRecordBridge
    {
        /// <summary>
        /// 해당 플래그가 활성화된 경우, Vfx는 배치 변환에 의해 회전값이 변하지 않는다.
        /// </summary>
        bool FixedRotationFlag { get; }
        
        /// <summary>
        /// 이펙트 배속율
        /// </summary>
        float SimulateSpeedFactor{ get; }
    }

    public interface ProjectileVfxExtraDataRecordBridge : VfxExtraDataRecordBridge
    {
        /// <summary>
        /// 투사체 동작 플래그
        /// </summary>
        ProjectileTool.ProjectileProgressFlag ProjectileProgressFlag { get; }

        /// <summary>
        /// 투사체 충돌 이벤트 플래그
        /// </summary>
        ProjectileTool.ProjectileCollisionEventFlag ProjectileCollisionEventFlag { get; }
               
        /// <summary>
        /// 투사체 유닛 충돌 이벤트 플래그
        /// </summary>
        ProjectileTool.ProjectileUnitCollisionEventFlag ProjectileUnitCollisionEventFlag { get; }

        /// <summary>
        /// 투사체 추적 방식 플래그
        /// </summary>
        ProjectileTool.ProjectileTraceTargetEventFlag ProjectileTraceTargetEventFlag { get; }
        
        /// <summary>
        /// 지속 시간
        /// </summary>
        float LifeSpan { get; }

        /// <summary>
        /// 파티클 충돌시 적용할 인수 튜플
        /// </summary>
        (float t_Dampen01, float t_Bounce02, float t_LifeTimeLose01) CollisionFactorTuple { get; }

        /// <summary>
        /// 타격 판정 인덱스
        /// </summary>
        int HitPresetIndex { get; }
            
        /// <summary>
        /// 배치 이벤트 인덱스
        /// </summary>
        int DeployEventPresetIndex { get; }
            
        /// <summary>
        /// 투사체 기본 충돌 레이어 마스크
        /// </summary>
        GameManager.GameLayerMaskType CollisionLayerMask { get; }

        /// <summary>
        /// 최대 충돌 이벤트 횟수
        /// </summary>
        int CollisionEventMaxCount { get; }

        /// <summary>
        /// 한 유닛에 적용될 수 있는 이벤트 횟수 및 간격
        /// </summary>
        (int t_Count, float t_Interval) SameUnitEventPreset { get; }

        /// <summary>
        /// 파티클에 운동이 적용되기까지 선딜레이
        /// </summary>
        int ParticleControlPredelay { get; }
            
        /// <summary>
        /// 파티클 운동 선딜레이가 적용되는 타겟까지의 거리 하한
        /// 해당 거리 이하에서 파티클 선딜레이는 0이 된다.
        /// </summary>
        float ParticleControlPredelayApplyLowerBoundDistance { get; }

        /// <summary>
        /// 타겟을 추적하는 시야 반각 내적값, 1에 가까울수록 같은 방향, 0에 가까울수록 수직방향, -1에 가까울수록 반대방향
        /// </summary>
        float TraceHalfSightDot { get; }
            
        /// <summary>
        /// 투사체 생성시에 유닛 현재 포커스가 없는 경우, 적을 탐색할 범위
        /// </summary>
        float FallbackSearchTargetDistance { get; }

        /// <summary>
        /// 투사체 생성시에 포커스가 너무 가까이에 있는지 검증하는 거리
        /// </summary>
        float DangerCloseDistance { get; }
    }
    
    public static class VisualEffectTool
    {
        
    }
}