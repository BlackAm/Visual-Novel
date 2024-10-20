using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackAm
{
    public interface IDeployEventRecord
    {
        ObjectPooler<ObjectDeployEventRecord> ObjectDeployEventRecordPooler { get; }
    }

    public interface IDeployableSpawner
    {
        ObjectDeployTool.DeployableType DeployableType { get; }
        void Preload(int p_Index, int p_Count);
    }
    
    public interface IDeployee
    {
        ObjectDeployTool.DeployableType DeployableType { get; }
        void ApplyAffinePreset(TransformTool.AffineCachePreset p_DeployPreset, bool p_ForceSurface);
    }

    public interface IObjectDeployTableBridge : ITableBase
    {
    }

    public interface IObjectDeployTableRecordBridge : ITableBaseRecord
    {
        /// <summary>
        /// 배치할 위치에 장해물이 있는 경우 처리 타입
        /// </summary>
        ObjectDeployTool.ObjectDeploySurfaceDeployType ObjectDeploySurfaceDeployType { get; }

        ObjectDeployEventExtraPreset CalculateDeployAffine(ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset);
    }

    public static class ObjectDeployTool
    {
        #region <Fields>

        public static DeployableType[] _DeployableTypeEnumerator;

        #endregion

        #region <Enums>

        [Flags]
        public enum DeployableType
        {
            None = 0,
            
            /// <summary>
            /// 파티클 시스템을 포함하는 오브젝트
            /// </summary>
            VFX = 1 << 0,

            /// <summary>
            /// 운동 및 충돌을 가지는 파티클 시스템을 포함하는 오브젝트
            /// </summary>
            ProjectileVfx = 1 << 1,

            /// <summary>
            /// 오디오 소스를 포함하는 오브젝트
            /// </summary>
            SFX = 1 << 2,

            /// <summary>
            /// 프로젝터를 포함하는 오브젝트
            /// </summary>
            Projector = 1 << 3,

            /// <summary>
            /// Projector와 달리 TargetPivot에 스폰되는 프로젝터
            /// </summary>
            TargetProjector = 1 << 4,

            /// <summary>
            /// 유닛 계열 컴포넌트, 충돌체, 애니메이터, 길찾기 에이전트 등을 포함하는 오브젝트
            /// </summary>
            Unit = 1 << 5,

            /// <summary>
            /// 특정 아핀값을 기준으로 유닛을 필터링하고, 필터링된 유닛에게 타격 이벤트를 전달하는 이벤트
            /// </summary>
            HitFilter = 1 << 6,

            /// <summary>
            /// TargetPivot에 기준으로 하여 재귀적으로 다른 배치 이벤트로 이어나가는 이벤트
            /// </summary>
            RecursiveDeploy = 1 << 7,

            /// <summary>
            /// UI를 생성한다.
            /// </summary>
            UI = 1 << 8,
            
            /// <summary>
            /// 배치 이벤트 테이블을 참조하여 이벤트를 생성하는 이벤트
            /// </summary>
            ObjectDeployMap = 1 << 9,
            
            /// <summary>
            /// 오토머튼 오브젝트를 생성한다.
            /// </summary>
            AutoMutton = 1 << 10,

            /// <summary>
            /// 지정한 액션을 수행한 후 릴리스되는 유닛을 생성하는 이벤트
            /// </summary>
            Actor = 1 << 11,

            /// <summary>
            /// 비디오 오브젝트
            /// </summary>
            Video = 1 << 12,
            
            /// <summary>
            /// 기준 유닛에게 힘을 가한다.
            /// </summary>
            AddForce = 1 << 15,

            /// <summary>
            /// Pivot으로 선정된 유닛의, 현재 아핀값을 파라미터로 하는 특정 함수를 호출한다.
            /// </summary>
            InteractPivotUnit = 1 << 20,
            
            /// <summary>
            /// 액션을 수행하는 유닛에게 버프를 적용 시킨다.
            /// </summary>
            Buff = 1 << 25,
        }
        
        /// <summary>
        /// 배치 아핀정보를 연산할 때, 기준으로 사용할 좌표계
        /// </summary>
        public enum DeployAffineCoordinateType
        {
            /// <summary>
            /// 테이블의 배치값을 읽지 않고, 지정한 유닛의 위치/회전값을 참조하여 배치한다.
            /// </summary>
            None,
            
            /// <summary>
            /// 배치 아핀 연산시 넘겨받은 아핀값을 그대로 사용하여 배치한다.
            /// </summary>
            FreeAffine,
            
            /// <summary>
            /// 지정한 유닛이 현재 동작중인 애니메이션 모션의 시작지점의 좌표계를 기준으로 배치한다.
            /// </summary>
            MotionCached,
            
            /// <summary>
            /// 절대 좌표계 기준으로 배치한다.
            /// </summary>
            World,
            
            /// <summary>
            /// 기준 유닛으로부터 기준 유닛의 포커스 유닛까지의 유닛벡터를 기준삼아 배치한다.
            /// 포커스 유닛이 없던 경우, AttachedObject 타입을 Fallback으로 삼아 배치한다.
            /// </summary>
            FocusForward_FreeAffine,
              
            /// <summary>
            /// 배치 이벤트 개시시, TargetPivot이 선정되었다면 해당 방향을 기준으로 기저를 계산한다.
            /// 없다면 FreeAffine을 적용한다.
            /// </summary>
            TargetPivotForward_FreeAffine,
            
            /// <summary>
            /// 기준 유닛으로부터 기준 유닛의 포커스 유닛까지의 유닛벡터를 기준삼아 배치한다.
            /// 포커스 유닛이 없던 경우, MotionCached 타입을 Fallback으로 삼아 배치한다.
            /// </summary>
            FocusForward_MotionCached,
            
            /// <summary>
            /// 배치 아핀 값을 포커스 유닛으로 변경한다.
            /// 포커스 유닛이 없을 경우 FreeAffine을 적용한다.
            /// </summary>
            FocusAffine,
            
            /// <summary>
            /// 테스트 코드
            /// 배치 아핀 값을 AttachPoint MainWeapon으로 변경한다.
            /// </summary>
            MainWeapon,
        }

        [Flags]
        public enum ObjectDeployFlag
        {
            None = 0,
            
            /// <summary>
            /// 아핀 연산 시, 위치 좌표에 스케일값이 영향을 주지 않게 하는 플래그
            /// </summary>
            IgnoreScaledPosition = 1 << 0,
            
            /// <summary>
            /// 아핀 연산 시, 선정된 위치에 일정 구간 사이의 랜덤 벡터 값을 보정하게 하는 플래그
            /// </summary>
            RandomizePosition = 1 << 2,
                        
            /// <summary>
            /// 아핀 연산 시, 선정된 회전도에 일정 구간 사이의 랜덤 실수 값을 보정하게 하는 플래그
            /// </summary>
            RandomizeRotation = 1 << 3,
            
            /// <summary>
            /// 아핀 연산 시, 선정된 회전도에 일정 구간 사이의 랜덤 실수 값을 보정하게 하는 플래그
            /// </summary>         
            RandomizeScale = 1 << 4,
            
            /// <summary>
            /// 최초 아핀 연산 시, 선정된 위치에 일정 구간 사이의 랜덤 벡터 값을 보정하게 하는 플래그
            /// </summary>
            RandomizeStartPosition = 1 << 5,
                        
            /// <summary>
            /// 최초 아핀 연산 시, 선정된 회전도에 일정 구간 사이의 랜덤 실수 값을 보정하게 하는 플래그
            /// </summary>
            RandomizeStartRotation = 1 << 6,
            
            /// <summary>
            /// 최초 아핀 연산 시, 선정된 회전도에 일정 구간 사이의 랜덤 실수 값을 보정하게 하는 플래그
            /// </summary>         
            RandomizeStartScale = 1 << 7,
            
            /// <summary>
            /// 회전도 여부
            /// </summary>
            RotationOffset = 1 << 8,
                        
            /// <summary>
            /// 추가 회전도 여부
            /// </summary>
            AdditiveRotationOffset = 1 << 9,
            
            /// <summary>
            /// 초기 배치 위치를 기준 유닛의 높이 절반만큼 보정하게 하는 플래그
            /// </summary>
            CorrectUnitHalfHeight = 1 << 10,
        }

        [Flags]
        public enum ObjectDeploySurfaceDeployType
        {
            /// <summary>
            /// 지정한 위치에 프리팹을 생성한다.
            /// </summary>
            None = 0,
            
            /// <summary>
            /// 파라미터로 받은 좌표를 기준으로 연직 아래에 레이캐스트를 수행하여 가장 높은 위치의 표면에 프리팹을 생성한다.
            /// 만약 지정한 위치에 이미 컬라이더가 있는경우에도 충돌로 취급한다.
            /// </summary>
            ForceSurfaceUsingParameterVector =  1 << 0,
            
            /// <summary>
            /// 파라미터로 받은 XZ좌표 및 Y좌표는 높이가 무한대로 가정하고 연직 아래에 레이캐스트를 수행하여 가장 높은 위치의 표면에 프리팹을 생성한다.
            /// </summary>
            ForceSurface = 1 << 1,
            
            /// <summary>
            /// 충돌검증이 끝난 경우, 원본 파라미터의 y값 만큼 y좌표를 보정해준다.
            /// </summary>
            UsingOriginYOffset = 1 << 2,
            
            /// <summary>
            /// 충돌검증을 통해 표면을 발견하지 못한 경우, 해당 유닛을 생성하지 않는다.
            /// </summary>
            BreakDeployWhenNoCollision = 1 << 3,
            
            /// <summary>
            /// 충돌검증을 통해 배치 좌표가 변경된 경우, 해당 유닛을 생성하지 않는다.
            /// </summary>
            BreakDeployWhenPositionChanged = 1 << 4,
            
            HasCollisionCheck = ForceSurfaceUsingParameterVector | ForceSurface,
        }

        #endregion
        
        #region <Constructors>

        static ObjectDeployTool()
        {
            _DeployableTypeEnumerator = SystemTool.GetEnumEnumerator<DeployableType>(SystemTool.GetEnumeratorType.ExceptMaskNone);
        }

        public static void Init()
        {
        }

        #endregion
        
        #region <Method/AffineCachePreset>
        
        /// <summary>
        /// 지정한 위치를 기준으로 표면 레이캐스트를 수행하여 위치를 보정하는 메서드
        /// </summary>
        public static (bool, TransformTool.AffineCachePreset) CorrectAffinePreset(TransformTool.AffineCachePreset p_AffineCachePreset, ObjectDeploySurfaceDeployType p_DeployFlagMask)
        {
            return CorrectAffinePreset(GameManager.Obstacle_Terrain_UnitEC_LayerMask, p_AffineCachePreset, p_DeployFlagMask);
        }
        
        /// <summary>
        /// 지정한 위치를 기준으로 표면 레이캐스트를 수행하여 위치를 보정하는 메서드
        /// </summary>
        public static (bool, TransformTool.AffineCachePreset) CorrectAffinePreset(int p_LayerMask, TransformTool.AffineCachePreset p_AffineCachePreset, ObjectDeploySurfaceDeployType p_DeployFlagMask)
        {
            var (isValid, resultAffine) = (false, p_AffineCachePreset);
            var tryPosition = resultAffine.Position;
            // 충돌 검증을 수행하는 경우
            if (p_DeployFlagMask.HasAnyFlagExceptNone(ObjectDeploySurfaceDeployType.HasCollisionCheck))
            {
                var collidedPos = default(Vector3);
      
                // 지정한 위치에 스폰이 가능하다고 판단된 경우
                if (isValid)
                {
                    // Y 값 보정을 해야하는 경우
                    if (p_DeployFlagMask.HasAnyFlagExceptNone(ObjectDeploySurfaceDeployType.UsingOriginYOffset))
                    {
                        collidedPos += Vector3.up * tryPosition.y;
                    }

                    resultAffine.SetPosition(collidedPos);

                    if (p_DeployFlagMask.HasAnyFlagExceptNone(ObjectDeploySurfaceDeployType.BreakDeployWhenPositionChanged))
                    {
                        var heightOffset = tryPosition.y - collidedPos.y;
                        isValid = heightOffset.IsReachedZero(0.01f);
                    }
                }
                else
                {
                    isValid = !p_DeployFlagMask.HasAnyFlagExceptNone(ObjectDeploySurfaceDeployType.BreakDeployWhenNoCollision);
                }
            }
            // 충돌 검증을 수행하지 않는 경우
            else
            {
                isValid = true;
            }

            return (isValid, resultAffine);
        }

        #endregion
    }

    public class ObjectDeployDataRoot : MultiTableProxy<ObjectDeployDataRoot, int, ObjectDeployDataRoot.ObjectDeployType, IObjectDeployTableBridge, IObjectDeployTableRecordBridge>
    {
        #region <Enums>

        public enum ObjectDeployType
        {
            /// <summary>
            /// 항상 일정한 위치에 배치하는 타입
            /// </summary>
            ZeroConstant,
            
            /// <summary>
            /// 최초의 이벤트 좌표로부터 아핀값이 선형적으로 증가하는 배치 타입
            /// </summary>
            Linear,
            
            /// <summary>
            /// 지정한 두 좌표 사이를 선형 러프하는 배치 타입
            /// </summary>
            LerpLinear,
            
            /// <summary>
            /// 3차 베지어 곡선을 따라 배치하는 타입
            /// </summary>
            CBezier,
        }

        #endregion

        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            await base.OnCreated();
            ObjectDeployTool.Init();
        }

        #endregion
        
        #region <Methods>

        protected override MultiTableIndexer<int, ObjectDeployType, IObjectDeployTableRecordBridge> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<ObjectDeployType, IObjectDeployTableRecordBridge>();
        }

        #endregion
    }
}