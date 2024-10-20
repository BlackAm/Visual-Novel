using UnityEngine;

namespace BlackAm
{
    /*public partial class Patcher : SceneController<Patcher, Patcher.PatchProgressPhase, AsyncPatchSequence>
    {
        #region <Enums>

        public enum PatchProgressPhase
        {
            None,
            
            CheckVersion,
            CompareVersion,
            GetAssetList,
            PatchFile,

            PatchTerminate,
        }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _SceneType = SceneControllerTool.SystemSceneType.PatchScene;

            base.OnCreated();
        }

        #endregion

        #region <Disposable>
        
        /// <summary>
        /// 해당 오브젝트를 제거한다.
        /// 오브젝트 제거는 매 로딩 종료시에 일어나므로, 해당 파기메서드의 호출은 반드시 게임 시스템의 종료를 의미하는 것은 아니다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
#if !SERVER_DRIVE
            _ForeGroundAnimation.Dispose();
            _Y1Animation.Dispose();
            _Y2Animation.Dispose();
            _SAnimation.Dispose();
            _PAnimation.Dispose();
            _RAnimation.Dispose();
            _KAnimation.Dispose();
            _NAnimation.Dispose();
            _YAnimation.Dispose();
            
            if (_FG_SpriteSheet != null)
            {
                _FG_SpriteSheet.Dispose();
                _FG_SpriteSheet = null;
            }
            
            if (_BG_Character_SpriteSheet != null)
            {
                _BG_Character_SpriteSheet.Dispose();
                _BG_Character_SpriteSheet = null;
            }
#endif
            AsyncPatchTaskRequestManager.GetInstance?.Dispose();
            
            base.DisposeUnManaged();
            
#if UNITY_EDITOR
            Debug.LogWarning("Notice : 패치 씬이 파기되었습니다.");
#endif
        }

        #endregion
    }*/
}