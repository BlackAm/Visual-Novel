using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public partial class SceneControllerManager
    {
        #region <Methods>
              
        private async void TurnSceneTo(SceneControllerTool.ScenePreset p_ScenePreset, int p_SceneVariableIndex, SceneControlPreset p_SceneControlPreset = default)
        {
            switch (_CurrentPhase)
            {
                case SceneControlPhase.None:
                    _CurrentPhase = SceneControlPhase.Reserved;
                    UpdateScenePreset(p_SceneControlPreset, p_ScenePreset, p_SceneVariableIndex);

                    if (IsFirstSceneTransition())
                    {
                        await UniTask.Delay(_DefaultSceneTransitionDelay);
                    }
                    else
                    {
                        SystemBoot.GetInstance.OnSceneTerminated();
                        await UniTask.Delay(_DefaultSceneTransitionDelay);
                        SystemBoot.GetInstance.OnSceneTransition();
                    }
                    
                    _CurrentPhase = SceneControlPhase.TransitionScene;
                    SceneControllerTool.LoadSystemScene();
                    break;
                case SceneControlPhase.Reserved:
                case SceneControlPhase.TransitionScene:
                    break;
            } 
        }

        /// <summary>
        /// 지정한 씬 이름을 가지는 씬을 지정한 VariableIndex 세팅으로 로드하는 메서드
        /// 씬 이름은 경로를 생략하고 파일명.unity 를 입력한다.
        /// </summary>
        public void TurnSceneTo(string p_SceneName, int p_VariableIndex, SceneControlPreset p_SceneControlPreset = default)
        {
            var scenePreset = SceneDataRoot.GetInstanceUnSafe[p_SceneName];
            TurnSceneTo(scenePreset, p_VariableIndex, p_SceneControlPreset);
        }

        /// <summary>
        /// 지정한 씬 타입에 대응하는 테이블의 서수에 따라 특정 인덱스의 씬을 로드시키는 메서드
        /// </summary>
        public void TurnSceneTo(SceneDataRoot.SceneDataType p_Type, int p_OrdinalIndex, int p_VariableIndex, SceneControlPreset p_SceneControlPreset = default)
        {
            var sceneName = SceneDataRoot.GetInstanceUnSafe[p_Type, p_OrdinalIndex].SceneName;
            TurnSceneTo(sceneName, p_VariableIndex, p_SceneControlPreset);
        }
        
        /// <summary>
        /// 지정한 인덱스를 가진 씬을 지정한 VariableIndex 세팅으로 로드하는 메서드
        /// </summary>
        public void TurnSceneTo(int p_SceneIndex, int p_VariableIndex, SceneControlPreset p_SceneControlPreset = default)
        {
            var scenePreset = SceneDataRoot.GetInstanceUnSafe[p_SceneIndex];
            TurnSceneTo(scenePreset, p_VariableIndex, p_SceneControlPreset);
        }
        
        /// <summary>
        /// 지정한 인덱스를 가진 SceneEntry 테이블을 기준으로 씬을 로드하는 메서드
        /// </summary>
        public void TurnSceneTo(int p_EntryIndex, SceneControlPreset p_SceneControlPreset = default)
        {
            var sceneEntryPreset = SceneEntryData.GetInstanceUnSafe[p_EntryIndex];
            p_SceneControlPreset.SetSceneEntryIndex(p_EntryIndex);
            TurnSceneTo(sceneEntryPreset.SceneSettingIndex, sceneEntryPreset.SceneVariableListIndex, p_SceneControlPreset);
        }
        
        /// <summary>
        /// 지정한 씬 타입에 대응하는 테이블의 서수에 따라 특정 인덱스의 씬을 로드시키는 메서드
        /// </summary>
        public void TurnSceneTo(SceneControllerTool.SceneControllerShortCutType p_SceneType, SceneControlPreset p_SceneControlPreset = default)
        {
            switch (p_SceneType)
            {
                case SceneControllerTool.SceneControllerShortCutType.LoginScene:
                    TurnSceneTo(SceneDataRoot.SceneDataType.SystemScene, 1, 0, p_SceneControlPreset);
                    break;
                case SceneControllerTool.SceneControllerShortCutType.CharacterSelectScene:
                    TurnSceneTo(SceneDataRoot.SceneDataType.SystemScene, 2, 0, p_SceneControlPreset);
                    break;
                case SceneControllerTool.SceneControllerShortCutType.MainHomeScene:
                    TurnSceneTo(SceneDataRoot.SceneDataType.SystemScene, 1, 0, p_SceneControlPreset);
                    break;
            }
        }
        
        #endregion
    }
}