#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    public partial class CameraManager
    {
        #region <Consts>

        public static float NearCullingRadiusLowerBound = 0.1f;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 기본 컬링 마스크 값
        /// </summary>
        private int DefaultMask;
        
        /// <summary>
        /// 원거리 컬링 거리 배열
        /// </summary>
        private float[] FarCullingDistanceMap;

        /// <summary>
        /// 유닛 레이어 컬링 거리
        /// </summary>
        public float UnitFarCullingDistance { get; private set; }

        #endregion

        #region <Enum>

        public enum CameraDistanceCullingType
        {
            None,
            NearCulling,
            FarCulling,
        }

        #endregion
        
        #region <Callbacks>

        private void OnCreateRenderPartial()
        {
            // 카메라가 랜더링 하지 않을 레이어는 제거해준다.
            RemoveCullingMask(GameManager.GameLayerMaskType.UI);
            RemoveCullingMask(GameManager.GameLayerMaskType.CameraIgnore);
            DefaultMask = MainCamera.cullingMask;
            
            FarCullingDistanceMap = new float[32];
        }
        
        #endregion
        
        #region <Method/CullingMask>

        public void SetCameraBlind()
        {
            MainCamera.cullingMask = 0;
        }

        public void SetCullingMask(int p_CullingMask)
        {
            MainCamera.cullingMask = p_CullingMask;
        }

        public void ResetCullingMask()
        {
            SetCullingMask(DefaultMask);
        }

        public void ToggleCullingMask(GameManager.GameLayerMaskType p_LayerMaskType)
        {
            MainCamera.cullingMask ^= (int)p_LayerMaskType;
        }
        
        public void AddCullingMask(GameManager.GameLayerMaskType p_LayerMaskType)
        {
            MainCamera.cullingMask |= (int)p_LayerMaskType;
        }

        public void RemoveCullingMask(GameManager.GameLayerMaskType p_LayerMaskType)
        {
            MainCamera.cullingMask &= ~(int)p_LayerMaskType;
        }
   
        #endregion

        #region <Method/CullingDistance>

        /// <summary>
        /// 원거리 카메라 컬링 거리를 레이어 별로 설정하는 메서드
        /// </summary>
        public void SetFarCullingDistance(GameManager.GameLayerType p_LayerType, float p_Distance)
        {
            var layer = (int)p_LayerType;
            FarCullingDistanceMap[layer] = p_Distance;
            MainCamera.layerCullDistances = FarCullingDistanceMap;
        }
        
        /// <summary>
        /// 지정한 레이어의 원거리 카메라 컬링 거리를 리턴하는 메서드
        /// </summary>
        public float GetFarCullingDistance(GameManager.GameLayerType p_LayerType)
        {
            var layer = (int)p_LayerType;
            return FarCullingDistanceMap[layer];
        }

        /// <summary>
        /// true 하는 경우, 거리 기반의 구면 레이어 컬링을 수행한다.
        /// false 하는 경우, far plane을 조정해서 Camera Frustum을 통해 컬링을 수행한다.
        ///
        /// 만약 카메라가 제자리 회전을 하는 경우, 각 오브젝트의 카메라 거리는 변하지 않으므로
        /// true 기준으로는 컬링되는 오브젝트 멤버가 변화가 없다.
        ///
        /// 반대로 false 기준으로는 회전에 따라 Frustum이 움직이므로, 회전만으로도
        /// 컬링되는 멤버가 변한다.
        ///
        /// 기본값은 false이다.
        /// </summary>
        public void SetLayerCullingSpherical(bool p_Flag)
        {
            MainCamera.layerCullSpherical = p_Flag;
        }

        /// <summary>
        /// 현재 씬 설정 레코드로부터 추적 거리 프리셋을 가져오는 메서드
        /// </summary>
        private void UpdateRenderSetting()
        {
            var farCullingDistances = _CurrentSceneVariableRecord.FarCullingDistances;
            if(farCullingDistances != null)
            {
                foreach (var farCullingKV in farCullingDistances)
                {
                    var tryLayerType = farCullingKV.Key;
                    var tryCullingDistance = farCullingKV.Value;
                    SetFarCullingDistance(tryLayerType, tryCullingDistance);
                    
                    // UnitA레이어를 기준으로 삼는다
                    if (tryLayerType == GameManager.GameLayerType.UnitA)
                    {
                        SetFarCullingDistance(GameManager.GameLayerType.UnitB, tryCullingDistance);
                        SetFarCullingDistance(GameManager.GameLayerType.UnitC, tryCullingDistance);
                        SetFarCullingDistance(GameManager.GameLayerType.UnitD, tryCullingDistance);
                        SetFarCullingDistance(GameManager.GameLayerType.UnitE, tryCullingDistance);
                        SetFarCullingDistance(GameManager.GameLayerType.UnitF, tryCullingDistance);
                        SetFarCullingDistance(GameManager.GameLayerType.UnitG, tryCullingDistance);
                        SetFarCullingDistance(GameManager.GameLayerType.UnitH, tryCullingDistance);

                        UnitFarCullingDistance = tryCullingDistance;
                        CameraEventSender.WhenPropertyModified(CameraEventType.FarCullingDistanceChanged, default);
                    }
                }
            }
        }

        public float GetCameraZoomLowerBound()
        {
            return _CurrentTraceTargetPreset.NearBlockRadius;
        }

        public float GetCameraZoomUpperBound()
        {
            return _CurrentTraceTargetPreset.FarBlockRadius;
        }
        
        public float GetUnitFarCullingDistance()
        {
            return UnitFarCullingDistance;
        }

        public CameraDistanceCullingType GetCullingType(float p_Distance)
        {
            if (p_Distance > GetUnitFarCullingDistance())
            {
                return CameraDistanceCullingType.FarCulling;
            }
            else if(p_Distance > _CurrentTraceTargetPreset.NearCullingRadius)
            {
                return CameraDistanceCullingType.None;
            }
            else
            {
                return CameraDistanceCullingType.NearCulling;
            }
        }
        
        public (CameraDistanceCullingType, float) GetCullingDistancePreset(Vector3 p_Position)
        {
            var distance = (_ViewControlFocusWrapper.localPosition + p_Position).GetDirectionVectorTo(_ViewControlZoomWrapper.position).magnitude;
            return (GetCullingType(distance), distance);
        }

        #endregion
    }
}
#endif