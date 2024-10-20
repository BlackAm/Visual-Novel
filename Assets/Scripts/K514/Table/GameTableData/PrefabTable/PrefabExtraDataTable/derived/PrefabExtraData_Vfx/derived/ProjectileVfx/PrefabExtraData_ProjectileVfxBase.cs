using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public abstract class PrefabExtraData_ProjectileVfxBase<M, T> : PrefabExtraData_VfxBase<M, T>
        where M : PrefabExtraData_ProjectileVfxBase<M, T>, new() 
        where T : PrefabExtraData_ProjectileVfxBase<M, T>.ProjectileVfxTableRecordBase, new()
    {
        public abstract class ProjectileVfxTableRecordBase : VfxTableRecordBase, ProjectileVfxExtraDataRecordBridge
        {
            /// <summary>
            /// 투사체 동작 플래그
            /// </summary>
            public ProjectileTool.ProjectileProgressFlag ProjectileProgressFlag { get; protected set; }
            
            /// <summary>
            /// 투사체 충돌 이벤트 플래그
            /// </summary>
            public ProjectileTool.ProjectileCollisionEventFlag ProjectileCollisionEventFlag { get; protected set; }
       
            /// <summary>
            /// 투사체 유닛 충돌 이벤트 플래그
            /// </summary>
            public ProjectileTool.ProjectileUnitCollisionEventFlag ProjectileUnitCollisionEventFlag { get; protected set; }

            /// <summary>
            /// 투사체 추적 방식 플래그
            /// </summary>
            public ProjectileTool.ProjectileTraceTargetEventFlag ProjectileTraceTargetEventFlag { get; protected set; }

            /// <summary>
            /// 지속 시간
            /// </summary>
            public float LifeSpan { get; protected set; }

            /// <summary>
            /// 파티클 충돌시 적용할 인수 튜플
            /// </summary>
            public (float t_Dampen01, float t_Bounce02, float t_LifeTimeLose01) CollisionFactorTuple { get; protected set; }

            /// <summary>
            /// 타격 판정 인덱스
            /// </summary>
            public int HitPresetIndex { get; protected set; }
            
            /// <summary>
            /// 배치 이벤트 인덱스
            /// </summary>
            public int DeployEventPresetIndex { get; protected set; }
            
            /// <summary>
            /// 투사체 기본 충돌 레이어 마스크
            /// </summary>
            public GameManager.GameLayerMaskType CollisionLayerMask { get; protected set; }

            /// <summary>
            /// 최대 충돌 이벤트 횟수
            /// </summary>
            public int CollisionEventMaxCount { get; protected set; }
            
            /// <summary>
            /// 한 유닛에 적용될 수 있는 이벤트 횟수 및 간격
            /// </summary>
            public (int t_Count, float t_Interval) SameUnitEventPreset { get; protected set; }

            /// <summary>
            /// 파티클에 운동이 적용되기까지 선딜레이
            /// </summary>
            public int ParticleControlPredelay { get; protected set; }
            
            /// <summary>
            /// 파티클 운동 선딜레이가 적용되는 타겟까지의 거리 하한
            /// 해당 거리 이하에서 파티클 선딜레이는 0이 된다.
            /// </summary>
            public float ParticleControlPredelayApplyLowerBoundDistance { get; protected set; }

            /// <summary>
            /// 타겟을 추적하는 시야 반각 내적값, 1에 가까울수록 같은 방향, 0에 가까울수록 수직방향, -1에 가까울수록 반대방향
            /// </summary>
            public float TraceHalfSightDot { get; protected set; }
            
            /// <summary>
            /// 투사체 생성시에 유닛 현재 포커스가 없는 경우, 적을 탐색할 범위
            /// </summary>
            public float FallbackSearchTargetDistance { get; protected set; }

            /// <summary>
            /// 투사체 생성시에 포커스가 너무 가까이에 있는지 검증하는 거리
            /// </summary>
            public float DangerCloseDistance { get; protected set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                if (CollisionLayerMask == default)
                {
                    CollisionLayerMask = GameManager.GameLayerMaskType.UnitLayerSet_Except_Corpse
                                         | GameManager.GameLayerMaskType.TerrainObstacleSet;
                }

                CollisionEventMaxCount = Mathf.Max(1, CollisionEventMaxCount);
                SameUnitEventPreset = (Mathf.Max(1, SameUnitEventPreset.t_Count), SameUnitEventPreset.t_Interval);
                
                ParticleControlPredelayApplyLowerBoundDistance = Mathf.Pow(ParticleControlPredelayApplyLowerBoundDistance, 2f);
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}