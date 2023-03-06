#if UNITY_EDITOR && ON_GUI
using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>

        private string AssetName;
        private AssetPreset LoadedAsset;
        private AssetPreset MultiLoadedAsset;
        
        private 
        
        #endregion

        #region <Callbacks>

        void OnAwakeAssetLoad()
        {
            var targetControlType = TestControlType.AssetLoadTest;
            
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, LoadAsset, "로드", "Gary_000.png 스프라이트를 로드");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, LoadAssetAsync, "비동기로드", "Gary_000.png 스프라이트를 비동기 로드");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.E, UnloadAsset, "언로드", "로드된 스프라이트를 언로드");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.A, LoadMultiAsset, "로드", "Jewel.png 멀티 스프라이트를 로드");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.S, LoadMultiAssetAsync, "비동기로드", "Jewel.png 멀티 스프라이트를 비동기 로드");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.D, UnloadMultiAsset, "언로드", "로드된 멀티 스프라이트를 언로드");
        }

        #endregion

        #region <Methods>
        
        private void LoadAsset(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease && !LoadedAsset.IsValid)
            {
                var resultTuple = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.WholeGame, "Gary_000.png");
                var sprite = resultTuple.Item2;
                LoadedAsset = resultTuple.Item1;
                
                if (sprite != null)
                {
                    Debug.Log($"에셋이름 [{LoadedAsset.AssetName}] / 사이즈 [{sprite.rect}]");
                }
            }
        }
        
        private async void LoadAssetAsync(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease && !LoadedAsset.IsValid)
            {
                var resultTuple = await LoadAssetManager.GetInstanceUnSafe.LoadAssetAsync<Sprite>(ResourceType.Image, ResourceLifeCycleType.WholeGame, "Gary_000.png");
                var sprite = resultTuple.Item2;
                LoadedAsset = resultTuple.Item1;
                
                if (sprite != null)
                {
                    Debug.Log($"에셋이름 [{LoadedAsset.AssetName}] / 사이즈 [{sprite.rect}]");
                }
            }
        }

        private void UnloadAsset(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease && LoadedAsset.IsValid)
            {
                LoadAssetManager.GetInstanceUnSafe.UnloadAsset(LoadedAsset);
                LoadedAsset = default;
            }
        }
        
        private void LoadMultiAsset(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease && !MultiLoadedAsset.IsValid)
            {
                var resultTuple = LoadAssetManager.GetInstanceUnSafe.LoadMultipleAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.WholeGame, "Jewel.png");
                var spriteArray = resultTuple.Item2;
                MultiLoadedAsset = resultTuple.Item1;

                if (spriteArray != null)
                {
                    Debug.Log($"에셋이름 [{MultiLoadedAsset.AssetName}] / 사이즈 [{spriteArray.Length}]");
                }
            }
        }
        
        private async void LoadMultiAssetAsync(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease && !MultiLoadedAsset.IsValid)
            {
                var resultTuple = await LoadAssetManager.GetInstanceUnSafe.LoadMultipleAssetAsync<Sprite>(ResourceType.Image, ResourceLifeCycleType.WholeGame, "Jewel.png");
                var spriteArray = resultTuple.Item2;
                MultiLoadedAsset = resultTuple.Item1;

                if (spriteArray != null)
                {
                    Debug.Log($"에셋이름 [{MultiLoadedAsset.AssetName}] / 사이즈 [{spriteArray.Length}]");
                }
            }
        }
        
        private void UnloadMultiAsset(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease && MultiLoadedAsset.IsValid)
            {
                LoadAssetManager.GetInstanceUnSafe.UnloadAsset(MultiLoadedAsset);
                MultiLoadedAsset = default;
            }
        }

        #endregion
    }
}
#endif