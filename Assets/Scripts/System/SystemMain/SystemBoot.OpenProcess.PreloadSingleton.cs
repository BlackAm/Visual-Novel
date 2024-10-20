using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 초기화하고자 하는 싱글톤 타입을 정적 배열로 미리 선언해두고 배열단위로 초기화를 수행하는 부분클래스
    ///
    /// 지정받은 타입으로부터 싱글톤을 초기화하려면 GetInstance계열의 정적멤버를 호출해야하는데, 현재 리플렉션으로 Type타입에서 클래스의 정적멤버에
    /// 접근하는 방식으로 처리중.
    ///
    /// 해당 방식은 느리고 힙할당도 상당하기 때문에 다른 방식으로 대체할 필요가 있다.
    /// 예를 들어, 그냥 메서드 안에서 타입을 통하는게 아니라 싱글톤에 직접 접근해버린다던가.
    /// </summary>
    public partial class SystemBoot
    {
        #region <Consts>

        private static readonly Type[] __SystemInitializeBasisTableGroup =
        {
            typeof(SystemLanguage),
        };
        
        private static readonly Type[] SystemInitializeBasisTableGroup =
#if UNITY_EDITOR
            typeof(EditorModeOnlyGameData<,,>)
                .GetSubClassTypeSet()
                .Concat
                (
                    __SystemInitializeBasisTableGroup
                )
                .ToArray();
#else
            __SystemInitializeBasisTableGroup;
#endif
        
        private static readonly Type[] PreLoadSystemTableGroup = 
        {
            typeof(SystemConfigData), typeof(ResourceListData), typeof(SystemValue), typeof(SystemLanguage),
        };

        private static readonly Type[] PreLoadSingletonGroup0 =
        {
            typeof(SceneChangeEventSender), typeof(GameManager), typeof(SteamManager), typeof(SaveLoadManager),
            typeof(LoadAssetManager), typeof(SceneControllerManager), 
            typeof(ControllerKeyMapData), typeof(CommandFunctionMapData), typeof(InputEventDeviceMap), typeof(KeyCodeCommandMapData),
            typeof(ControllerTool), typeof(DialogueKeyInputDataRoot), typeof(DialoguePlayModeDataRoot), typeof(DialoguePlayModePresetData),

        };

        private static readonly Type[] PreLoadSingletonGroup1 =
        {
            typeof(SceneEnvironmentManager), typeof(DialogueGameManager),
            typeof(PrefabPoolingManager), typeof(EventTimerCoroutineManager),
            typeof(GameEventTimerHandlerManager),
        };

        private static readonly Type[] PreLoadSingletonGroup2 =
        {
            typeof(ObjectDeployLoader), typeof(SelectDialoguePoolingManager), typeof(DialogueHistoryPoolingManager),
            typeof(CharacterImagePoolingManager),
            typeof(PrefabPoolingTool), typeof(LanguageManager), typeof(GalleryManager),
            typeof(DialogueSceneInfoData), typeof(DialogueSceneExtraInfo),
        };

        private static readonly Type[] PreLoadSingletonGroup3 =
        {
#if SERVER_DRIVE
#else        
            typeof(DialoguePresetData), typeof(DialogueEventPresetData),
            typeof(CameraManager), typeof(SfxSpawnManager), typeof(AudioManager), typeof(BGMManager),
            typeof(UIDataRoot),
#endif
        };

        private static readonly Type[] PreLoadSingletonGroup4 =
        {
            typeof(PrefabModelDataRoot),
#if SERVER_DRIVE
#else       
            typeof(DefaultUIManagerSet), typeof(SpriteSheetManager),
            typeof(TouchEventManager), typeof(DirectionalLightController),
#endif
        };

        private static readonly Type[] PreLoadSingletonGroup5 =
        {
#if SERVER_DRIVE
#else
            typeof(SpriteSheetManager),
            typeof(TouchEventManager), typeof(DirectionalLightController),
#endif
        };

        private static readonly Type[] PreLoadGameTableGroup0 =
        {
            typeof(SceneEnvironmentTypeMap),
        };
        
        private static readonly Type[] PreLoadGameTableGroup1 =
        {
            typeof(PrefabModelData_Vfx),
        };

        private static readonly Type[] PreLoadGameTableGroup2 =
        {
            typeof(CharacterImagePresetData), typeof(EventCGPresetData),
            typeof(DialogueBGMPresetData), typeof(DialogueSEPresetData), typeof(SelectDialoguePresetData),
            typeof(DialogueBackGroundImagePresetData), typeof(SelectDialogueInfoData), typeof(DialogueFadePresetData),
        };
        
        private static readonly Type[] PreLoadGameTableGroup3 =
        {
            typeof(SelectDialogueConditionPresetData), typeof(SelectDialogueConditionLikingPresetData),
            
            typeof(ShowSelectDialoguePresetData),
            typeof(ChangeEventCGPresetData), typeof(ChangeBackGroundImagePresetData), typeof(ResizeBackGroundImagePresetData), 
            typeof(MoveBackGroundImageLerpPresetData),
            typeof(ChangeCharacterImagePresetData), typeof(ResizeCharacterImagePresetData), typeof(RemoveCharacterImagePresetData),
            
            typeof(DialogueFadeInPresetData), typeof(DialogueFadeOutPresetData),
        };
                
        private static readonly Type[] PreLoadGameTableGroup4 =
        {
            typeof(CharacterImageGallery), typeof(EventCGGallery), typeof(SceneGallery),
            
            typeof(ObjectDeployPresetMapData),
            typeof(ObjectVectorMapData),
        };

        private static readonly Type[] PreLoadGameTableGroup5 =
        {
            typeof(SceneDataRoot), typeof(SceneEntryData), typeof(SceneSettingData), typeof(SceneVariableData),
#if SERVER_DRIVE
#else
            typeof(CameraConfigureData), typeof(CameraCommandTableData), 
#endif
        };

        private static readonly Type[] MainGameSceneBaseTableGroup =
            typeof(PrefabSpawnDataBase<,>)
                .GetSubClassTypeSet()
                .Concat
                (
                    typeof(BaseResourceNameTable<,,,>)
                        .GetSubClassTypeSet()
                )
                .Concat
                (
                    new[]
                    {
                        typeof(PrefabExtraDataRoot),
                        typeof(AnimationControllerData),
                        typeof(ObjectDeployDataRoot),
#if !SERVER_DRIVE
                        typeof(ImageNameTableData),
                        typeof(UIParticleEffectData),
#endif
                    }
                )
                .ToArray();

        #endregion

        #region <Methods>

        /// <summary>
        /// 타입 배열 중, 테이블 타입 싱글톤만 초기화 시키는 메서드
        /// </summary>
        private static async UniTask PreloadTableAsync(Type[] p_TypeSet)
        {
            var targetSingletonGroup = p_TypeSet;
            foreach (var preLoadTableType in targetSingletonGroup)
            {
                switch (preLoadTableType)
                {
                    case var type when SingletonTool.IsTableData(type) || SingletonTool.IsTableDataRoot(type):
                        await SingletonTool.CreateAsyncSingleton(type);
                        break;
                }
            }
        }

        /// <summary>
        /// 타입 배열 중, 컴포넌트/싱글톤/비동기싱글톤을 초기화 시키는 메서드
        /// </summary>
        private async UniTask PreloadSingletonAsync(Type[] p_TypeSet, bool NoLog = true)
        {
            // 싱글톤 중에서도, 시스템 부팅을 위한 싱글톤은 먼저 로드되어야 한다.
            var monoBehaviourType = typeof(MonoBehaviour);
            foreach (var preLoadSingletonType in p_TypeSet)
            {
                if(!NoLog)
                {
                    DisplayLog($"PreloadSingletonAsync. Type({preLoadSingletonType}) LoadStart");
                }
                switch (preLoadSingletonType)
                {
                    // 싱글톤
                    case var type when SingletonTool.IsSingleton(type):
                        SingletonTool.CreateSingleton(type);
                        break;
                    // 유니티 싱글톤
                    case var type when SingletonTool.IsUnityComponentSingleton(type):
                        gameObject.AddComponent(type);
                        break;
                    // 비동기 싱글톤
                    case var type when SingletonTool.IsAsyncSingleton(type):
                        await SingletonTool.CreateAsyncSingleton(type);
                        break;
                    // 유니티 비동기 싱글톤
                    case var type when SingletonTool.IsUnityComponentAsyncSingleton(type):
                        gameObject.AddComponent(type);
                        // 비동기 로드 완료를 기다려준다.
                        await SingletonTool.CreateAsyncSingleton(type);
                        break;
                    // 그 외의 컴포넌트
                    case var type when type.IsSubclassOf(monoBehaviourType):
                        gameObject.AddComponent(type);
                        break;
                }
                if (!NoLog)
                {
                    DisplayLog($"PreloadSingletonAsync. Type({preLoadSingletonType}) End");
                }
            }
        }

        #endregion

        #region <Method/TypeSetInvoke>

        static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        public static void DisplayLog(string str)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
                Debug.LogWarning($"{str} : Time({sw.ElapsedMilliseconds}).mesc");
                sw.Reset();
            }
            
            sw.Start();
        }

        public static async UniTask LoadSystemInitializeBasisTable()
        {
#if !UNITY_EDITOR
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
#endif
            DisplayLog("LoadSystemInitializeBasisTable Load");
            await PreloadTableAsync(SystemInitializeBasisTableGroup);
            DisplayLog("LoadSystemInitializeBasisTable End");
        }

        public static async UniTask PreloadSystemTableAsync()
        {
            DisplayLog("PreloadSystemTableAsync Load");
            await PreloadTableAsync(PreLoadSystemTableGroup);
            DisplayLog("PreloadSystemTableAsync End");
        }

        public async UniTask PreloadGameSingletonAsync0()
        {
            DisplayLog("PreloadGameSingletonAsync0 Load");
            await PreloadSingletonAsync(PreLoadSingletonGroup0);
            DisplayLog("PreloadGameSingletonAsync0 End");
        }

        public async UniTask PreloadGameSingletonAsync1()
        {
            DisplayLog("PreloadGameSingletonAsync1 Load");
            await PreloadSingletonAsync(PreLoadSingletonGroup1);
            DisplayLog("PreloadGameSingletonAsync1 End");
        }

        public async UniTask PreloadGameSingletonAsync2()
        {
            DisplayLog("PreloadGameSingletonAsync2 Load");
            await PreloadSingletonAsync(PreLoadSingletonGroup2);
            DisplayLog("PreloadGameSingletonAsync2 End");
        }

        public async UniTask PreloadGameSingletonAsync3()
        {
            DisplayLog("PreloadGameSingletonAsync3 Load");
            await PreloadSingletonAsync(PreLoadSingletonGroup3, false);
            DisplayLog("PreloadGameSingletonAsync3 End");
        }

        public async UniTask PreloadGameSingletonAsync4()
        {
            DisplayLog("PreloadGameSingletonAsync4 Load");
            await PreloadSingletonAsync(PreLoadSingletonGroup4);
            DisplayLog("PreloadGameSingletonAsync4 End");
        }
        
        public async UniTask PreloadGameSingletonAsync5()
        {
            DisplayLog("PreloadGameSingletonAsync5 Load");
            await PreloadSingletonAsync(PreLoadSingletonGroup5);
            DisplayLog("PreloadGameSingletonAsync5 End");
        }

        public async UniTask PreloadGameTableAsync0()
        {
            DisplayLog("PreloadGameTableAsync0 Load");
            await PreloadTableAsync(PreLoadGameTableGroup0);
            DisplayLog("PreloadGameTableAsync0 End");
        }

        public async UniTask PreloadGameTableAsync1()
        {
            DisplayLog("PreloadGameTableAsync1 Load");
            await PreloadTableAsync(PreLoadGameTableGroup1);
            DisplayLog("PreloadGameTableAsync1 End");
        }

        public async UniTask PreloadGameTableAsync2()
        {
            DisplayLog("PreloadGameTableAsync2 Load");
            await PreloadTableAsync(PreLoadGameTableGroup2);
            DisplayLog("PreloadGameTableAsync2 End");
        }

        public async UniTask PreloadGameTableAsync3()
        {
            DisplayLog("PreloadGameTableAsync3 Load");
            await PreloadTableAsync(PreLoadGameTableGroup3);
            DisplayLog("PreloadGameTableAsync3 End");
        }

        public async UniTask PreloadGameTableAsync4()
        {
            DisplayLog("PreloadGameTableAsync4 Load");
            await PreloadTableAsync(PreLoadGameTableGroup4);
            DisplayLog("PreloadGameTableAsync4 End");
        }

        public async UniTask PreloadGameTableAsync5()
        {
            DisplayLog("PreloadGameTableAsync5 Load");
            await PreloadTableAsync(PreLoadGameTableGroup5);
            DisplayLog("PreloadGameTableAsync5 End");
        }

        public async UniTask LoadMainGameTableAsync()
        {
            DisplayLog("LoadMainGameTableAsync Load");
            await PreloadTableAsync(MainGameSceneBaseTableGroup);
            DisplayLog("LoadMainGameTableAsync End");
        }

#endregion
    }
}