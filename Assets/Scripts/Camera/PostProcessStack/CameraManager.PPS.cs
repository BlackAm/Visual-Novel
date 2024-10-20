#if !SERVER_DRIVE && APPLY_PPS

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace BlackAm
{
    public partial class CameraManager
    {
        #region <Consts>

        private const string __Default_PPS_Resources_Name = "PostProcessResources.asset";

        #endregion
        
        #region <Fields>

        private PostProcessLayer _PPS_Layer;
        private float _OPenGLVersion;

        /// <summary>
        /// 해당 에셋은 PPS Layer가 정적으로 존재하지 않는 상태에서, 프로젝트가 빌드되는 경우
        /// 아마도 빌드에 포함되지 않아서, 런타임에서 AddComponent로 PPS Layer를 생성할 때
        /// 해당 리소스를 못찾아 널포 에러가 뜨는 듯 하다.
        ///
        /// 해당 에셋은 PPS 패키지 폴더 내부에 존재하는데 프로젝트 리소스 폴더로 복붙해서
        /// 로드하는 방식으로 널포 에러를 임시 해결함.
        /// 
        /// </summary>
        private PostProcessResources _PPS_Resources;
        public List<PostProcessObjectBase> _PPS_ProcessorGroup { get; private set; }
        private Transform _PPS_ObjectWrapper;
        
        #endregion

        #region <Callbacks>

        private void OnCreatePPSWrapperPartial()
        {
            _PPS_Layer = MainCamera.gameObject.AddComponent<PostProcessLayer>();
            var resultTuple = LoadAssetManager.GetInstanceUnSafe.LoadAsset<PostProcessResources>(ResourceType.Misc,
                ResourceLifeCycleType.WholeGame, __Default_PPS_Resources_Name);
            _PPS_Resources = resultTuple.Item2;
#if UNITY_EDITOR
            _PPS_Layer.enabled = false;
            if (_PPS_Resources == null)
            {
                Debug.LogError($"PostProcessLayer 를 초기화 시키기 위한 리소스가 존재하지 않습니다.");
            }
#endif
            _PPS_Layer.Init(_PPS_Resources);
            _PPS_ProcessorGroup = new List<PostProcessObjectBase>();
            _OPenGLVersion = OPENGL_ES_VERSION();
            AddPPSLayer(GameManager.GameLayerMaskType.PostProcessVolume);
            SetPPSTrigger(BaseWrapper);
            _PPS_ObjectWrapper = new GameObject("PPSVolume").transform;
            _PPS_ObjectWrapper.SetParent(SystemBoot.GetInstance._Transform, false);
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// PPS 기능을 활성화시키는 메서드
        /// </summary>
        public void SetPPSEnable(bool p_Flag)
        {
#if UNITY_EDITOR
            return;
#endif
#if UNITY_ANDROID || UNITY_IOS
            if (_OPenGLVersion < 3)
            {
                p_Flag = false;
            }
#endif
            _PPS_Layer.enabled = p_Flag;
            foreach (var postProcessObjectBase in _PPS_ProcessorGroup)
            {
                postProcessObjectBase.SetVolumeEnable(p_Flag);
            }
        }
        public float OPENGL_ES_VERSION()
        {
            float version = 0;
            string[] str = SystemInfo.graphicsDeviceVersion.Split(' ');
            for (int i = 0; i < str.Length; i++)
            {
                float.TryParse(str[i], out version);
                if (version > 0)
                {
                    break;
                }
            }
            return version;
        }
        /// <summary>
        /// 볼륨 블랜딩을 수행할 아핀 객체를 지정한다.
        /// </summary>
        private void SetPPSTrigger(Transform p_Transform)
        {
            _PPS_Layer.volumeTrigger = p_Transform;
        }

        private void AddPPSLayer(GameManager.GameLayerMaskType p_LayerType)
        {
            _PPS_Layer.volumeLayer |= (int)p_LayerType;
        }

        private void UpdatePostProcessStackSetting()
        {
#if UNITY_EDITOR
            return;
#endif
            var prefabList = _CurrentSceneVariableRecord.PostProcessorPrefabList;
            if (prefabList != null)
            {
                foreach (var prefabPreset in prefabList)
                {
                    var prefabName = prefabPreset.PrefabName;
                    var targetComponent = prefabPreset.PostProcessObjectBaseComponent;
                    var spawned = PrefabPoolingManager.GetInstance.PoolInstance(prefabName, ResourceLifeCycleType.Scene,
                        ResourceType.GameObjectPrefab, p_PrefabExtraDataPreset: targetComponent).Item1;
                    spawned._Transform.SetParent(_PPS_ObjectWrapper);
                }
            }
            SetPPSEnable(true);
        }

        private Transform CurrentInventoryMapSetting()
        {
            var PrefabName = "New_InventoryCharacter_As_Volume.prefab";
            var PostProcessObjectBaseComponent = typeof(BlackAm.PostProcessGlobalVolume);
            var spawned = PrefabPoolingManager.GetInstance.PoolInstance(PrefabName, ResourceLifeCycleType.Scene,
                ResourceType.GameObjectPrefab, p_PrefabExtraDataPreset: PostProcessObjectBaseComponent).Item1;
            spawned._Transform.SetParent(_PPS_ObjectWrapper);

            spawned.gameObject.SetActive(false);
            return spawned.transform;
        }
#endregion
    }
}
#endif
