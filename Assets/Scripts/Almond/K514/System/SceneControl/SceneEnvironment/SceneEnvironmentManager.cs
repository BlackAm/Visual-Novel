using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace k514
{
    /// <summary>
    /// SceneEnvironment를 제어하는 매니저
    /// </summary>
    public class SceneEnvironmentManager : PropertySceneChangeEventSingleton<SceneVariableChangeEventSender, SceneEnvironmentManager.SceneVariableEventType, SceneVariableData.TableRecord, SceneEnvironmentManager>
    {
        #region <Consts>

        private const string SceneEnvironmentObjectName = "SceneEnvironmentObject";
        private static readonly Type DefaultSceneEnvironmentType = typeof(LamierePlayerDeploySceneEnvironment);
        
        #endregion

        #region <Fields>

        /// <summary>
        /// 현재 씬의 Environment 객체
        /// </summary>
        public SceneEnvironment _CurrentTargetEnvironmentObject { get; private set; }

        /// <summary>
        /// 현재 씬 설정 레코드
        /// </summary>
        public SceneSettingData.TableRecord CurrentSceneSettingRecord;

        /// <summary>
        /// 현재 참조중인 씬 설정의 Variable 레코드 인덱스
        /// </summary>
        private int _CurrentSceneVariableListIndex;
        
        #endregion
        
        #region <Enum>

        [Flags]
        public enum SceneVariableEventType
        {
            None = 0,
            OnSceneVariableChanged = 1 << 0,
        }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            Priority = 150;

#if !SERVER_DRIVE
            SingletonTool.CreateSingleton(typeof(CameraManager));
#endif
        }

        public override void OnInitiate()
        {
        }

        /// <summary>
        /// 씬 로딩 성공 시, 수행할 작업을 수행한다.
        /// </summary>
        public override async UniTask OnScenePreload()
        {
            await UniTask.SwitchToMainThread();
            
            var currentSceneControlPreset = SceneControllerManager.GetInstance.CurrentSceneControlPreset;
            var currentScenePreset = currentSceneControlPreset.ScenePreset;
            
            LoadSceneSetting(currentScenePreset.SceneTableRecordIndex, currentSceneControlPreset.GetSceneVariableListIndex());
           
            // 해당 씬에 컴포넌트가 추가되어 있는지 검색한다.
            _CurrentTargetEnvironmentObject = Object.FindObjectOfType<SceneEnvironment>();
            
            // 로드된 씬에서 컴포넌트를 찾을 수 없는 경우
            if (_CurrentTargetEnvironmentObject == null)
            {
                // 씬 세팅 레코드에서 지정하고 있는 컴포넌트 타입이 존재하는 경우
                var tryIndexingKey = GetSceneVariableData().SceneEnvironmentTypeIndex;
                if (tryIndexingKey != default)
                {
                    var targetTable = SceneEnvironmentTypeIndexingMap.GetInstanceUnSafe.GetTable();
                    if (targetTable.TryGetValue(tryIndexingKey, out var o_Record))
                    {
                        _CurrentTargetEnvironmentObject = new GameObject(SceneEnvironmentObjectName).AddComponent(o_Record.Type) as SceneEnvironment;
                    }
                    // 없다면 디폴트 컴포넌트를 생성
                    else
                    {
                        _CurrentTargetEnvironmentObject = new GameObject(SceneEnvironmentObjectName).AddComponent(DefaultSceneEnvironmentType) as SceneEnvironment;
                    }
                }
                // 없는 경우, 씬 이름에 매핑된 컴포넌트 타입이 있는지 검색한다.
                else
                {
                    var targetTable = SceneEnvironmentTypeMap.GetInstanceUnSafe.GetTable();
                    var trySceneName = currentScenePreset.SceneName;
                    if (targetTable.ContainsKey(trySceneName))
                    {
                        _CurrentTargetEnvironmentObject = new GameObject(SceneEnvironmentObjectName).AddComponent(targetTable[trySceneName].Type) as SceneEnvironment;
                    }
                    // 없다면 디폴트 컴포넌트를 생성
                    else
                    {
                        _CurrentTargetEnvironmentObject = new GameObject(SceneEnvironmentObjectName).AddComponent(DefaultSceneEnvironmentType) as SceneEnvironment;
                    }
                }
            }

            if (!ReferenceEquals(null, _CurrentTargetEnvironmentObject))
            {
                await _CurrentTargetEnvironmentObject.OnScenePreload();
            }
        }

        /// <summary>
        /// 씬 실행 시, 수행할 작업을 수행한다.
        /// </summary>
        public override void OnSceneStarted()
        {
            _CurrentTargetEnvironmentObject.OnSceneStarted();
        }

        /// <summary>
        /// 씬 전이 시, 수행할 작업을 수행한다.
        /// </summary>
        public override void OnSceneTerminated()
        {
            _CurrentTargetEnvironmentObject.OnSceneTerminated();
        }

        /// <summary>
        /// 로딩 씬으로 전이 시, 수행할 작업을 기술한다.
        /// </summary>
        public override void OnSceneTransition()
        {
            _CurrentTargetEnvironmentObject.OnSceneTransition();
            _CurrentTargetEnvironmentObject = null;
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 지정한 인덱스의 씬의 세팅 레코드를 로드하는 메서드
        /// </summary>
        protected void LoadSceneSetting(int p_SceneIndex, int p_Variable)
        {
            var targetMapTable = SceneSettingData.GetInstanceUnSafe.GetTable();
            if (SceneSettingData.GetInstanceUnSafe.GetTable().TryGetValue(p_SceneIndex, out var o_CurrentMapData))
            {
                CurrentSceneSettingRecord = o_CurrentMapData;
            }
            else
            {
                CurrentSceneSettingRecord = targetMapTable[0];
            }
            
            TurnSceneVariable(p_Variable);
        }

        /// <summary>
        /// 지정한 씬 세팅 변위 값을 다음 값으로 변경하는 메서드
        /// </summary>
        public void TurnNextSceneVariable(bool p_PlayBGM)
        {
            TurnSceneVariable(_CurrentSceneVariableListIndex + 1);
#if !SERVER_DRIVE
            if (p_PlayBGM)
            {
                BGMManager.GetInstance.PlayBGM(true);
            }
#endif
        }
        
        /// <summary>
        /// 지정한 씬 세팅 변위 값을 변경하는 메서드
        /// </summary>
        public void TurnSceneVariable(int p_VariableIndex)
        {
            var tryVariableSceneData = CurrentSceneSettingRecord?[p_VariableIndex];
            if (tryVariableSceneData != null)
            {
                _CurrentSceneVariableListIndex = p_VariableIndex;
                TriggerPropertyEvent(SceneVariableEventType.OnSceneVariableChanged, tryVariableSceneData);
            }
        }

        /// <summary>
        /// 지정한 씬 변위 테이블 레코드를 리턴하는 메서드
        /// </summary>
        public SceneVariableData.TableRecord GetSceneVariableData(int p_VariableIndex)
        {
            return CurrentSceneSettingRecord[p_VariableIndex];
        }     
        
        /// <summary>
        /// 현재 선택된 씬 변위 테이블 레코드를 리턴하는 메서드
        /// </summary>
        public SceneVariableData.TableRecord GetSceneVariableData()
        {
            return GetSceneVariableData(_CurrentSceneVariableListIndex);
        }
        
        /// <summary>
        /// 현재 씬에서 플레이어 초기화에 필요한 값을 리턴하는 메서드
        /// </summary>
        public (Vector3 t_Position, Vector3 t_Rotation) GetPlayerStartPreset()
        {
            var currentSceneControlPreset = SceneControllerManager.GetInstance.CurrentSceneControlPreset;
            var (valid, playerControlPreset) = currentSceneControlPreset.GetPlayerControl();
            if (valid)
            {
                return (playerControlPreset.StartPosition, playerControlPreset.StartRotation);
            }
            else
            {
                var variable = GetSceneVariableData();
                return (variable.PlayerStartPosition, variable.PlayerStartRotation);
            }
        }
        
        /// <summary>
        /// 현재 선택된 씬 변위 테이블 레코드가 멀티플레이를 지원하는지 검증하는 플래그
        /// </summary>
        public bool IsCurrentVariableSupportMultiPlay()
        {
            return GetSceneVariableData(_CurrentSceneVariableListIndex).SceneVariableFlagMask.HasAnyFlagExceptNone(SceneDataTool.SceneVariablePropertyType.SupportMultiPlay);
        }
        
        /// <summary>
        /// 현재 선택된 씬 변위 테이블 레코드가 던전인지 검증하는 플래그
        /// </summary>
        public bool IsCurrentVariableDungeon()
        {
            return GetSceneVariableData(_CurrentSceneVariableListIndex).SceneVariableFlagMask.HasAnyFlagExceptNone(SceneDataTool.SceneVariablePropertyType.Dungeon);
        }

        #endregion
    }
}