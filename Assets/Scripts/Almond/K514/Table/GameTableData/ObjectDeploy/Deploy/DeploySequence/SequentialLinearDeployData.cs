using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 선형 아핀 배치
    /// 배치 이벤트의 진행에 따라 아핀값이 선형적으로 증감하는 배치 이벤트를 기술하는 테이블 클래스
    /// </summary>
    public class SequentialLinearDeployData : ObjectDeployDataBase<SequentialLinearDeployData, SequentialLinearDeployData.TableRecord>
    {
        public class TableRecord : DeployTableRecordBase
        {
            /// <summary>
            /// 추가 좌표 값
            /// </summary>
            public Vector3 AdditivePositionOffset;
            
            /// <summary>
            /// 추가 회전각도 벡터 값
            /// </summary>
            public Vector3 AdditiveRotationOffset;
            
            /// <summary>
            /// 추가 스케일 값
            /// </summary>
            public float AdditiveScaleOffset;
                      
            /// <summary>
            /// 추가 랜덤 좌표 값
            /// </summary>
            public (CustomMath.XYZType, float, float) RandomizePositionRange;
            
            /// <summary>
            /// 추가 랜덤 회전각도 값
            /// </summary>
            public (CustomMath.XYZType, float, float) RandomizeRotationRange;
            
            /// <summary>
            /// 추가 랜덤 스케일 값
            /// </summary>
            public (float, float) RandomizeScaleRange;
            
            /// <summary>
            /// xml 파일로부터 레코드 인스턴스가 초기화되었을 때, 처리할 작업을 기술하는 콜백
            /// </summary>
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                if (AdditiveRotationOffset != default)
                {
                    ObjectDeployFlagMask.AddFlag(ObjectDeployTool.ObjectDeployFlag.AdditiveRotationOffset);
                }
                if (RandomizePositionRange != default)
                {
                    ObjectDeployFlagMask.AddFlag(ObjectDeployTool.ObjectDeployFlag.RandomizePosition);
                }
                if (RandomizeRotationRange != default)
                {
                    ObjectDeployFlagMask.AddFlag(ObjectDeployTool.ObjectDeployFlag.RandomizeRotation);
                }
                if (RandomizeScaleRange != default)
                {
                    ObjectDeployFlagMask.AddFlag(ObjectDeployTool.ObjectDeployFlag.RandomizeScale);
                }
            }
            
            protected override ObjectDeployEventExtraPreset CalculateDeployAffineFirst(ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset)
            {
                #region <InitAffineValue>
                
                // 테이블 레코드 값으로 각 아핀 값을 초기화한다.
                var pivotUnit = p_ObjectDeployEventExtraPreset._MasterNode;
                var positionOffset = StartPositionOffset;
                var rotationOffset = StartRotationOffset;
                var rotatedPositionOffset = StartPositionRotatedOffset;
                var hasRotate = default(bool);
                var scaleOffset = StartScaleOffset;
                var prevAffine = p_ObjectDeployEventExtraPreset.StartPivot;

                // 테이블 레코드의 플래그 상태에 따라 각 아핀 값을 초기화한다.
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.CorrectUnitHalfHeight))
                {
                    positionOffset += pivotUnit.GetNonScaledHeight(0.5f) * Vector3.up;
                }
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.RotationOffset)
                || ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.AdditiveRotationOffset))
                {
                    hasRotate = true;
                }
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.RandomizeStartPosition))
                {
                    positionOffset += CustomMath.RandomVector(RandomizeStartPositionRange);
                }
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.RandomizeStartRotation))
                {
                    rotationOffset += CustomMath.RandomVector(RandomizeStartRotationRange);
                    hasRotate = true;
                }
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.RandomizeStartScale))
                {
                    scaleOffset += RandomizeStartScaleRange.GetRandom();
                }
     
                #endregion
                
                #region <CalculateBasis>
                
                switch (CoordType)
                {
                    case ObjectDeployTool.DeployAffineCoordinateType.FreeAffine:
                        break;
                    case ObjectDeployTool.DeployAffineCoordinateType.MotionCached:
                        var motionCached = pivotUnit._AnimationObject.CachedMasterNodeUV;
                        prevAffine.SetBasis(motionCached);
                        break;
                    case ObjectDeployTool.DeployAffineCoordinateType.World:
                        prevAffine.SetBasis();
                        break;
                    case ObjectDeployTool.DeployAffineCoordinateType.FocusForward_FreeAffine:
                        if (pivotUnit.FocusNode)
                        {
                            var tryForward =
                                pivotUnit._Transform.GetDirectionUnitVectorTo((Vector3) pivotUnit.FocusNode);
                            var tryUp = pivotUnit._Transform.up;
                            var tryRight = Vector3.Cross(tryUp, tryForward);
                            prevAffine.SetBasis(tryRight, tryUp, tryForward);
                        }
                        else
                        {
                            goto case ObjectDeployTool.DeployAffineCoordinateType.FreeAffine;
                        }
                        break;
                    case ObjectDeployTool.DeployAffineCoordinateType.TargetPivotForward_FreeAffine:
                        var rangeTarget = p_ObjectDeployEventExtraPreset.RangeTarget;
                        if (rangeTarget.t_Valid)
                        {
                            var targetUnit = rangeTarget.t_Filtered;
                            var tryForward =
                                pivotUnit._Transform.GetDirectionUnitVectorTo(targetUnit._Transform);
                            var tryUp = pivotUnit._Transform.up;
                            var tryRight = Vector3.Cross(tryUp, tryForward);
                            prevAffine.SetBasis(tryRight, tryUp, tryForward);
                        }
                        else
                        {
                            goto case ObjectDeployTool.DeployAffineCoordinateType.FreeAffine;
                        }
                        break;
                    case ObjectDeployTool.DeployAffineCoordinateType.FocusForward_MotionCached:
                        if (pivotUnit.FocusNode)
                        {
                            var tryForward =
                                pivotUnit._Transform.GetDirectionUnitVectorTo((Vector3) pivotUnit.FocusNode);
                            var tryUp = pivotUnit._Transform.up;
                            var tryRight = Vector3.Cross(tryUp, tryForward);
                            prevAffine.SetBasis(tryRight, tryUp, tryForward);
                        }
                        else
                        {
                            goto case ObjectDeployTool.DeployAffineCoordinateType.MotionCached;
                        }
                        break;
                    case ObjectDeployTool.DeployAffineCoordinateType.FocusAffine:
                        if (pivotUnit.FocusNode)
                        {
                            var tryForward = pivotUnit._Transform.GetDirectionUnitVectorTo((Vector3) pivotUnit.FocusNode);
                            var tryUp = pivotUnit._Transform.up;
                            var tryRight = Vector3.Cross(tryUp, tryForward);
                            prevAffine.SetBasis(tryRight, tryUp, tryForward);
                            prevAffine.SetPosition((Vector3) pivotUnit.FocusNode);
                        }
                        else
                        {
                            goto case ObjectDeployTool.DeployAffineCoordinateType.FreeAffine;
                        }

                        break;
                    case ObjectDeployTool.DeployAffineCoordinateType.MainWeapon:
                        if (pivotUnit.GetAttachPoint(Unit.UnitAttachPoint.MainWeapon) != pivotUnit._Transform)
                        {
                            prevAffine.SetBasis();
                            p_ObjectDeployEventExtraPreset._IsFoucsOnMainWeapon = true;
                        }
                        else
                        {
                            goto case ObjectDeployTool.DeployAffineCoordinateType.MotionCached;
                        }

                        break;
                    case ObjectDeployTool.DeployAffineCoordinateType.None:
                    default:
                        prevAffine.SetBasis(pivotUnit);
                        break;
                }
                
                #endregion
                
                #region <CalculateResult>
                
                // 스케일, 평행이동, 회전, 회전된 평행이동 순으로 변환을 적용한다.
                var result = prevAffine;
                result.AddScaleFactor(scaleOffset);
                result.AddLocalPositionOffset(positionOffset, true);

                if (hasRotate)
                {
                    result.Rotate(rotationOffset);
                }
                
                result.AddLocalPositionOffset(rotatedPositionOffset, true);

                #endregion
                                                
                #region <CalulatePivot>

                if (p_ObjectDeployEventExtraPreset._UsingFallbackTargetPositionFlag)
                {
                    var fallBackPosition = prevAffine.TransformPosition(FallBackPositionOffset);
                    p_ObjectDeployEventExtraPreset.SetTargetPivot(fallBackPosition);
                }

                p_ObjectDeployEventExtraPreset.SetStartPivot(result, true);

                #endregion
                
                return p_ObjectDeployEventExtraPreset;
            }
                     
            protected override ObjectDeployEventExtraPreset CalculateDeployAffineSequence(ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset)
            {
                #region <InitAffineValue>
                
                // 테이블 레코드 값으로 각 아핀 값을 초기화한다.
                var positionOffset = AdditivePositionOffset;
                var rotationOffset = AdditiveRotationOffset;
                var scaleOffset = AdditiveScaleOffset;
                var hasIgnoreScaledPositionFlag = default(bool);
                var hasRotate = default(bool);
                var prevAffine = p_ObjectDeployEventExtraPreset.CurrentPivot;
 
                // 테이블 레코드의 플래그 상태에 따라 각 아핀 값을 초기화한다.
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.IgnoreScaledPosition))
                {
                    hasIgnoreScaledPositionFlag = true;
                }
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.AdditiveRotationOffset))
                {
                    hasRotate = true;
                }
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.RandomizePosition))
                {
                    positionOffset += CustomMath.RandomVector(RandomizePositionRange);
                }
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.RandomizeRotation))
                {
                    rotationOffset += CustomMath.RandomVector(RandomizeRotationRange);
                    hasRotate = true;
                }
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.RandomizeScale))
                {
                    scaleOffset += RandomizeScaleRange.GetRandom();
                }
                
                #endregion
                
                #region <CalculateResult>
                
                // 스케일, 회전, 평행이동 순으로 변환을 적용한다.
                var result = prevAffine;
                result.AddScaleFactor(scaleOffset);

                if (hasRotate)
                {
                    result.Rotate(rotationOffset);
                }

                result.AddLocalPositionOffset(positionOffset, !hasIgnoreScaledPositionFlag);
                
                #endregion
                                              
                #region <CalulatePivot>

                if (p_ObjectDeployEventExtraPreset._UsingFallbackTargetPositionFlag)
                {
                    var fallBackPosition = prevAffine.TransformPosition(FallBackPositionOffset);
                    p_ObjectDeployEventExtraPreset.SetTargetPivot(fallBackPosition);
                }
                
                p_ObjectDeployEventExtraPreset.SetCurrentPivot(result);

                #endregion
                
                return p_ObjectDeployEventExtraPreset;
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "SequentialLinearDeployDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 10000;
            EndIndex = 20000;
        }

        public override ObjectDeployDataRoot.ObjectDeployType GetThisLabelType()
        {
            return ObjectDeployDataRoot.ObjectDeployType.Linear;
        }
    }
}