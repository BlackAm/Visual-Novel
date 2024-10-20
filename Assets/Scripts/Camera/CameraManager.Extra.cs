#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    public partial class CameraManager
    {
        #region <Callbacks>

        private void OnCreateExtraPartial()
        {
            
        }

        #endregion
        
        #region <Methods>

        public Vector3 GetHorizontalRotation()
        {
            return _ViewControlRotationWrapper.localRotation.eulerAngles;
        }

#if UNITY_EDITOR
        public void Set_AffineFeature_From_CurrentMapSetting(SceneVariableData.TableRecord CurrentSceneVariableRecord)
        {
            _CurrentSceneVariableRecord = CurrentSceneVariableRecord;
            _ViewControlAffineTransformObject.SetAffineTransform(_CurrentSceneVariableRecord);
        }
         
        public void ApplyEditorMapSetting()
        {
            _CurrentSceneCameraConfigure = CameraConfigureData.GetInstanceUnSafe.GetTableData(0);
            UpdateAffineSetting();
#if APPLY_PPS
            UpdatePostProcessStackSetting();
#endif
        }
#endif
                
        /// <summary>
        /// 해당 번호의 마스크만 비활성하는 메소드.
        /// </summary>
        /// <param name="target">GameManager.GameLayerMaskType의 값</param>
        /// <param name="active">활성/비활성 유무</param>
        public void ActiveMask(int target)
        {
            MainCamera.cullingMask = ~(1 << target);
        }
        
        #endregion
    }
}
#endif