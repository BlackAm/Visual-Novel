#if !SERVER_DRIVE && APPLY_PPS
using System;
using UnityEngine.Rendering.PostProcessing;

namespace k514
{
    public abstract class PostProcessObjectBase : PrefabInstance
    {
        #region <Fields>

        protected PostProcessVolume _Volume;

        #endregion
        
        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            _Volume = GetComponent<PostProcessVolume>();
            CameraManager.GetInstanceUnSafe._PPS_ProcessorGroup.Add(this);
            gameObject.TurnLayerTo(GameManager.GameLayerType.PostProcessVolume, false);
        }

        public override void OnPooling()
        {
            base.OnPooling();
            SetVolumeEnable(true);
        }

        public override void OnRetrieved()
        {
            CameraManager.GetInstanceUnSafe?._PPS_ProcessorGroup.Remove(this);

            base.OnRetrieved();
        }

        #endregion

        #region <Methods>

        public void SetVolumeEnable(bool p_Enable)
        {
            _Volume.enabled = p_Enable;
        }

        #endregion
    }

    public struct PostProcessObjectPreset
    {
        public string PrefabName;
        public Type PostProcessObjectBaseComponent;

        public PostProcessObjectPreset(string p_PrefabName, Type p_PostProcessObjectBaseComponent)
        {
            PrefabName = p_PrefabName;
            PostProcessObjectBaseComponent = p_PostProcessObjectBaseComponent;
        }
    }
}
#endif