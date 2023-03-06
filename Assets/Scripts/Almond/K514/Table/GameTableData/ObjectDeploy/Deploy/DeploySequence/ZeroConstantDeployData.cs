using UnityEngine;

namespace k514
{
    /// <summary>
    /// 상수 아핀 배치
    /// 지정한 위치에 배치를 하는, 이벤트 진행에 따라 딱히 배치 아핀값이 변하지 않는 이벤트를 기술하는 테이블 클래스
    /// </summary>
    public class ZeroConstantDeployData : ObjectDeployDataBase<ZeroConstantDeployData, ZeroConstantDeployData.TableRecord>
    {
        public class TableRecord : DeployTableRecordBase
        {
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
                if (ObjectDeployFlagMask.HasAnyFlagExceptNone(ObjectDeployTool.ObjectDeployFlag.RotationOffset))
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
                return p_ObjectDeployEventExtraPreset;
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ZeroConstantDeployDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 10000;
        }

        public override ObjectDeployDataRoot.ObjectDeployType GetThisLabelType()
        {
            return ObjectDeployDataRoot.ObjectDeployType.ZeroConstant;
        }
    }
}