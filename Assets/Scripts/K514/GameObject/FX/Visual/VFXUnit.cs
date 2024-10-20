using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 파티클 시스템을 포함하는, 연출용 오브젝트를 제어하는 컴포넌트 클래스
    /// </summary>
    public class VFXUnit : FXUnit
    {
        #region <Fields>
 
        /// <summary>
        /// 최상위 파티클 시스템
        /// </summary>
        protected ParticleSystem _MainParticleSystem;
        
        /// <summary>
        /// 최상위 파티클 시스템의 메인 모듈
        /// </summary>
        protected ParticleSystem.MainModule _MainParticleSystemMain;
        
        /// <summary>
        /// 메인 파티클 시스템 + 하위 파티클 시스템 리스트
        /// </summary>
        protected List<ParticleSystem> _ParticleSystemGroup;
         
        /// <summary>
        /// 애니메이터 그룹
        /// </summary>
        private List<Animator> _AnimatorGroup;
         
        /// <summary>
        /// 파티클 랜더러 그룹
        /// </summary>
        private List<ParticleSystemRenderer> _ParticleRendererGroup;
        
        /// <summary>
        /// 해당 오브젝트가 활성화되고, 자동으로 릴리스되기까지 걸리는 시간.
        /// 해당 시간은 모든 파티클 시스템의 수명을 평가하여 정해진다.
        /// </summary>
        private uint _LifeTime_Msec;
                  
        /// <summary>
        /// 평가된 파티클 최대 수명과 별개로 추가할 수명
        /// </summary>
        protected uint _ExtraLifeTime_Msec;
        
        /// <summary>
        /// 원본 프리팹이 루프를 가지고 있었는지 표시하는 플래그
        /// </summary>
        private bool _DefaultHasLoop;

        /// <summary>
        /// 현재 파티클에 루프가 적용중인지 표시하는 플래그
        /// </summary>
        private bool _CurrentHasLoop;

        /// <summary>
        /// 파티클 충돌시 동작할 SubEmitter 인덱스 리스트
        /// </summary>
        protected List<int> _SubEmitterIndexListOnCollision;
        
        /// <summary>
        /// 파티클 파기시 동작할 SubEmitter 인덱스 리스트
        /// </summary>
        protected List<int> _SubEmitterIndexListOnDeath;

        /// <summary>
        /// 현재 적용중인 시뮬레이트 속도
        /// </summary>
        protected FloatProperty_Inverse _SimulateSpeed;

        /// <summary>
        /// 수명을 측정하는데 사용할 시간 이벤트 객체
        /// </summary>
        protected SafeReference<object, GameEventTimerHandlerWrapper> _TimerEventHandler;

        /// <summary>
        /// 최초 스폰되었을 때의 부모 아핀 객체
        /// </summary>
        private Transform _OriginParent;

        /// <summary>
        /// 파기가 예약됬는지 표시하는 플래그
        /// </summary>
        private bool _DeadReserveFalg;
        
        #endregion
   
        #region <Callbacks>
 
        public override void OnSpawning()
        {
            base.OnSpawning();

            gameObject.TurnLayerTo(GameManager.GameLayerType.Vfx, true);
            
            DeployableType = ObjectDeployTool.DeployableType.VFX;
            _Transform.SetParent(VfxSpawnManager.GetInstance._VFXObjectWrapper);
            _OriginParent = _Transform.parent;

            OnInitializeMainParticleSystem();
            
            _ParticleSystemGroup = new List<ParticleSystem>();
            GetComponentsInChildren(_ParticleSystemGroup);
            _AnimatorGroup = new List<Animator>();
            GetComponentsInChildren(_AnimatorGroup);
            _ParticleRendererGroup = new List<ParticleSystemRenderer>();
            GetComponentsInChildren(_ParticleRendererGroup);

            var maxLifeSpan = 0f;
            // 해당 오브젝트에 포함된 파티클들의 최대 수명 값을 찾는다.
            foreach (var particleSystem in _ParticleSystemGroup)
            {
                var psMain = particleSystem.main;
                
                // 루프하는 파티클이 있다면 해당 플래그를 세트한다.
                if (!_CurrentHasLoop)
                {
                    _DefaultHasLoop = _CurrentHasLoop = particleSystem.main.loop;
                }
                
                // 파티클 기대 수명 = 파티클 생성 선딜레이(startDelay) + Max[파티클 시스템 오브젝트 수명(duration), 생성된 파티클 수명(lifeTime)] + 1초
                var evaluatedTime = 
                    Mathf.Max(psMain.startDelay.constant, psMain.startDelay.constantMax, psMain.startDelay.constantMin)
                    + Mathf.Max(psMain.duration , psMain.startLifetime.constant, psMain.startLifetime.constantMax, psMain.startLifetime.constantMin) 
                    + 1f;

                maxLifeSpan = Mathf.Max(maxLifeSpan, evaluatedTime);
                // 파티클의 활성화 시점도 제어해야하기 때문에 활성화 기능을 오프해준다.
                particleSystem.Stop();
                psMain.playOnAwake = false;
                psMain.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
            }

            _LifeTime_Msec = (uint) (maxLifeSpan * 1000);
        }

        protected virtual void OnInitializeMainParticleSystem()
        {
            _MainParticleSystem = GetComponent<ParticleSystem>();
            
            // 최상위 파티클 시스템이 없다면, emit기능이 없는 파티클 시스템 컴포넌트를 추가시켜준다.
            if (null == _MainParticleSystem)
            {
                _MainParticleSystem = _Transform.gameObject.AddComponent<ParticleSystem>();
                _MainParticleSystemMain = _MainParticleSystem.main;
                _MainParticleSystemMain.loop = false;

#if UNITY_EDITOR && SERVER_DRIVE
                 if (!CustomDebug.VisualizeServerNode)
                {  
                    var emitModule = _MainParticleSystem.emission;
                    emitModule.enabled = false;
                }
#else
                var emitModule = _MainParticleSystem.emission;
                emitModule.enabled = false;
#endif
            }
            else
            {
                _MainParticleSystemMain = _MainParticleSystem.main;
            }

            // 시뮬레이트 속도를 초기화 시킨다.
            var simulateSpeed = _MainParticleSystemMain.simulationSpeed;
            _SimulateSpeed = new FloatProperty_Inverse(simulateSpeed, 0f);

            // 서브 이미터를 초기화 시킨다.
            _SubEmitterIndexListOnCollision = new List<int>();
            _SubEmitterIndexListOnDeath = new List<int>();
            
            var subEmitterModule = _MainParticleSystem.subEmitters;
            var subEmitterCount = subEmitterModule.subEmittersCount;
            for (int i = 0; i < subEmitterCount; i++)
            {
                var subEmitterType = subEmitterModule.GetSubEmitterType(i);
                switch (subEmitterType)
                {
                    case ParticleSystemSubEmitterType.Birth:
                    case ParticleSystemSubEmitterType.Trigger:
                    case ParticleSystemSubEmitterType.Manual:
                        break;
                    case ParticleSystemSubEmitterType.Collision:
                    {
                        subEmitterModule.SetSubEmitterType(i, ParticleSystemSubEmitterType.Manual);
                        _SubEmitterIndexListOnCollision.Add(i);
                    }
                        break;
                    case ParticleSystemSubEmitterType.Death:
                    {
                        subEmitterModule.SetSubEmitterType(i, ParticleSystemSubEmitterType.Manual);
                        _SubEmitterIndexListOnDeath.Add(i);
                    }
                        break;
                }
            }
        }

        public override void OnPooling()
        {
            base.OnPooling();
             
            SetStop();
            _CurrentHasLoop = _DefaultHasLoop;
            _ExtraLifeTime_Msec = 0;
            
            ApplyScale(ObjectScale.CurrentSqrtValue, ObjectScale.CurrentInverseSqrtValue);
        }
 
        public override void OnRetrieved()
        {
            ApplyScale(ObjectScale.CurrentInverseSqrtValue, ObjectScale.CurrentSqrtValue);
            
            base.OnRetrieved();

            EventTimerTool.ReleaseEventHandler(ref _TimerEventHandler);
            _Transform.SetParent(_OriginParent, false);
            SetSimulateSpeed(1f);
 
            foreach (var animator in _AnimatorGroup)
            {
                animator.Rebind();
            }

            _DeadReserveFalg = false;
        }

        protected virtual void OnSpawnSubEmitter_OnCollision()
        {
            foreach (var index in _SubEmitterIndexListOnCollision)
            {
                _MainParticleSystem.TriggerSubEmitter(index);
            }
        }
        
        protected virtual void OnSpawnSubEmitter_OnDeath()
        {
            foreach (var index in _SubEmitterIndexListOnDeath)
            {
                _MainParticleSystem.TriggerSubEmitter(index);
            }
        }

        #endregion
 
        #region <Methods>

        public override void ApplyAffinePreset(TransformTool.AffineCachePreset p_Affine)
        {
            _Transform.position = p_Affine.Position;
            
            var extraRecord = (VfxExtraDataRecordBridge) _PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            if (!extraRecord.FixedRotationFlag)
            {
                _Transform.rotation = p_Affine.Rotation;
            }
            
            ObjectScale = new FloatProperty_Inverse_Sqr_Sqrt(p_Affine.ScaleFactor, 0f);
            SetScale(1f);
        }
        
        public void ApplyScale(float p_SizeScale, float p_EmitScale)
        {
            foreach (var ps in _ParticleSystemGroup)
            {
                var shapeModule = ps.shape;
                shapeModule.scale *= p_EmitScale;
                var mainModule = ps.main;
                if (mainModule.startSize3D)
                {
                    mainModule.startSize = MultiplyScaleModule(mainModule.startSize, p_SizeScale);
                }
                else
                {
                    mainModule.startSizeX = MultiplyScaleModule(mainModule.startSizeX, p_SizeScale);
                    mainModule.startSizeY = MultiplyScaleModule(mainModule.startSizeY, p_SizeScale);
                    mainModule.startSizeZ = MultiplyScaleModule(mainModule.startSizeZ, p_SizeScale);
                }
            }
        }

        private ParticleSystem.MinMaxCurve MultiplyScaleModule(ParticleSystem.MinMaxCurve p_ShadeModule, float p_ScaleFactor)
        {
            switch (p_ShadeModule.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    p_ShadeModule.constant *= p_ScaleFactor;
                    break;
                case ParticleSystemCurveMode.Curve:
                    p_ShadeModule.curveMultiplier *= p_ScaleFactor;
                    break;
                case ParticleSystemCurveMode.TwoCurves:
                    p_ShadeModule.curveMultiplier *= p_ScaleFactor;
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    p_ShadeModule.constantMin *= p_ScaleFactor;
                    p_ShadeModule.constantMax *= p_ScaleFactor;
                    break;
            }

            return p_ShadeModule;
        }

        public bool IsReservedDeadOrDead()
        {
            return _DeadReserveFalg || !this.IsValid();
        }

        public virtual void SetRemove(bool p_InstantRemove, uint p_Predelay)
        {
            if (!IsReservedDeadOrDead())
            {
                GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _TimerEventHandler, this, SystemBoot.TimerType.GameTimer, false);
                var totalDelay = _ExtraLifeTime_Msec + (uint)(_LifeTime_Msec * _SimulateSpeed.CurrentInverseValue) + p_Predelay;
                
                if (p_InstantRemove || totalDelay < 50)
                {
                    RetrieveObject();
                }
                else
                {
                    _DeadReserveFalg = true;
                    
                    if (_CurrentHasLoop)
                    {
                        SetStop();
                    }
                     
                    var (_, eventHandler) = _TimerEventHandler.GetValue();
                    eventHandler
                        .AddEvent(
                            totalDelay,
                            handler =>
                            {
                                handler.Arg1.RetrieveObject();
                                MainGameUI.Instance._UIEffect.RemoveListValue(this);
                                return true;
                            }, 
                            null, this
                        );
                    eventHandler.StartEvent();
                }
            }
        }
 
        protected virtual void SetPlay()
        {
            var extraRecord = (VfxExtraDataRecordBridge) _PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            SetSimulateSpeed(extraRecord.SimulateSpeedFactor);
            
            foreach (var particleSystem in _ParticleSystemGroup)
            {
                particleSystem.Play();
            }
 
            if (_CurrentHasLoop)
            {
                 
            }
            else
            {
                SetRemove(false, 0);
            }
        }
         
        public void SetPlay(uint p_PreDelay)
        {
            if (p_PreDelay < 50)
            {
                SetPlay();
            }
            else
            {
                GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _TimerEventHandler, this, SystemBoot.TimerType.GameTimer, false);
                var (_, eventHandler) = _TimerEventHandler.GetValue();
                eventHandler
                    .AddEvent(
                        p_PreDelay,
                        handler =>
                        {
                            handler.Arg1.SetPlay();
                            return true;
                        }, 
                        null, this
                    );
                eventHandler.StartEvent();
            }
        }
         
        public void BreakLoop()
        {
            _CurrentHasLoop = false;
        }
 
        public void SetExtraLifeTime(uint p_Duration)
        {
            _ExtraLifeTime_Msec = p_Duration;
        }
 
        public void SetStop()
        {
            foreach (var particleSystem in _ParticleSystemGroup)
            {
                particleSystem.Stop();
            }
        }

        protected void SetSimulateSpeed(float p_Factor)
        {
            _SimulateSpeed = _SimulateSpeed.ApplyScale(p_Factor);

            foreach (var ps in _ParticleSystemGroup)
            {
                var tryMainModule = ps.main;
                tryMainModule.simulationSpeed = _SimulateSpeed.CurrentValue;
            }
        }

        protected void SetEnableRender(bool p_Flag)
        {
            foreach (var renderer in _ParticleRendererGroup)
            {
                renderer.enabled = p_Flag;
            }
        }

        #endregion
    }
}