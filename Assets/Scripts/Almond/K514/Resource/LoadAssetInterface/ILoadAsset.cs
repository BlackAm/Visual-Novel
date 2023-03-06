using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 유니티 에셋을 로드하는 모듈의 공통 인터페이스
    /// </summary>
    public interface ILoadAsset
    {
        /// <summary>
        /// 최초 생성 시점에 수행하고자 하는 초기화 콜백을 기술할 것.
        /// </summary>
        void OnCreated();

        /// <summary>
        /// 에셋을 로드하도록 기술할 것.
        /// </summary>
        T LoadAsset<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object;

        /// <summary>
        /// 에셋을 비동기 로드하도록 기술할 것.
        /// </summary>
        UniTask<T> LoadAssetAsync<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object;
        
        /// <summary>
        /// 멀티 에셋을 로드하도록 기술할 것.
        /// </summary>
        T[] LoadMultipleAsset<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_FullPath) where T : Object;

        /// <summary>
        /// 멀티 에셋을 비동기 로드하도록 기술할 것.
        ///
        /// 유니티 멀티 에셋 로드는 LoadAll 메서드를 통해서 이루어지는데
        /// 해당 메서드는 유니티 메인스레드에서만 동작하므로 해당 인터페이스는 구현될 수 없다.
        /// </summary>
        UniTask<T[]> LoadMultipleAssetAsync<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_FullPath) where T : Object;
        
        /// <summary>
        /// 지정한 수명 타입을 가지는 에셋을 메모리로부터 릴리스 시킨다.
        /// </summary>
        void UnloadAsset(ResourceLifeCycleType p_ResourceLifeCycleType);

        /// <summary>
        /// 지정한 오브젝트를 릴리스 시킨다.
        /// </summary>
        void UnloadAsset(AssetPreset p_AssetPreset);
        
        /// <summary>
        /// 로드된 에셋번들 내부에 존재하는 씬의 이름을 리턴하는 메서드
        /// </summary>
        UniTask<string> LoadSceneAsset(ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName);

        /// <summary>
        /// 특정 에셋 번들에 등록된 씬들의 이름을 리스트로 리턴하는 메서드
        /// </summary>
        UniTask<string[]> LoadAllSceneAsset(ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName);
    }
    
    /// <summary>
    /// 로드된 에셋의 정보를 담는 구조체
    /// </summary>
    public struct AssetPreset
    {
        #region <Fields>

        /// <summary>
        /// 로드된 단일 에셋, 멀티 에셋의 경우 null
        /// </summary>
        public Object Asset { get; private set; }
        
        /// <summary>
        /// 로드된 멀티 에셋, 단일 에셋의 경우 null
        /// </summary>
        public Object[] MultiAsset { get; private set; }
        
        /// <summary>
        /// 로드된 에셋의 이름. 확장자를 포함
        /// </summary>
        public string AssetName { get; private set; }
        
        /// <summary>
        /// 에셋 분류 타입
        /// </summary>
        public ResourceType ResourceType { get; private set; }
        
        /// <summary>
        /// 해당 에셋의 수명 타입
        /// </summary>
        public ResourceLifeCycleType ResourceLifeCycleType { get; private set; }

        /// <summary>
        /// 해당 에셋이 멀티 에셋인지 표시하는 플래그
        /// </summary>
        public bool IsMultiAsset;
        
        /// <summary>
        /// 해당 구조체의 유효성
        /// </summary>
        public bool IsValid;
        
        #endregion

        #region <Constructors>

        public AssetPreset(Object p_Asset, string p_AssetName, ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
            Asset = p_Asset;
            MultiAsset = null;
            AssetName = p_AssetName;
            ResourceType = p_ResourceType;
            ResourceLifeCycleType = p_ResourceLifeCycleType;
            IsMultiAsset = false;
            IsValid = true;
        }
        
        public AssetPreset(Object[] p_Asset, string p_AssetName, ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
            Asset = null;
            MultiAsset = p_Asset;
            AssetName = p_AssetName;
            ResourceType = p_ResourceType;
            ResourceLifeCycleType = p_ResourceLifeCycleType;
            IsMultiAsset = true;
            IsValid = true;
        }

        #endregion
    }
}