using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 유니티 오브젝트를 대상으로 하는 정적 메서드가 모인 클래스
    /// </summary>
    public static class GameObjectTool
    {
        #region <Methods>

        public static bool IsValid(this GameObject p_Target)
        {
            return !ReferenceEquals(null, p_Target);
        }
        
        /// <summary>
        /// activeSelf 체크 및 SetActive를 수행하는 메서드
        /// </summary>
        public static void SetActiveSafe(this GameObject p_Target, bool p_Flag)
        {
            if (p_Target.activeSelf != p_Flag)
            {
                p_Target.SetActive(p_Flag);
            }
        }

        public static Camera GetDefaultCamera(this Transform p_Transform, CameraClearFlags p_CameraClearFlag, Color p_TargetSolidColor)
        {
            var cameraObject = new GameObject();
            cameraObject.transform.SetParent(p_Transform, false);
            cameraObject.transform.localPosition = Vector3.zero;
            
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = p_CameraClearFlag;
            camera.backgroundColor = p_TargetSolidColor;

            return camera;
        }

        public static Camera GetBlackListDOFMaskCamera(this Transform p_Transform, CameraClearFlags p_CameraClearFlag, int cullingLayer)
        {
            var cameraObject = new GameObject();
            cameraObject.transform.SetParent(p_Transform, false);
            cameraObject.transform.localPosition = Vector3.zero;
            
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = p_CameraClearFlag;
            camera.cullingMask = cullingLayer;

            return camera;
        }

        public static void TurnLayerTo(this GameObject p_TargetObject, GameManager.GameLayerType p_LayerType, bool p_TurnChild)
        {
            var targetLayer = (int)p_LayerType;
            if (p_TurnChild)
            {
                var cjildGameObjectGroup = p_TargetObject.transform.GetComponentsInChildren<Transform>();
                foreach (var prefabObject in cjildGameObjectGroup)
                {
                    prefabObject.gameObject.layer = targetLayer;
                }
            }
            else
            {
                p_TargetObject.layer = targetLayer;
            }
        }
        
        /// <summary>
        /// 지정한 오브젝트가 Disposable이라면 해당 오브젝트의 Disposed 키를 재생성해주는 메서드
        /// </summary>
        public static void RejuvenateDisposable<T>(this T p_Object) where T : _IDisposable
        {
            if (SingletonTool.IsDisposable(p_Object.GetType()))
            {
                p_Object.Rejunvenate();
            }
        }

        #endregion
    }
}