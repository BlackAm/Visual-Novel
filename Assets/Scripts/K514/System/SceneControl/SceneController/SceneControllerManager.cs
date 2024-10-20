using UnityEngine;

namespace BlackAm
{
    public partial class SceneControllerManager : Singleton<SceneControllerManager>
    {
        #region <Consts>

        private const int _DefaultSceneTransitionDelay = 1000;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 현재 페이즈
        /// </summary>
        private SceneControlPhase _CurrentPhase;
        
        /// <summary>
        /// 씬 프리셋 인식자 종자값
        /// </summary>
        private int __RoomSerialId;

        /// <summary>
        /// 로딩 씬을 기준으로 마지막으로 로드했던 씬의 정보
        /// </summary>
        public SceneControlPreset PrevSceneControlPreset { get; private set; }

        /// <summary>
        /// 로딩 씬을 기준으로 현재 로드해야할 씬의 정보
        /// </summary>
        public SceneControlPreset CurrentSceneControlPreset { get; private set; }

        /// <summary>
        /// 로드할 씬에 배치할 플레이어 정보
        /// </summary>
        private SceneControlPlayerPreset PlayerDeployPreset;
        
        #endregion

        #region <Enums>

        private enum SceneControlPhase
        {
            None,
            Reserved,
            TransitionScene,
        }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
        }

        public override void OnInitiate()
        {
        }

        public void OnSceneLoadOver()
        {
            _CurrentPhase = SceneControlPhase.None;
            SystemBoot.GetInstance.OnSceneStarted();
        }

        #endregion

        #region <Methods>

        private void UpdateScenePreset(SceneControlPreset p_SceneControlPreset, SceneControllerTool.ScenePreset p_ScenePreset, int p_SceneVariableIndex)
        {
            PrevSceneControlPreset = CurrentSceneControlPreset;
            p_SceneControlPreset.SetScenePreset(++__RoomSerialId, p_ScenePreset, p_SceneVariableIndex);
            CurrentSceneControlPreset = p_SceneControlPreset;
        }

        public string GetCurrentSceneFullPath()
        {
            return CurrentSceneControlPreset.ScenePreset.SceneFullPath;
        }

        public bool IsSceneStable()
        {
            return _CurrentPhase != SceneControlPhase.TransitionScene;
        }

        public bool IsFirstSceneTransition()
        {
            return CurrentSceneControlPreset.SceneControlFlagMask.HasAnyFlagExceptNone(SceneControllerTool
                .SceneControlFlag.SystemBootInvoke);
        }
  
        #endregion
    }

    public struct SceneControlPreset
    {
        #region <Fields>

        /// <summary>
        /// 해당 씬 전이 id
        /// </summary>
        public int Id;

        /// <summary>
        /// 씬 로딩 연출 타입
        /// </summary>
        public SceneControllerTool.LoadingSceneType LoadingSceneType;

        /// <summary>
        /// 해당 씬 전이 플래그
        /// </summary>
        public SceneControllerTool.SceneControlFlag SceneControlFlagMask;

        /// <summary>
        /// 해당 씬 전이에 의해 플레이어 제어가 필요한 경우 사용하는 프리셋
        /// </summary>
        private SceneControlPlayerPreset SceneControlPlayerPreset;

        /// <summary>
        /// 해당 씬 프리셋
        /// </summary>
        public SceneControllerTool.ScenePreset ScenePreset;
        
        /// <summary>
        /// 씬에 적용할 변수 설정 레코드 인덱스
        /// </summary>
        private int _SceneEntryIndex;
        
        /// <summary>
        /// 씬에 적용할 변수 설정 리스트 인덱스
        /// </summary>
        private int _SceneVariableListIndex;
        
        #endregion

        #region <Constructor>

        public SceneControlPreset(SceneControllerTool.LoadingSceneType p_LoadingSceneType) : this(p_LoadingSceneType, SceneControllerTool.SceneControlFlag.None)
        {
        }
        
        public SceneControlPreset(SceneControllerTool.SceneControlFlag p_SceneControlFlagMask) : this(SceneControllerTool.LoadingSceneType.Black, p_SceneControlFlagMask)
        {
        }
        
        public SceneControlPreset(SceneControlPlayerPreset p_SceneControlPlayerPreset) : this(SceneControllerTool.LoadingSceneType.Black, default, p_SceneControlPlayerPreset)
        {
        }
        
        public SceneControlPreset(SceneControllerTool.LoadingSceneType p_LoadingSceneType, SceneControllerTool.SceneControlFlag p_SceneControlFlagMask)
        {
            Id = 0;
            LoadingSceneType = p_LoadingSceneType;
            SceneControlFlagMask = p_SceneControlFlagMask;
            SceneControlPlayerPreset = default;
            ScenePreset = default;

            _SceneEntryIndex = default;
            _SceneVariableListIndex = default;
        }
       
        public SceneControlPreset(SceneControllerTool.LoadingSceneType p_LoadingSceneType, SceneControlPlayerPreset p_SceneControlPlayerPreset) : this(p_LoadingSceneType, SceneControllerTool.SceneControlFlag.None, p_SceneControlPlayerPreset)
        {
        }
        
        public SceneControlPreset(SceneControllerTool.LoadingSceneType p_LoadingSceneType, SceneControllerTool.SceneControlFlag p_SceneControlFlagMask, SceneControlPlayerPreset p_SceneControlPlayerPreset)
        {
            Id = 0;
            LoadingSceneType = p_LoadingSceneType;
            SceneControlFlagMask = p_SceneControlFlagMask | SceneControllerTool.SceneControlFlag.HasSceneControlPlayer;
            SceneControlPlayerPreset = p_SceneControlPlayerPreset;
            ScenePreset = default;
            
            _SceneEntryIndex = default;
            _SceneVariableListIndex = default;
        }
        
        #endregion
        
        #region <Operator>

        public static implicit operator SceneControlPreset(SceneControllerTool.LoadingSceneType p_LoadingSceneType)
        {
            return new SceneControlPreset(p_LoadingSceneType);
        }

        public static implicit operator SceneControlPreset(SceneControllerTool.SceneControlFlag p_SceneControlFlagMask)
        {
            return new SceneControlPreset(p_SceneControlFlagMask);
        }

        public static implicit operator SceneControlPreset(SceneControlPlayerPreset p_SceneControlPlayerPreset)
        {
            return new SceneControlPreset(p_SceneControlPlayerPreset);
        }

        public static implicit operator SceneControlPreset((SceneControllerTool.LoadingSceneType t_LoadingSceneType, SceneControllerTool.SceneControlFlag t_SceneControlFlagMask) p_Tuple)
        {
            return new SceneControlPreset(p_Tuple.t_LoadingSceneType, p_Tuple.t_SceneControlFlagMask);
        }
        
        public static implicit operator SceneControlPreset((SceneControllerTool.LoadingSceneType t_LoadingSceneType, SceneControlPlayerPreset t_SceneControlPlayerPreset) p_Tuple)
        {
            return new SceneControlPreset(p_Tuple.t_LoadingSceneType, p_Tuple.t_SceneControlPlayerPreset);
        }
        
        public static implicit operator SceneControlPreset((SceneControllerTool.LoadingSceneType t_LoadingSceneType, SceneControllerTool.SceneControlFlag t_SceneControlFlagMask, SceneControlPlayerPreset t_SceneControlPlayerPreset) p_Tuple)
        {
            return new SceneControlPreset(p_Tuple.t_LoadingSceneType, p_Tuple.t_SceneControlFlagMask, p_Tuple.t_SceneControlPlayerPreset);
        }

#if UNITY_EDITOR
        public override string ToString()
        {
            return $"id : {Id}, Scene : {ScenePreset}";
        }
#endif

        #endregion

        #region <Methods>

        public void SetScenePreset(int p_Id, SceneControllerTool.ScenePreset p_ScenePreset, int p_SceneVariableListIndex)
        {
            Id = p_Id;
            ScenePreset = p_ScenePreset;
            _SceneVariableListIndex = p_SceneVariableListIndex;
        }

        public void SetSceneEntryIndex(int p_Index)
        {
            SceneControlFlagMask.AddFlag(SceneControllerTool.SceneControlFlag.HasSceneEntryIndex);
            _SceneEntryIndex = p_Index;
        }

        public int GetSceneVariableListIndex()
        {
            return _SceneVariableListIndex;
        }

        public (bool, int) TryGetSceneEntryIndex()
        {
            if (SceneControlFlagMask.HasAnyFlagExceptNone(SceneControllerTool.SceneControlFlag.HasSceneEntryIndex))
            {
                return (true, _SceneEntryIndex);
            }
            else
            {
                return default;
            }
        }

        public (bool, SceneControlPlayerPreset) GetPlayerControl()
        {
            if (SceneControlFlagMask.HasAnyFlagExceptNone(SceneControllerTool.SceneControlFlag.HasSceneControlPlayer))
            {
                return (true, SceneControlPlayerPreset);
            }
            else
            {
                return default;
            }
        }

        #endregion
    }

    public struct SceneControlPlayerPreset
    {
        #region <Fields>

        public Vector3 StartPosition;
        public Vector3 StartRotation;
        
        #endregion

        #region <Constructor>

        public SceneControlPlayerPreset(Vector3 p_StartPosition, Vector3 p_StartRotation)
        {
            StartPosition = p_StartPosition;
            StartRotation = p_StartRotation;
        }

        #endregion
    }
}