using System;

namespace k514
{
    public enum TaskPhase
    {
        None,
        TaskProgressing,
        TaskTerminate,
    }
    
    /// <summary>
    /// 경로 파라미터의 타입
    /// </summary>
    public enum PathType
    {
        /// <summary>
        /// 플랫폼 별 절대경로
        /// </summary>
        SystemGenerate_AbsolutePath,
        
        /// <summary>
        /// 유니티 Asset 폴더를 기준으로 한 상대 경로
        /// </summary>
        SystemGenerate_RelativePath
    }

    /// <summary>
    /// 리소스 타입
    /// </summary>
    [Flags]
    public enum ResourceType
    {
        /// <summary>
        /// 리소스 구분 없음, 유효하지 않은 타입
        /// </summary>
        None = 0,
        
        /// <summary>
        /// 테이블
        /// </summary>
        Table = 1 << 0,
        
        /// <summary>
        /// 해당 타입은 일종의 메타 타입으로
        /// 위의 타입들이 AssetBundle 타입의 서브 타입으로 활용될 수 있다.
        /// </summary>
        AssetBundle = 1 << 1,
        
        /// <summary>
        /// 유니티 텍스트 에셋
        /// </summary>
        TextAsset = 1 << 2,
        
        /// <summary>
        /// 프리팹 오브젝트
        /// </summary>
        GameObjectPrefab = 1 << 3,
        
        /// <summary>
        /// 파티클 시스템이 주가되는 프리팹 오브젝트, 아마도 공통적으로 Vfx에 관한 스크립트를 가지고 있도록 구현함.
        /// </summary>
        VfxPrefab = 1 << 4,
        
        /// <summary>
        /// 이미지 리소스
        /// </summary>
        Image = 1 << 5,
        
        /// <summary>
        /// 머티리얼 리소스
        /// </summary>
        Material = 1 << 6,
                
        /// <summary>
        /// 미분류 리소스
        /// </summary>
        Misc = 1 << 7,
        
        /// <summary>
        /// 씬 오브젝트
        /// 씬의 경우에 유니티 리소스 폴더에서 로드하는게 아니라, 자동적으로 유니티의 SceneManager에서 관리하기 때문에
        /// 넣지 않아도 되지만,
        /// 에셋 번들에서는 다른 오브젝트와 씬을 구분해서 관리할 필요가 있기에 타입으로 관리한다.
        /// </summary>
        Scene = 1 << 8,
        
        /// <summary>
        /// 3D 모델 등, fbx 및 머티리얼 셰이더 텍스쳐를 포함하는 리소스
        /// </summary>
        Model = 1 << 9,
        
        /// <summary>
        /// 음원 에셋을 기술하는 리소스 타입
        /// </summary>
        AudioClip = 1 << 10,
        
        /// <summary>
        /// 영상 에셋을 기술하는 리소스 타입
        /// </summary>
        VideoClip = 1 << 11,
        
        /// <summary>
        /// 에셋번들이 되지 않는 리소스 타입
        /// 즉 빌드에 항상 포함되어야 하는 리소스 타입
        /// </summary>
        Dependencies = 1 << 15,
    }

    /// <summary>
    /// 빌드에 반드시 포함되는 의존성을 가지는 리소스 하위 타입
    /// </summary>
    public enum DependencyResourceSubType
    {
        None,
        GameObject,
        AudioClip,
        VideoClip,
        TextAsset,
        Texture,
        Sprite,
        AudioMixer,
        RenderTexture,
        AnimationClip,
        SystemTable,
        RuntimeAnimatorController,
        Shader,
        Misc,
    }

    /// <summary>
    /// 로드한 리소스의 수명 타입
    /// </summary>
    [Flags]
    public enum ResourceLifeCycleType
    {
        /// <summary>
        /// 아직 로드되지 않은 상태
        /// </summary>
        None = 0,

        /// <summary>
        /// 씬 전환시 언로드되는 수명 타입
        /// </summary>
        Scene = 1 << 0,
        
        /// <summary>
        /// 한번 로드되면 게임 수명과 함께하는 에셋 수명 타입
        /// </summary>
        WholeGame = 1 << 1,
        
        /// <summary>
        /// 특정 조건 하에 언로드되는 타입
        /// </summary>
        Free_Condition = 1 << 2,
    }

    /// <summary>
    /// 에셋 로드 타입
    /// 각 리소스 타입들이 어떤 방식으로 로드되는지에 관한 타입
    /// </summary>
    public enum AssetLoadType
    {
        /// <summary>
        /// 유니티 에셋번들로부터 로드하는 모드
        /// </summary>
        FromAssetBundle,
        
        /// <summary>
        /// 유니티 리소스 폴더에서 로드하는 모드
        /// </summary>
        FromUnityResource,
    }
}