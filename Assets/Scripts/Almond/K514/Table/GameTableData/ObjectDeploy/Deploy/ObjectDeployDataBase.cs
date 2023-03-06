using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 오브젝트 배치 이벤트의 기저 테이블 클래스
    /// </summary>
    public abstract class ObjectDeployDataBase<M, T> : MultiTableBase<M, int, T, ObjectDeployDataRoot.ObjectDeployType, IObjectDeployTableRecordBridge>, IObjectDeployTableBridge
        where M : ObjectDeployDataBase<M, T>, new() 
        where T : ObjectDeployDataBase<M, T>.DeployTableRecordBase, new()
    {
        public abstract class DeployTableRecordBase : GameTableRecordBase, IObjectDeployTableRecordBridge
        {
            /// <summary>
            /// 아핀 연산에 사용할 기저 벡터를 연산할 좌표계 타입
            /// </summary>
            public ObjectDeployTool.DeployAffineCoordinateType CoordType;

            /// <summary>
            /// 배치 연산에 적용할 추가 연산을 표시하는 플래그 마스크
            /// </summary>
            public ObjectDeployTool.ObjectDeployFlag ObjectDeployFlagMask;

            /// <summary>
            /// 배치할 위치에 장해물이 있는 경우 처리 타입
            /// </summary>
            public ObjectDeployTool.ObjectDeploySurfaceDeployType ObjectDeploySurfaceDeployType { get; protected set; }

            /// <summary>
            /// 최초 배치 시점에 더해지는 좌표 값
            /// </summary>
            public Vector3 StartPositionOffset;
            
            /// <summary>
            /// 최초 배치 시점에 더해지는 회전도 값
            /// [x축 기준 회전, y축 기준 회전, z축 기준 회전] 값.(왼손좌표계 기준)
            /// </summary>
            public Vector3 StartRotationOffset;
            
            /// <summary>
            /// 최초 배치 시점에 회전 변환 이후에 더해지는 좌표 값
            /// </summary>
            public Vector3 StartPositionRotatedOffset;
            
            /// <summary>
            /// 최초 배치 시점에 더해지는 스케일 값
            /// 배치할 오브젝트나 이벤트에도 적용된다.
            /// </summary>
            public float StartScaleOffset;

            /// <summary>
            /// 플래그 활성화 시, 선정된 위치에 해당 구간 사이의 난수 값을 원소로 하는 벡터값이 최초에 한번만 보정된다.
            /// </summary>
            public (CustomMath.XYZType, float, float) RandomizeStartPositionRange;
            
            /// <summary>
            /// 플래그 활성화 시, 선정된 회전 각에 해당 구간 사이의 난수 값을 원소로 하는 벡터값이 최초에 한번만 보정된다.
            /// </summary>
            public (CustomMath.XYZType, float, float) RandomizeStartRotationRange;
            
            /// <summary>
            /// 플래그 활성화 시, 스케일 값에 해당 구간 사이의 난수 값을 원소로 하는 벡터값이 최초에 한번만 보정된다.
            /// </summary>
            public (float, float) RandomizeStartScaleRange;

            /// <summary>
            /// 배치 이벤트의 오토타겟팅 모드에서 대상 유닛이 없는 경우 대신 사용할 좌표 오프셋
            /// </summary>
            public Vector3 FallBackPositionOffset;
            
            /// <summary>
            /// xml 파일로부터 레코드 인스턴스가 초기화되었을 때, 처리할 작업을 기술하는 콜백
            /// </summary>
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                if (RandomizeStartPositionRange != default)
                {
                    ObjectDeployFlagMask.AddFlag(ObjectDeployTool.ObjectDeployFlag.RandomizeStartPosition);
                }
                if (RandomizeStartRotationRange != default)
                {
                    ObjectDeployFlagMask.AddFlag(ObjectDeployTool.ObjectDeployFlag.RandomizeStartRotation);
                }
                if (RandomizeStartScaleRange != default)
                {
                    ObjectDeployFlagMask.AddFlag(ObjectDeployTool.ObjectDeployFlag.RandomizeStartScale);
                }
                if (StartRotationOffset != default)
                {
                    ObjectDeployFlagMask.AddFlag(ObjectDeployTool.ObjectDeployFlag.RotationOffset);
                }
            }

            /// <summary>
            /// 기재된 레코드를 통해 아핀연산을 수행하여 리턴하는 메서드
            /// </summary>
            public ObjectDeployEventExtraPreset CalculateDeployAffine(ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset)
            {
                // 현재 진행중인 배치 이벤트의 인덱스 값을 갱신시켜준다.
                var lastCalculatedDeployIndex = p_ObjectDeployEventExtraPreset.DeployEventIndex;
                p_ObjectDeployEventExtraPreset.DeployEventIndex = KEY;

                // 배치 인덱스가 변경된 경우에 최초 진입으로 취급해준다.
                p_ObjectDeployEventExtraPreset._IsEnterFirstCalculate = lastCalculatedDeployIndex != KEY;

                if (p_ObjectDeployEventExtraPreset._IsEnterFirstCalculate)
                {
                    // 현재 진행중인 배치 이벤트가 러프 이벤트인지 갱신시켜준다.
                    var tryLabelType = GetInstanceUnSafe.GetThisLabelType();
                    p_ObjectDeployEventExtraPreset._IsLerpAffine =
                        tryLabelType == ObjectDeployDataRoot.ObjectDeployType.LerpLinear ||
                        tryLabelType == ObjectDeployDataRoot.ObjectDeployType.CBezier;
                    
                    p_ObjectDeployEventExtraPreset.OnEntryFirstCalculation();
                    return CalculateDeployAffineFirst(p_ObjectDeployEventExtraPreset);
                }
                else
                {
                    return CalculateDeployAffineSequence(p_ObjectDeployEventExtraPreset);
                }
            }

            /// <summary>
            /// 해당 아핀연산이 최초 연산인 경우
            /// </summary>
            protected abstract ObjectDeployEventExtraPreset CalculateDeployAffineFirst(ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset);

            /// <summary>
            /// 해당 아핀연산이 두번째 이벤트 프레임 이후의 연산인 경우
            /// </summary>
            protected abstract ObjectDeployEventExtraPreset CalculateDeployAffineSequence(ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset);
        }

        public override MultiTableIndexer<int, ObjectDeployDataRoot.ObjectDeployType, IObjectDeployTableRecordBridge> GetMultiGameIndex()
        {
            return ObjectDeployDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}