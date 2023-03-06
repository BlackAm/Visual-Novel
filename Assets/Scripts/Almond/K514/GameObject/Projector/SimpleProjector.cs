using UnityEngine;

namespace k514
{
    public class SimpleProjector : PrefabInstance
    {
        #region <Consts>

#if !SERVER_DRIVE
        protected const string __MainTexture_PropertyName = "_MainTex";
        protected const float __ProjectorFrustumLowerBound = 0.1f;
#endif

        #endregion

        #region <Fields>

#if !SERVER_DRIVE
        protected Projector _InnerProjector;
        protected Transform _InnerProjectorTransform;
        protected Material _Material;
        protected Color _Color;

        protected ProjectorWorkState _CurrentPhase;
        protected ProgressTimer _FadeInTimer;
        protected (bool t_Valid, ProgressTimer t_Timer) _ProjectTimerTuple;
        protected ProgressTimer _FadeOutTimer;
        protected (AssetPreset, Texture) _AssetTuple;
        protected float _OriginOrthoSize;
        protected float _HeightOffset;
        protected float _ProjectDistance;
        private Transform _OriginParent;
#endif

        #endregion

        #region <Enums>
        
#if !SERVER_DRIVE
        public enum ProjectorWorkState
        {
            None = 0,
            Enter,
            FadeIn,
            Project,
            FadeOut,
            Terminate,
        }
#endif

        #endregion

        #region <Callbacks>

#if !SERVER_DRIVE
        public override void OnSpawning()
        {
            base.OnSpawning();
           
            DeployableType = ObjectDeployTool.DeployableType.Projector;
            _Transform.SetParent(ProjectorSpawnManager.GetInstance._ProjectorObjectWrapper);
            _OriginParent = _Transform.parent;
            
            _InnerProjector = _Transform.GetComponentInChildren<Projector>();
            _InnerProjector.orthographic = true;
            _InnerProjectorTransform = _InnerProjector.transform;
            _InnerProjector.material = new Material(_InnerProjector.material);
            _Material = _InnerProjector.material;
            _Color = _Material.color;
            
            var projectorExtraData = (ProjectorExtraDataRecordBridge)_PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            _HeightOffset = projectorExtraData.HeightOffset;
            _ProjectDistance = projectorExtraData.ProjectDistance;
            
            _InnerProjector.orthographicSize = _OriginOrthoSize = projectorExtraData.ProjectSize;
            _InnerProjectorTransform.transform.localPosition = _HeightOffset * Vector3.up;

            if (projectorExtraData.ProjectTime > 0f)
            {
                _ProjectTimerTuple.t_Valid = true;
                _ProjectTimerTuple.t_Timer.Initialize(projectorExtraData.ProjectTime);
            }
            else
            {
                _ProjectTimerTuple.t_Valid = false;
            }

            _FadeInTimer.Initialize(projectorExtraData.FadeInTime);
            _FadeOutTimer.Initialize(projectorExtraData.FadeOutTime);

            var ignoreLayerList = projectorExtraData.IgnoreLayerFlagList;
            if (!ReferenceEquals(null, ignoreLayerList))
            {
                foreach (var gameLayerType in ignoreLayerList)
                {
                    _InnerProjector.ignoreLayers |= (int) gameLayerType;
                }
            }

            _InnerProjector.enabled = false;
            OnLoadFirstImage();
        }

        public override void OnPooling()
        {
            base.OnPooling();
           
            SetTransparent(0);
            var projectorExtraData = (ProjectorExtraDataRecordBridge)_PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            if (_ProjectTimerTuple.t_Valid)
            {
                var collisionCheckRate = projectorExtraData.ProjectCollisionCheckRate;
                if (collisionCheckRate > 0.1f)
                {
                    var projectorRange = GetProjectorRange(collisionCheckRate);
                    var result = PhysicsTool.GetBoxOverlap(projectorRange.Item1, projectorRange.Item3, projectorRange.Item2, GameManager.Terrain_LayerMask);
                    if (result)
                    {
                        SetProjectionStart();
                    }
                    else
                    {
                        SetProjectionTerminate();
                    }
                }
                else
                {
                    SetProjectionStart();
                }
            }

            SetScale(1f);
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();

            InitProjector();
        }

        protected virtual void Update()
        {
            switch (_CurrentPhase)
            {
                case ProjectorWorkState.None:
                    break;
                case ProjectorWorkState.Enter:
                    _CurrentPhase++;
                    _InnerProjector.enabled = true;
                    break;
                case ProjectorWorkState.FadeIn:
                    OnFadeIn(Time.deltaTime);
                    break;
                case ProjectorWorkState.Project:
                    OnProjection(Time.deltaTime);
                    break;
                case ProjectorWorkState.FadeOut:
                    OnFadeOut(Time.deltaTime);
                    break;
                case ProjectorWorkState.Terminate:
                    if (_ProjectTimerTuple.t_Valid)
                    {
                        RetrieveObject();
                    }
                    else
                    {
                        InitProjector();
                    }
                    break;
            }
        }

        protected virtual void OnLoadFirstImage()
        {
            var projectorExtraData = (PrefabExtraData_SimpleProjector.PrefabExtraDataSimpleProjectorRecord) _PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            _AssetTuple = ImageNameTableData.GetInstanceUnSafe.GetTexture(projectorExtraData.ImageIndex,
                ResourceType.Image, ResourceLifeCycleType.Free_Condition);

            SetTexture(_AssetTuple.Item2);
        }

        protected virtual void OnFadeIn(float p_Dt)
        {
            if (_FadeInTimer.IsOver())
            {
                _CurrentPhase++;
            }
            else
            {
                _FadeInTimer.Progress(p_Dt);
                SetTransparent(_FadeInTimer.ProgressRate);
            }
        }

        protected virtual void OnProjection(float p_Dt)
        {
            if (_ProjectTimerTuple.t_Valid)
            {
                if (_ProjectTimerTuple.t_Timer.IsOver())
                {
                    _CurrentPhase++;
                }
                else
                {
                    _ProjectTimerTuple.t_Timer.Progress(p_Dt);
                }
            }
        }

        protected virtual void OnFadeOut(float p_Dt)
        {
            if (_FadeOutTimer.IsOver())
            {
                _CurrentPhase++;
            }
            else
            {
                _FadeOutTimer.Progress(p_Dt);
                SetTransparent(1f - _FadeOutTimer.ProgressRate);
            }
        }

        protected virtual void OnDispose()
        {
            if (_AssetTuple.Item1.IsValid)
            {
                SetTexture(null);
                LoadAssetManager.GetInstanceUnSafe?.UnloadAsset(_AssetTuple.Item1);
                _AssetTuple = default;
            }
        }
#endif

        #endregion

        #region <Methods>

#if !SERVER_DRIVE
        public void SetParent(Transform p_Target, bool p_WorldPosStay)
        {
            _Transform.SetParent(p_Target, p_WorldPosStay);
            UpdateFarClipPlaneScale();
        }

        public (Vector3, Quaternion, Vector3) GetProjectorRange(float p_PlaneRate)
        {
            var halfNearPlane = _InnerProjector.nearClipPlane * 0.5f;
            var projectSize = _InnerProjector.orthographicSize * p_PlaneRate;
            return (_InnerProjectorTransform.position + halfNearPlane * Vector3.down, _Transform.rotation, new Vector3(projectSize, halfNearPlane, projectSize));
        }

        public void SetTransparent(float p_Alpha)
        {
            _Material.color = new Color(_Color.r, _Color.g, _Color.b, p_Alpha);
        }

        public void SetIntensity(float p_Intensity)
        {
            float factor = Mathf.Pow(2, p_Intensity);
            _Material.color = new Color(_Color.r * factor, _Color.g * factor, _Color.b * factor, _Color.a);
        }

        public void SetTexture(Texture p_Texture)
        {
            _Material.SetTexture(__MainTexture_PropertyName, p_Texture);
        }
        
        public override void SetScale(float p_ScaleRate)
        {
            base.SetScale(p_ScaleRate);

            if (PoolState != PoolState.None)
            {
                _InnerProjector.orthographicSize = _OriginOrthoSize * p_ScaleRate;
                UpdateFarClipPlaneScale();
            }
        }

        private void UpdateFarClipPlaneScale()
        {
            SetFarClipPlaneScale(_Transform.lossyScale.y);
        }
        
        public void SetFarClipPlaneScale(float p_ScaleRate)
        {
            _InnerProjector.farClipPlane = p_ScaleRate * (_HeightOffset + Mathf.Max(__ProjectorFrustumLowerBound, _ProjectDistance));
        }

        public void SetProjectionStart()
        {
            _FadeInTimer.Reset();
            SetTransparent(0f);
            _FadeOutTimer.Reset();
            _CurrentPhase = ProjectorWorkState.Enter;
            UpdateFarClipPlaneScale();
        }

        public void SetProjectionFadeOut()
        {
            _FadeOutTimer.Reset();
            _CurrentPhase = ProjectorWorkState.FadeOut;
        }
        
        public void SetProjectionTerminate()
        {
            _CurrentPhase = ProjectorWorkState.Terminate;
        }

        private void InitProjector()
        {
            _CurrentPhase = ProjectorWorkState.None;
            _FadeInTimer.Reset();
            _ProjectTimerTuple.t_Timer.Reset();
            _FadeOutTimer.Reset();
            _InnerProjector.enabled = false;
            _Transform.SetParent(_OriginParent, false);
        }
#endif

        #endregion

        #region <Disposable>
        
#if !SERVER_DRIVE
        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            OnDispose();
            
            base.DisposeUnManaged();
        }
#endif
     
        #endregion
    }
}