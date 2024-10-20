using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public static class TransformTool
    {
        #region <Methods>

        public static bool IsValid(this Transform p_TargetTransform)
        {
            return !ReferenceEquals(null, p_TargetTransform);
        }

        public static (bool, Transform) FindRecursiveInclude(this Transform p_Target, List<string> p_NameSet, bool p_SearchParameterToo = true)
        {
            if (p_SearchParameterToo && p_Target.name.SearchKeyWord(p_NameSet))
            {
                return (true, p_Target);
            }
            else
            {
                var childCount = p_Target.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = p_Target.GetChild(i);
                    var recur = child.FindRecursiveInclude(p_NameSet);
                    if (recur.Item1)
                    {
                        return recur;
                    }
                }
                return (false, null);
            }
        }
        
        public static (bool, Transform) FindRecursive(this Transform p_Target, string p_Name, bool p_SearchParameterToo = true)
        {
            if (p_SearchParameterToo && p_Target.name == p_Name)
            {
                return (true, p_Target);
            }
            else
            {
                var childCount = p_Target.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = p_Target.GetChild(i);
                    var recur = child.FindRecursive(p_Name);
                    if (recur.Item1)
                    {
                        return recur;
                    }
                }
                return (false, null);
            }
        }
        
        public static (bool, T) FindRecursive<T>(this Transform p_Target, string p_Name, bool p_SearchParameterToo = true) where T : Object
        {
            var tryTransform = p_Target.FindRecursive(p_Name, p_SearchParameterToo);
            if (tryTransform.Item1)
            {
                return (true, tryTransform.Item2.GetComponent<T>());
            }
            else
            {
                return default;
            }
        }
        
        public static void SetTransformPreset(this Transform p_TargetTransform, TransformPreset p_Preset)
        {
            p_Preset.ApplyAffine(p_TargetTransform);
        }

        public static TransformPreset GetTransformPreset(this Transform p_TargetTransform)
        {
            return new TransformPreset(p_TargetTransform);
        }

        #endregion
        
        #region <Structs>

        public struct AffineDelta
        {
            public static AffineDelta GetDefaultAffineDelta()
            {
                return new AffineDelta(Vector3.zero, Quaternion.identity, 0f);
            }

            public Vector3 DeltaPos;
            public Quaternion DeltaRotation;
            public float DeltaScaleOffset;

            public AffineDelta(Vector3 p_DeltaPosition, Quaternion p_DeltaRotation, float p_DeltaScaleOffset)
            {
                DeltaPos = p_DeltaPosition;
                DeltaRotation = p_DeltaRotation;
                DeltaScaleOffset = p_DeltaScaleOffset;
            }
            
            public static AffineDelta operator+(AffineDelta p_Left, AffineDelta p_Right)
            {
                var deltaPosition = p_Left.DeltaPos + p_Right.DeltaPos;
                var deltaRotation = p_Left.DeltaRotation * p_Right.DeltaRotation;
                var deltaScaleOffset = p_Left.DeltaScaleOffset + p_Right.DeltaScaleOffset;

                return new AffineDelta(deltaPosition, deltaRotation, deltaScaleOffset);
            }
        }

        public struct AffineCachePreset
        {
            #region <Consts>

            public static AffineCachePreset GetDefaultAffineCachePreset()
            {
                return new AffineCachePreset(Vector3.zero);
            }

            #endregion
            
            #region <Fields>

            public Transform TargetAffine { get; private set; }
            public Vector3 Position { get; private set; }
            public Vector3 Right { get; private set; }
            public Vector3 Up { get; private set; }
            public Vector3 Forward { get; private set; }
            public Quaternion Rotation { get; private set; }
            public float ScaleFactor { get; private set; }
            
            #endregion

            #region <Constructor>

            public AffineCachePreset(Transform p_Transform)
            {
                TargetAffine = p_Transform;
                Position = default;
                Right = default;
                Up = default;
                Forward = default;
                ScaleFactor = TargetAffine.lossyScale.x;
                Rotation = Quaternion.identity;
                SyncAffine();
            }

            public AffineCachePreset(PrefabInstance p_Unit) : this(p_Unit._Transform)
            {
            }

            public AffineCachePreset(Vector3 p_Position)
            {
                TargetAffine = default;
                Position = p_Position;
                Right = Vector3.right;
                Up = Vector3.up;
                Forward = Vector3.forward;
                ScaleFactor = 1f;
                Rotation = Quaternion.identity;
            }
            
            public AffineCachePreset(Vector3 p_Position, Quaternion p_Rotation) : this(p_Position, p_Rotation, 1f)
            {
            }
            
            public AffineCachePreset(Vector3 p_Position, Quaternion p_Rotation, float p_Scale)
            {
                TargetAffine = default;
                Position = p_Position;
                Rotation = p_Rotation;
                Right = Rotation * Vector3.right;
                Up = Rotation * Vector3.up;
                Forward = Rotation * Vector3.forward;
                ScaleFactor = p_Scale;
            }
            
            public AffineCachePreset(Vector3 p_Position, Vector3 p_Angle) : this(p_Position, p_Angle, 1f)
            {
            }
            
            public AffineCachePreset(Vector3 p_Position, float p_Scale) : this(p_Position, Vector3.zero, p_Scale)
            {
            }
            
            public AffineCachePreset(Vector3 p_Position, Vector3 p_Angle, float p_Scale)
            {
                TargetAffine = default;
                Position = p_Position;
                Rotation = Quaternion.AngleAxis(p_Angle.x, Vector3.right)    
                            * Quaternion.AngleAxis(p_Angle.y, Vector3.up)
                            * Quaternion.AngleAxis(p_Angle.z, Vector3.forward);
                Right = Rotation * Vector3.right;
                Up = Rotation * Vector3.up;
                Forward = Rotation * Vector3.forward;
                ScaleFactor = p_Scale;
            }
            
            #endregion

            #region <Operator>
            
            public static implicit operator AffineCachePreset(Transform p_TargetTransform)
            {
                return new AffineCachePreset(p_TargetTransform);
            }
            
            public static implicit operator AffineCachePreset(Vector3 p_Position)
            {
                return new AffineCachePreset(p_Position);
            }
            
            public static implicit operator AffineCachePreset((Vector3 t_Position, Vector3 t_Angle) p_AffineTuple)
            {
                return new AffineCachePreset(p_AffineTuple.t_Position, p_AffineTuple.t_Angle);
            }
            
            public static implicit operator AffineCachePreset((Vector3 t_Position, Vector3 t_Angle, float t_Scale) p_AffineTuple)
            {
                return new AffineCachePreset(p_AffineTuple.t_Position, p_AffineTuple.t_Angle, p_AffineTuple.t_Scale);
            }
                        
            public static implicit operator AffineCachePreset((Vector3 t_Position, Quaternion t_Rotation) p_AffineTuple)
            {
                return new AffineCachePreset(p_AffineTuple.t_Position, p_AffineTuple.t_Rotation);
            }
            
            public static implicit operator AffineCachePreset((Vector3 t_Position, Quaternion t_Rotation, float t_Scale) p_AffineTuple)
            {
                return new AffineCachePreset(p_AffineTuple.t_Position, p_AffineTuple.t_Rotation, p_AffineTuple.t_Scale);
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                CustomDebug.DrawAffineCachedPreset(this, 1f);
                return $"{TargetAffine?.name} {Position}";
            }
#endif

            #endregion

            #region <Methods>

            /// <summary>
            /// 해당 프리셋이 아핀 객체를 참조중인지 리턴하는 메서드
            /// </summary>
            public bool IsValid()
            {
                return TargetAffine.IsValid();
            }

            /// <summary>
            /// 현재 아핀 객체를 참조하고 있는 경우, 모든 아핀 필드 멤버를
            /// 해당 객체의 값으로 갱신시킨다.
            /// </summary>
            public void SyncAffine()
            {
                if (IsValid())
                {
                    Position = TargetAffine.position;
                    Right = TargetAffine.right;
                    Up = TargetAffine.up;
                    Forward = TargetAffine.forward;
                    Rotation = TargetAffine.rotation;
                }
            }
            
            /// <summary>
            /// 현재 아핀 객체를 참조하고 있는 경우, 위치를
            /// 해당 객체의 값으로 갱신시킨다.
            /// </summary>
            public void SyncPosition()
            {
                if (IsValid())
                {
                    Position = TargetAffine.position;
                }
            }

            /// <summary>
            /// 스케일 배율을 지정하는 메서드
            /// </summary>
            public void SetScaleFactor(float p_Scale)
            {
                ScaleFactor = p_Scale;
            }
            
            /// <summary>
            /// 스케일 배율을 더하는 메서드
            /// </summary>
            public void AddScaleFactor(float p_Scale)
            {
                ScaleFactor += p_Scale;
            }
            
            /// <summary>
            /// 스케일 배율을 지정하는 메서드
            /// </summary>
            public void MulScaleFactor(float p_Scale)
            {
                ScaleFactor *= p_Scale;
            }
            
            /// <summary>
            /// 해당 아핀 좌표계를 기준으로 파라미터를 로컬변환시키는 메서드
            /// </summary>
            public Vector3 TransformVector(Vector3 p_Vector)
            {
                return p_Vector.x * Right + p_Vector.y * Up + p_Vector.z * Forward;
            }
            
            /// <summary>
            /// 해당 아핀 좌표계를 기준으로 파라미터를 로컬변환시키는 메서드
            /// </summary>
            public Vector3 TransformPosition(Vector3 p_Vector)
            {
                return Position + TransformVector(p_Vector);
            }
            
            /// <summary>
            /// 해당 아핀 좌표계를 기준으로 파라미터 만큼 Position을 평행이동 시키는 메서드
            /// </summary>
            public void AddLocalPositionOffset(Vector3 p_Offset, bool p_ApplyScaleFactor)
            {
                Position = TransformPosition(p_ApplyScaleFactor ? ScaleFactor * p_Offset : p_Offset);
            }
            
            /// <summary>
            /// 절대 좌표계를 기준으로 파라미터 만큼 Position을 평행이동 시키는 메서드
            /// </summary>
            public void AddWorldPositionOffset(Vector3 p_Offset)
            {
                Position += p_Offset;
            }
            
            /// <summary>
            /// 위치값을 변경하는 메서드
            /// </summary>
            public void SetPosition(Vector3 p_Position)
            {
                Position = p_Position;
            }

            /// <summary>
            /// 현재 아핀 객체의 좌표를 리턴한다.
            /// 만약 아핀 객체가 없다면 Position 값을 리턴한다.
            /// </summary>
            public Vector3 GetAffinePivotPosition()
            {
                return IsValid() ? TargetAffine.position : Position;
            }

            /// <summary>
            /// 기저 벡터값을 변경하는 메서드
            /// </summary>
            public void SetBasis(Vector3 p_Forward)
            {
                p_Forward = p_Forward.normalized;
                
                var right = p_Forward == Vector3.up ? Vector3.Cross(Vector3.back, p_Forward) :
                    p_Forward == Vector3.down ? Vector3.Cross(Vector3.forward, p_Forward) :
                    Vector3.Cross(Vector3.up, p_Forward);
                SetBasis(right, Vector3.Cross(p_Forward, right), p_Forward);
            }
            
            /// <summary>
            /// 기저 벡터값을 변경하는 메서드
            /// </summary>
            public void SetBasis(Vector3 p_Forward, Vector3 p_Up)
            {
                SetBasis(Vector3.Cross(p_Up, p_Forward), p_Up, p_Forward);
            }
            
            /// <summary>
            /// 기저 벡터값을 변경하는 메서드
            /// </summary>
            public void SetBasis(Vector3 p_Right, Vector3 p_Up, Vector3 p_Forward)
            {
                p_Right = p_Right.normalized;
                p_Up = p_Up.normalized;
                p_Forward = p_Forward.normalized;
                
                Rotation = Quaternion.LookRotation(p_Forward, p_Up);
                Right = p_Right;
                Up = p_Up;
                Forward = p_Forward;
            }
            
            /// <summary>
            /// 기저 벡터값을 변경하는 메서드
            /// </summary>
            public void SetBasis(Transform p_Pivot)
            {
                SetBasis(p_Pivot.right, p_Pivot.up, p_Pivot.forward);
            }
            
            /// <summary>
            /// 기저 벡터값을 변경하는 메서드
            /// </summary>
            public void SetBasis(AffineCachePreset p_Pivot)
            {
                Rotation = p_Pivot.Rotation;
                Right = p_Pivot.Right;
                Up = p_Pivot.Up;
                Forward = p_Pivot.Forward;
            }
            
            /// <summary>
            /// 기저 벡터값을 변경하는 메서드
            /// </summary>
            public void SetBasis()
            {
                Rotation = Quaternion.identity;
                Right = Vector3.right;
                Up = Vector3.up;
                Forward = Vector3.forward;
            }
            
            /// <summary>
            /// 해당 아핀 셋을 회전시키는 메서드
            /// </summary>
            public void Rotate(Quaternion p_Quaternion)
            {
                Rotation = p_Quaternion * Rotation;
                Right = p_Quaternion * Right;
                Up = p_Quaternion * Up;
                Forward = p_Quaternion * Forward;
            }

            /// <summary>
            /// 해당 아핀 셋을 회전시키는 메서드
            /// </summary>
            public void Rotate(Vector3 p_Angle)
            {
                var resultRotationX = Quaternion.AngleAxis(p_Angle.x, Right);
                var resultRotationY = Quaternion.AngleAxis(p_Angle.y, Up);
                var resultRotationZ = Quaternion.AngleAxis(p_Angle.z, Forward);
                var resultRotation = resultRotationX * resultRotationY * resultRotationZ;

                Rotate(resultRotation);
            }

            public void SetRotation(AffineCachePreset p_Affine)
            {
                Rotation = p_Affine.Rotation;
                Right = p_Affine.Right;
                Up = p_Affine.Up;
                Forward = p_Affine.Forward;
            }

            /// <summary>
            /// 파라미터로 받은 아핀 위상값 만큼 아핀연산을 수행하는 메서드
            /// </summary>
            public void ApplyAffineDelta(AffineDelta p_AffineDelta)
            {
                AddScaleFactor(p_AffineDelta.DeltaScaleOffset);
                AddLocalPositionOffset(p_AffineDelta.DeltaPos, false);
                Rotate(p_AffineDelta.DeltaRotation);
            }

            public AffineDelta GetAffineDelta(AffineCachePreset p_Affine)
            {
                var pivotPosition = p_Affine.Position;
                var pivotInverseRotation = Quaternion.Inverse(p_Affine.Rotation);
                var positionOffset = pivotInverseRotation * pivotPosition.GetDirectionVectorTo(Position);
                var scaleOffset = ScaleFactor - p_Affine.ScaleFactor;
                
                return new AffineDelta(positionOffset, Rotation * pivotInverseRotation, scaleOffset);
            }

            #endregion
        }
        
        public struct TransformPreset
        {
            public Transform Wrapper;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;

            public TransformPreset(Transform p_Wrapper)
            {
                Wrapper = p_Wrapper;
                Position = Wrapper.localPosition;
                Rotation = Wrapper.localRotation;
                Scale = Wrapper.localScale;
            }

            public void ZeroAffine()
            {
                Position = Vector3.zero;
                Rotation = Quaternion.identity;
                Scale = Vector3.one;
                ResetAffine();
            }
            
            public void ZeroAffine(Transform p_Wrapper)
            {
                Wrapper = p_Wrapper;
                ZeroAffine();
            }
            
            public void ResetAffine()
            {
                ApplyAffine(Wrapper);
            }
            
            public void SetAffine(TransformPreset p_TransformPreset)
            {
                Wrapper.localPosition = p_TransformPreset.Position;
                Wrapper.localRotation = p_TransformPreset.Rotation;
                Wrapper.localScale = p_TransformPreset.Scale; 
            }

            public void ApplyAffine(Transform p_TargetWrapper)
            {
                p_TargetWrapper.localPosition = Position;
                p_TargetWrapper.localRotation = Rotation;
                p_TargetWrapper.localScale = Scale;  
            }
        }

        #endregion
    }
}