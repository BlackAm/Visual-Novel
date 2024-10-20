using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 하나의 씬 내부에서 설정값이 변해야 하는 경우
    /// 그러한 변위 값들을 지정하고 있는 테이블
    /// </summary>
    public class SceneVariableData : GameTable<SceneVariableData, int, SceneVariableData.TableRecord>
    {
        #region <Callbacks>

        #endregion
        
        public class TableRecord : GameTableRecordBase
        {
            /// <summary>
            /// StageDescription
            /// </summary>
            public string StageDescription { get; private set; }
            
            /// <summary>
            /// [레이어 타입, 원거리 컬링 거리] 컬렉션
            /// </summary>
            public Dictionary<GameManager.GameLayerType, float> FarCullingDistances { get; private set; }

            /// <summary>
            /// 해당 씬 속성 플래그 마스크
            /// </summary>
            public SceneDataTool.SceneVariablePropertyType SceneVariableFlagMask { private set; get; }
            
            /// <summary>
            /// 플레이어 진입 위치
            /// </summary>
            public Vector3 PlayerStartPosition { private set; get; }
            
            /// <summary>
            /// 플레이어 초기 회전
            /// </summary>
            public Vector3 PlayerStartRotation { private set; get; }
            
            /* 카메라 관련 */
            /// <summary>
            /// 카메라 초기 수직 회전
            /// </summary>
            public float CameraRootWrapperTiltDegree { private set; get; }
            
            /// <summary>
            /// 카메라 초기 수평 회전
            /// </summary>
            public float CameraRootWrapperSightDegree { private set; get; }
                        
            /// <summary>
            /// 카메라 추적 오프셋
            /// </summary>
            public Vector3 TraceOffset { private set; get; }
            
            /// <summary>
            /// 카메라 초기 거리
            /// </summary>
            public float CameraDistance { private set; get; }
            
#if APPLY_PPS
            /// <summary>
            /// 해당 씬에 적용할 포스트 프로세스 볼륨 프리팹 이름 그룹
            /// </summary>
            public List<PostProcessObjectPreset> PostProcessorPrefabList { private set; get; }
#endif
            /// <summary>
            /// 해당 씬 이름
            /// </summary>
            public int SceneName { private set; get; }

            /// <summary>
            /// 백그라운드 사운드
            /// </summary>
            public int BackGroundSound { private set; get; }

            /// <summary>
            /// 서브 백그라운드 사운드
            /// </summary>
            public int SubBackGroundSound { private set; get; }

            /// <summary>
            /// 해당 씬에 추가할 SceneEnvironment 타입 인덱스
            /// </summary>
            public int SceneEnvironmentTypeIndex { private set; get; }
            
            public List<int> SceneKeyInputIndex { private set; get; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneVariable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
