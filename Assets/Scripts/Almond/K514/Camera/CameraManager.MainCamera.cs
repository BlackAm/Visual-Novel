#if !SERVER_DRIVE
using System.Linq;
using UnityEngine;

namespace k514
{
    public partial class CameraManager
    {
        #region <Fields>

        /// <summary>
        /// 메인 카메라 레퍼런스
        /// </summary>
        public Camera MainCamera { get; private set; }

        /// <summary>
        /// 메인 카메라 아핀 객체
        /// </summary>
        public Transform MainCameraTransform;

        /// <summary>
        /// 메인 카메라 오디오 리스너
        /// </summary>
        public AudioListener AudioListener { get; private set; }

        #endregion

        #region <Callbacks>

        private void OnCreateMainCameraPartial()
        {
            // 메인 카메라를 카메라 타입 래퍼들의 최하위 자손으로 넣어준다.
            var terminateWrapper = CameraWrapperTransformCollection.Last();
            Transform terminateWrapperTransform = null;
            var wrapperType = terminateWrapper.Key;
            
            if (AffineWrapperTypeList.Contains(wrapperType))
            {
                terminateWrapperTransform = CameraAffineTransformCollection[wrapperType].Rear;
            }
            else
            {
                terminateWrapperTransform = terminateWrapper.Value;
            }

            // 메인 카메라를 초기화 시켜준다.
            MainCamera = terminateWrapperTransform.GetDefaultCamera(CameraClearFlags.SolidColor, Color.black);
            MainCamera.name = "MainManagerCamera";
            MainCameraTransform = MainCamera.transform;

            // 메인 카메라의 레이어를 지정해준다.
            MainCamera.gameObject.TurnLayerTo(GameManager.GameLayerType.Camera, false);
            
            // 메인 카메라에 트리거 컬라이더를 추가해준다.
            var sphereCollider = MainCamera.gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = MainCamera.nearClipPlane;
        }
        
        #endregion

        #region <Methods>

        public void AddAudioListener()
        {
            // 메인 카메라에 오디오 리스너를 추가해준다.
            AudioListener = MainCamera.gameObject.AddComponent<AudioListener>();
            SetAudioListener(false);
        }

        public void UpdateMainCameraSetting()
        {
            MainCamera.clearFlags = _CurrentSceneCameraConfigure.CameraClearFlags;
        }

        public void SetAudioListener(bool p_Flag)
        {
            AudioListener.enabled = p_Flag;
        }

        #endregion
    }
}
#endif