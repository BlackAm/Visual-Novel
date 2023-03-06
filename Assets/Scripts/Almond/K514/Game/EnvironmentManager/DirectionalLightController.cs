#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class DirectionalLightController : SceneChangeEventSingleton<DirectionalLightController>
    {
        #region <Fields>

        /// <summary>
        /// 현재 씬의 메인 광원 오브젝트
        /// </summary>
        public Light MainLight { get; private set; }

        /// <summary>
        /// 현재 씬의 메인 광원 아핀 오브젝트
        /// </summary>
        public Transform MainLightTransform { get; private set; }
        
        /// <summary>
        /// 현재 씬의 메인 광원 오브젝트가 유효한지 표시하는 플래그
        /// </summary>
        private bool _MainLightValid;
        
        /// <summary>
        /// 메인 광원 색상 제어 이벤트 핸들러
        /// </summary>
        private ColorLerpEventTimerHandler _LightControlColorHandler;
        
        /// <summary>
        /// 메인 광원 자전 이벤트 핸들러
        /// </summary>
        private FloatLerpEventTimerHandler _LightControlSelfRotationDegreeHandler;
                
        /// <summary>
        /// 메인 광원 자전 이벤트 핸들러2
        /// </summary>
        private DirectionLerpEventTimerHandler _LightControlSelfRotationForwardHandler;
        
        /// <summary>
        /// 메인 광원 공전 이벤트 핸들러
        /// </summary>
        private DirectionLerpEventTimerHandler _LightControlOrbitRotationHandler;
        
        /// <summary>
        /// 메인 광원 세기 제어 이벤트 핸들러
        /// </summary>
        private FloatLerpEventTimerHandler _LightControlIntensityHandler;
        
        /// <summary>
        /// 메인 광원 간접광 세기 제어 이벤트 핸들러
        /// </summary>
        private FloatLerpEventTimerHandler _LightControlIndirectMultiplierHandler;

        /// <summary>
        /// 자전 축 벡터
        /// </summary>
        private Vector3 _SelfRotationPivot;
        
        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _MainLightValid = false;
            
            _LightControlColorHandler = new ColorLerpEventTimerHandler(SystemBoot.GameEventTimer);
            _LightControlSelfRotationDegreeHandler = new FloatLerpEventTimerHandler(SystemBoot.GameEventTimer);
            _LightControlSelfRotationForwardHandler = new DirectionLerpEventTimerHandler(SystemBoot.GameEventTimer);
            _LightControlOrbitRotationHandler = new DirectionLerpEventTimerHandler(SystemBoot.GameEventTimer);
            _LightControlIntensityHandler = new FloatLerpEventTimerHandler(SystemBoot.GameEventTimer);
            _LightControlIndirectMultiplierHandler = new FloatLerpEventTimerHandler(SystemBoot.GameEventTimer);
        }

        public override void OnInitiate()
        {
        }

        public override async UniTask OnScenePreload()
        {
            MainLight = null;
            MainLightTransform = null;
            _MainLightValid = false;

            await UniTask.SwitchToMainThread();
            var lightGroups = Object.FindObjectsOfType<Light>();
            foreach (var light in lightGroups)
            {
                if (light.type == LightType.Directional)
                {
                    MainLight = light;
                    MainLightTransform = MainLight.transform;
                    _MainLightValid = true;
                }
            }

            if (_MainLightValid)
            {
                _LightControlColorHandler.SetDefaultValue(MainLight.color);
                _LightControlColorHandler.SetValueChangedCallback(OnLightControlColorChanged);
                _LightControlSelfRotationDegreeHandler.SetDefaultValue(0f);
                _LightControlSelfRotationDegreeHandler.SetValueChangedCallback(OnLightControlRotateSelfDegreeChanged);
                _LightControlSelfRotationForwardHandler.SetDefaultValue(MainLightTransform.forward);
                _LightControlSelfRotationForwardHandler.SetValueChangedCallback(OnLightControlRotateSelfForwardChanged);
                _LightControlOrbitRotationHandler.SetDefaultValue(MainLightTransform.position);
                _LightControlIntensityHandler.SetDefaultValue(MainLight.intensity);
                _LightControlIndirectMultiplierHandler.SetDefaultValue(MainLight.bounceIntensity);
            }
        }

        public override void OnSceneStarted()
        {
        }

        public override void OnSceneTerminated()
        {
        }

        public override void OnSceneTransition()
        {
        }
        
        private void OnLightControlColorChanged()
        {
            if (_MainLightValid)
            {
                MainLight.color = _LightControlColorHandler._CurrentValue;
            }
        }
        private void OnLightControlRotateSelfDegreeChanged()
        {
            if (_MainLightValid)
            {
                MainLightTransform.Rotate(_SelfRotationPivot, _LightControlSelfRotationDegreeHandler._DeltaValue);
            }
        }
        
        private void OnLightControlRotateSelfForwardChanged()
        {
            if (_MainLightValid)
            {
                MainLightTransform.forward = _LightControlSelfRotationForwardHandler._CurrentValue;
            }
        }
        
        #endregion

        #region <Methods>

        public void SetMainLightColorLerp(Color p_TargetColor, uint p_Predelay, uint p_LerpDuration)
        {
            _LightControlColorHandler.SetValueLerpTo(p_TargetColor, p_Predelay, p_LerpDuration);
        }

        public void ResetMainLightColorLerp(uint p_Predelay, uint p_LerpDuration)
        {
            _LightControlColorHandler.ResetValueLerpTo(p_Predelay, p_LerpDuration);
        }

        public void SetMainLightRotationSelfLerp(Vector2 p_UV, float p_Degree, uint p_Predelay, uint p_LerpDuration)
        {
            _SelfRotationPivot = (p_UV.x * MainLightTransform.right + p_UV.y * MainLightTransform.up).normalized;
            _LightControlSelfRotationDegreeHandler.SetDefaultValue(0f);
            _LightControlSelfRotationDegreeHandler.SetValueLerpTo(p_Degree, p_Predelay, p_LerpDuration);
        }

        public void ResetMainLightRotationSelfLerp(uint p_Predelay, uint p_LerpDuration)
        {
            _LightControlSelfRotationForwardHandler.ResetValueLerpTo(p_Predelay, p_LerpDuration);
        }
        
        #endregion
    }
}
#endif