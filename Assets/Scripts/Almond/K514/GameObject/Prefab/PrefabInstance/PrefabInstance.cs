using UnityEngine;

namespace k514
{
    public partial class PrefabInstance : PoolingUnityObject<PrefabInstance>, IDeployee
    {
        #region <Fields>

        /// <summary>
        /// 아핀 초기 값 캐싱 프리셋
        /// </summary>
        protected TransformTool.TransformPreset _TransformPreset;

        /// <summary>
        /// 배치 타입
        /// </summary>
        public ObjectDeployTool.DeployableType DeployableType { get; protected set; }

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            // 해당 풀링 인스턴스의 수명은, 해당 인스턴스의 원본 프리팹의 ResourceLifeCycleType에 의해 결정되며
            // 씬 전환으로는 해당 인스턴스는 파기되지 않는다.
            // 파기하기 위해서는 PrefabPoolingManager의 Release 계열 메서드를 통해 파기해야한다.
            if (transform.parent == null)
            {
                DontDestroyOnLoad(this);
            }
            
#if UNITY_EDITOR
            if (CustomDebug.PrintUnitLifeCycleLog)
            {
                Debug.Log("Spawning");
            }
#endif
            OnAwakeNode();
        }

        public override void OnPooling()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintUnitLifeCycleLog)
            {
                Debug.Log("Pooling");
            }
#endif
            
            // 해당 오브젝트는 풀러에 의해서 Spawning~Pooling 콜백 사이에
            // 절대좌표가 정해진다.
            // 해당 값을 캐싱해둔다.
            _TransformPreset = _Transform.GetTransformPreset();
        }

        public override void OnRetrieved()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintUnitLifeCycleLog)
            {
                Debug.Log("Removing");
            }
#endif
            _TransformPreset.ZeroAffine(_Transform);
            SetScale(1f);

            OnRetrieveNode();
        }

        public virtual void OnPositionTransition(Vector3 p_TransitionTo)
        {
            _TransformPreset.Position = p_TransitionTo;
            _Transform.position = _TransformPreset.Position;
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 지정한 위치로 해당 오브젝트를 옮기고, 해당 위치를 캐싱하는 메서드
        /// </summary>
        public virtual void SetPivotPosition(Vector3 p_TargetPos, bool p_SyncPosition, bool p_ForceSurface)
        {
            var tryPosition = p_ForceSurface 
                ? PhysicsTool.GetHighestObjectPosition_RayCast(p_TargetPos, GameManager.Obstacle_Terrain_LayerMask, QueryTriggerInteraction.Ignore).Item2 
                : p_TargetPos;
           
            if (p_SyncPosition)
            {
                OnPositionTransition(tryPosition);
            }
            else
            {
                _TransformPreset.Position = tryPosition;
            }
        }

        public void SetLook(Unit p_TargetUnit)
        {
            if (p_TargetUnit.IsValid())
            {
                SetLook(p_TargetUnit._Transform.position);
            }
        }
        
        public void SetLook(Transform p_TargetAffine)
        {
            if (p_TargetAffine.IsValid())
            {
                SetLook(p_TargetAffine.position);
            }
        }
        
        public void SetLook(Vector3 p_TargetPosition)
        {
            var targetDirectionXZ = _Transform.position.GetDirectionUnitVectorTo(p_TargetPosition).XZVector();
            SetLookAt(targetDirectionXZ);
        }

        public void SetLookAt(Vector3 p_TargetVector)
        {
            if (!p_TargetVector.IsReachedZero())
            {
                _Transform.forward = p_TargetVector;
            }
        }

        public void SetRotation(Vector3 p_Rotation)
        {
            _Transform.rotation = Quaternion.Euler(p_Rotation);
        }

        public void SetRotation(Quaternion p_Rotation)
        {
            _Transform.rotation = p_Rotation;
        }
        
        public void RotateSelf(float p_ClockWiseValue)
        {
            _Transform.Rotate(_Transform.up, p_ClockWiseValue, Space.Self);
        }
        
        public Vector3 GetPivotPosition()
        {
            return _TransformPreset.Position;
        }

        public void ApplyAffinePreset(Vector3 p_Position, Vector3 p_Rotation, bool p_ForceSurface)
        {
            SetPivotPosition(p_Position, true, p_ForceSurface);
            SetRotation(p_Rotation);
        }
        
        public void ApplyAffinePreset(Vector3 p_Position, Vector3 p_Rotation, float p_Scale, bool p_ForceSurface)
        {
            SetPivotPosition(p_Position, true, p_ForceSurface);
            SetRotation(p_Rotation);
            SetScale(p_Scale);
        }
        
        public virtual void ApplyAffinePreset(TransformTool.AffineCachePreset p_DeployPreset, bool p_ForceSurface)
        {
            SetPivotPosition(p_DeployPreset.Position, true, p_ForceSurface);
            SetRotation(p_DeployPreset.Rotation);
            SetScale(p_DeployPreset.ScaleFactor);
        }

        #endregion
    }
}