using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 각 씬 별로 존재하는 초기화나 전용 씬 기믹, 사용할 리소스 등을 기술하는 공통 클래스
    /// </summary>
    public abstract class SceneEnvironment : MonoBehaviour, ISceneChange
    {
        #region <Callbacks>

        /// <summary>
        /// 로딩 과정에서, 전이할 씬에서 해당 객체를 찾았을 때 호출되는 전처리 콜백
        /// </summary>
        public virtual async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
            
#if UNITY_EDITOR
            if (CustomDebug.PrintSceneEnvironment)
            {
                Debug.Log($"[{name}] Scene Environment Loading !");
            }
#endif
        }

        /// <summary>
        /// 로딩 과정이 끝난 경우 호출되는 콜백
        /// </summary>
        public virtual void OnSceneStarted()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintSceneEnvironment)
            {
                Debug.Log($"[{name}] Scene Environment Rizap !");
            }
#endif
        }

        /// <summary>
        /// 해당 씬이 종료되는 경우 호출되는 콜백
        /// </summary>
        public virtual void OnSceneTerminated()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintSceneEnvironment)
            {
                Debug.Log($"[{name}] Scene Environment Disposed");
            }
#endif
        }
        
        /// <summary>
        /// 씬이 종료되어 로딩씬으로 전이될 때 호출되는 콜백
        /// </summary>
        public virtual void OnSceneTransition()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintSceneEnvironment)
            {
                Debug.Log($"[{name}] Scene Environment Transition to LoadingScene");
            }
#endif
        }
        
        #endregion
    }
}