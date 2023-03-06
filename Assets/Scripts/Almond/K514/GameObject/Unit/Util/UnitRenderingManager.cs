using Cysharp.Threading.Tasks;

namespace k514
{
    public class UnitRenderingManager : SceneChangeEventSingleton<UnitRenderingManager>
    {
        #region <Consts>
   
        /// <summary>
        /// 유닛 스턴시 스폰할 Vfx 스폰 인덱스
        /// </summary>
        public const int __UNIT_STUN_VFX_SPAWN_INDEX = 50000;
         
        /// <summary>
        /// 유닛 중독시 스폰할 Vfx 스폰 인덱스
        /// </summary>
        public const int __UNIT_POISON_VFX_SPAWN_INDEX = 0;//6은 해당 인덱스가 존재하지 않으므로 0으로 대체
 
        /// <summary>
        /// 유닛 출혈시 스폰할 Vfx 스폰 인덱스
        /// </summary>
        public const int __UNIT_BLEEDING_VFX_SPAWN_INDEX = 0;//6은 해당 인덱스가 존재하지 않으므로 0으로 대체

        /// <summary>
        /// 유닛 빙결시 스폰할 Vfx 스폰 인덱스
        /// </summary>
        public const int __UNIT_FREEZING_VFX_SPAWN_INDEX = 50009;
 
        /// <summary>
        /// 프리로드할 갯수
        /// </summary>
        private const int __UNIT_RENDERING_VFX_PRELOAD_COUNT = 8;
           
        /// <summary>
        /// 프리로드할 Vfx 인덱스 셋
        /// </summary>
        public static readonly int[] __UNIT_RENDERING_VFX_PRELOAD_INDEX_SET =
            {__UNIT_STUN_VFX_SPAWN_INDEX, __UNIT_POISON_VFX_SPAWN_INDEX, __UNIT_BLEEDING_VFX_SPAWN_INDEX, __UNIT_FREEZING_VFX_SPAWN_INDEX};
           
        #endregion
           
        #region <Fields>
   
           
   
        #endregion
           
        #region <Callbacks>

        protected override void OnCreated()
        {
        }
   
        public override void OnInitiate()
        {
        }
   
        public override async UniTask OnScenePreload()
        {
            await UniTask.SwitchToMainThread();
            foreach (var vfxIndex in __UNIT_RENDERING_VFX_PRELOAD_INDEX_SET)
            {
                VfxSpawnManager.GetInstance.Preload(vfxIndex, __UNIT_RENDERING_VFX_PRELOAD_COUNT);
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
   
        #endregion
   
        #region <Methods>
   
        public (bool, VFXUnit) CastUnitAttachedStunVfx(Unit p_Target, TransformTool.AffineCachePreset p_AffineCachePreset, 
            uint p_PreDelay = 0)
        {
            return CastUnitAttachedVfx(__UNIT_STUN_VFX_SPAWN_INDEX, p_Target, p_AffineCachePreset, p_PreDelay);
        }
           
        public (bool, VFXUnit) CastUnitAttachedPoisonVfx(Unit p_Target, TransformTool.AffineCachePreset p_AffineCachePreset, 
            uint p_PreDelay = 0)
        {
            return CastUnitAttachedVfx(__UNIT_POISON_VFX_SPAWN_INDEX, p_Target, p_AffineCachePreset, p_PreDelay);
        }
           
        public (bool, VFXUnit) CastUnitAttachedBleedVfx(Unit p_Target, TransformTool.AffineCachePreset p_AffineCachePreset, 
            uint p_PreDelay = 0)
        {
            return CastUnitAttachedVfx(__UNIT_BLEEDING_VFX_SPAWN_INDEX, p_Target, p_AffineCachePreset, p_PreDelay);
        }
           
        public (bool, VFXUnit) CastUnitAttachedVfx(int p_Index, Unit p_Target, TransformTool.AffineCachePreset p_AffineCachePreset, 
            uint p_PreDelay = 0)
        {
            var (isValid, spawned) = VfxSpawnManager.GetInstance.CastVfx<VFXUnit>(p_Index, p_AffineCachePreset,
                ResourceLifeCycleType.Scene, ObjectDeployTool.ObjectDeploySurfaceDeployType.None, p_PreDelay,
                false);
   
            if (isValid)
            {
                spawned.SetAttach(p_Target);
                spawned.SetPlay(p_PreDelay);
            }
               
            return (isValid, spawned);
        }
   
        #endregion
    }
}