using System;

namespace BlackAm
{
    public static class ProjectileTool
    {
        #region <Fields>

        #endregion

        #region <Enums>

        [Flags]
        public enum ProjectileProgressFlag
        {
            None = 0,
            
            /// <summary>
            /// 투사체 충돌 이펙트를 없앤다.
            /// </summary>
            BlockCollisionSubEmit = 1 << 0,
            
            /// <summary>
            /// 투사체 파기 이펙트를 없앤다.
            /// </summary>
            BlockTerminateSubEmit = 1 << 1,

            /// <summary>
            /// 타겟이 없는 경우, 투사체 자체를 생성하지 않는다.
            /// </summary>
            BlockShotWhenHasNoTarget = 1 << 2,

            /// <summary>
            /// 해당 플래그가 참일 때, 다음과 같은 동작을 수행한다.
            /// 1. 추적 대상을 갱신하지 않는다.
            /// 2. 추적 대상 이외의 유닛과 충돌해도 이벤트를 발생시키지 않는다.
            /// 3. 수명이 다하여 파기됬을 때, 추적대상이 유효하다면 충돌하지 않았어도 이벤트를 발동시킨다.
            /// 4. 유닛 추적 하한 거리 및 유닛 추적 선딜레이가 0이된다.
            /// </summary>
            Hrunting = 1 << 8,

            /// <summary>
            /// 추적 대상이 존재하고, 투사체가 해당 대상과 너무 가까운 거리에서 생성된 경우
            /// 파티클을 생성하기 전에 투사체를 파기하고 이벤트를 처리한다.
            /// </summary>
            DangerClose = 1<< 9,
        }

        [Flags]
        public enum ProjectileCollisionEventFlag
        {
            None = 0,
            
            /// <summary>
            /// 해당 플래그가 참일 때, 장해물이나 지형 레이어를 가지는 컬라이더와 충돌시, 해당 투사체를 제거한다.
            /// 그 외의 경우에는 파티클 시스템의 설정을 따른다.
            /// </summary>
            BurstWhenBlocked_Terrain_Obstacle = 1 << 0,
            
            /// <summary>
            /// 해당 플래그가 참일 때, 유닛 레이어를 가지는 컬라이더와 충돌시, 해당 투사체를 제거한다.
            /// 그 외의 경우에는 파티클 시스템의 설정을 따른다.
            /// </summary>
            BurstWhenBlocked_Unit = 1 << 1,
            
            /// <summary>
            /// 해당 플래그가 참일 때, 충돌한 유닛에게 데미지를 준다.
            /// </summary>
            TossDamageWhenBlocked_Unit = 1 << 2,
            
                                         
            /// <summary>
            /// 해당 플래그가 참일 때, 장해물이나 지형 레이어를 가지는 컬라이더와 충돌시, 지정한 배치 이벤트를 수행한다.
            /// </summary>
            DeployEventWhenBlocked_Terrain_Obstacle = 1 << 5,
            
            /// <summary>
            /// 해당 플래그가 참일 때, 유닛 레이어를 가지는 컬라이더와 충돌시, 지정한 배치 이벤트를 수행한다.
            /// </summary>
            DeployEventWhenBlocked_Unit = 1 << 6,
                        
            /// <summary>
            /// 해당 플래그가 참일 때, 투사체 수명 종료시, 지정한 배치 이벤트를 수행한다.
            /// </summary>
            DeployEventWhenLifeOver = 1 << 7,
            
            
            /// <summary>
            /// 해당 플래그가 참일 때, 장해물이나 지형 레이어를 가지는 컬라이더와 충돌시, 지정한 필터 이벤트를 수행한다.
            /// </summary>
            FilterEventWhenBlocked_Terrain_Obstacle = 1 << 10,
            
            /// <summary>
            /// 해당 플래그가 참일 때, 유닛 레이어를 가지는 컬라이더와 충돌시, 지정한 필터 이벤트를 수행한다.
            /// </summary>
            FilterEventWhenBlocked_Unit = 1 << 11,
                        
            /// <summary>
            /// 해당 플래그가 참일 때, 투사체 수명 종료시, 지정한 필터 이벤트를 수행한다.
            /// </summary>
            FilterEventWhenLifeOver = 1 << 12,
 
            BurstWhenBlocked = BurstWhenBlocked_Terrain_Obstacle | BurstWhenBlocked_Unit,
            DeployEventWhenBlocked = DeployEventWhenBlocked_Terrain_Obstacle | DeployEventWhenBlocked_Unit,
            DeployEvent = DeployEventWhenBlocked | DeployEventWhenLifeOver,
            FilterEventWhenBlocked = FilterEventWhenBlocked_Terrain_Obstacle | FilterEventWhenBlocked_Unit,
            FilterEvent = FilterEventWhenBlocked | FilterEventWhenLifeOver,
        }

        public static ProjectileCollisionEventFlag[] ProjectileCollisionEventFlagEnumerator;

        [Flags]
        public enum ProjectileUnitCollisionEventFlag
        {
            None = 0,
            
            /// <summary>
            /// 해당 플래그가 참일 때, 파티클 시스템의 유니티 물리엔진을 따라 충돌 이벤트를 검증하고 처리한다.
            /// </summary>
            DriveUnityPhysicsEvent = 1 << 0,
            
            /// <summary>
            /// 해당 플래그가 참일 때, 파티클 오브젝트의 주변을 검색하여 충돌 이벤트를 검증하고 처리한다.
            /// </summary>
            DriveUnitFilterPhysicsEvent = 1 << 1,
        }
        
        public static ProjectileUnitCollisionEventFlag[] ProjectileUnitCollisionEventFlagEnumerator;

        [Flags]
        public enum ProjectileTraceTargetEventFlag
        {
            None = 0,
            
            /// <summary>
            /// 투사체와 타겟간의 거리를 측정하여, 적이 일정 거리 이내 및 각도 안에 있는 경우
            /// 운동 방향과 타겟 방향을 보간하여 타겟을 추적하게 한다.
            /// </summary>
            TraceTarget = 1 << 0,
            
            /// <summary>
            /// 투사체 생성시, 타겟 정보를 입력하지 않은 경우
            /// 생성된 위치를 기준으로 주변적을 탐색하여 타겟을 정한다.
            /// </summary>
            UsingFallbackTraceTargeting = 1 << 1,
            
            /// <summary>
            /// 투사체 운동중에, 추적대상이 추적불능 상태가 되는 경우
            /// 해당 위치를 기준으로 주변적을 탐색하여 다음 타겟을 정한다.
            /// </summary>
            UpdateFallbackTraceTargeting = 1 << 2,
            
            /// <summary>
            /// UsingFallbackTraceTargeting 이후, 파티클시스템을 타겟을 바라보도록 회전시킨다.
            /// </summary>
            SetVfxAffineTowardTarget = 1 << 3,
            
            /// <summary>
            /// 투사체가 유닛과 충돌하여 이벤트처리를 할 때, 이벤트 처리에 필요한 아핀 정보를
            /// 투사체의 충돌 위치가 아닌 피격 유닛의 위치를 사용하게 한다.
            /// </summary>
            CorrectParticlePositionWhenBlocked_Unit = 1 << 5,
            
            /// <summary>
            /// 추적 선딜레이 종료시, 추적 대상이 유효하다면 해당 위치로 투사체의 방향을 꺾는다.
            /// </summary>
            CorrectDirectionWhenFirstDelayOver = 1 << 6,
            
            FallbackTraceTargeting = UsingFallbackTraceTargeting | UpdateFallbackTraceTargeting,
        }

        [Flags]
        public enum ProjectileEventHandleFlag
        {
            None = 0,
            
            FindCollidedUnit = 1 << 0,
            UpdateCollisionEvent = 1 << 1,
            
            HitFilter = 1 << 4,
            DeployEvent = 1 << 5,
            TossDamage = 1 << 6,
            
            RemoveVfx = 1 << 15,
            
            UpdateUnitCollisionEvent = FindCollidedUnit | UpdateCollisionEvent,
        }
        
        public static ProjectileEventHandleFlag[] ProjectileEventHandleFlagEnumerator;

        #endregion

        #region <Constructor>

        static ProjectileTool()
        {
            ProjectileCollisionEventFlagEnumerator =
                SystemTool.GetEnumEnumerator<ProjectileCollisionEventFlag>(SystemTool.GetEnumeratorType.ExceptNone);
            ProjectileUnitCollisionEventFlagEnumerator =
                SystemTool.GetEnumEnumerator<ProjectileUnitCollisionEventFlag>(SystemTool.GetEnumeratorType.ExceptNone);
            ProjectileEventHandleFlagEnumerator =
                SystemTool.GetEnumEnumerator<ProjectileEventHandleFlag>(SystemTool.GetEnumeratorType.ExceptNone);
        }

        #endregion
    }
}