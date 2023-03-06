using System;
using UnityEngine;

namespace k514
{
    public class GameManager : Singleton<GameManager>
    {
        #region <Consts>

        /// <summary>
        /// UI 레이어 마스크
        /// </summary>
        public readonly static int UI_LayerMask = (int) GameLayerMaskType.UI;
                
        /// <summary>
        /// 지형 및 유닛 레이어 마스크
        /// </summary>
        public readonly static int Camera_Layer = (int) GameLayerType.Camera;
        
        /// <summary>
        /// 지형 및 유닛 레이어 마스크
        /// </summary>
        public readonly static int Camera_LayerMask = (int) GameLayerMaskType.Camera;

        /// <summary>
        /// 유닛 레이어
        /// </summary>
        public readonly static int Unit_LayerA = (int) GameLayerType.UnitA;
        public readonly static int Unit_LayerB = (int) GameLayerType.UnitB;
        public readonly static int Unit_LayerC = (int) GameLayerType.UnitC;
        public readonly static int Unit_LayerD = (int) GameLayerType.UnitD;
        public readonly static int Unit_LayerE = (int) GameLayerType.UnitE;
        public readonly static int Unit_LayerF = (int) GameLayerType.UnitF;
        public readonly static int Unit_LayerG = (int) GameLayerType.UnitG;
        public readonly static int Unit_LayerH = (int) GameLayerType.UnitH;
        
        /// <summary>
        /// 유닛 레이어 마스크
        /// </summary>
        public readonly static int Unit_LayerMaskA = (int) GameLayerMaskType.UnitA;
        public readonly static int Unit_LayerMaskB = (int) GameLayerMaskType.UnitB;
        public readonly static int Unit_LayerMaskC = (int) GameLayerMaskType.UnitC;
        public readonly static int Unit_LayerMaskD = (int) GameLayerMaskType.UnitD;
        public readonly static int Unit_LayerMaskE = (int) GameLayerMaskType.UnitE;
        public readonly static int Unit_LayerMaskF = (int) GameLayerMaskType.UnitF;
        public readonly static int Unit_LayerMaskG = (int) GameLayerMaskType.UnitG;
        public readonly static int Unit_LayerMaskH = (int) GameLayerMaskType.UnitH;
        public readonly static int UnitSet_LayerMask = (int) GameLayerMaskType.UnitLayerSet;
        public readonly static int UnitSet_ExceptCorpse_LayerMask = (int) GameLayerMaskType.UnitLayerSet_Except_Corpse;
        
        /// <summary>
        /// 지형 레이어
        /// </summary>
        public readonly static int Terrain_Layer = (int) GameLayerType.Terrain;
        
        /// <summary>
        /// 지형 레이어 마스크
        /// </summary>
        public readonly static int Terrain_LayerMask = (int) GameLayerMaskType.Terrain;
                
        /// <summary>
        /// 장해물 레이어
        /// </summary>
        public readonly static int Obstacle_Layer = (int) GameLayerType.Obstacle;
        
        /// <summary>
        /// 장해물 레이어 마스크
        /// </summary>
        public readonly static int Obstacle_LayerMask = (int) GameLayerMaskType.Obstacle;
        
        /// <summary>
        /// 지형 및 유닛 레이어 마스크
        /// </summary>
        public readonly static int Terrain_Unit_LayerMask = Terrain_LayerMask | UnitSet_LayerMask;
                     
        /// <summary>
        /// 지형 및 유닛 레이어 마스크
        /// </summary>
        public readonly static int Terrain_UnitEC_LayerMask = Terrain_LayerMask | UnitSet_ExceptCorpse_LayerMask;
        
        /// <summary>
        /// 장해물 및 지형 레이어 마스크
        /// </summary>
        public readonly static int Obstacle_Terrain_LayerMask = Obstacle_LayerMask | Terrain_LayerMask;
        
        /// <summary>
        /// 장해물, 지형 및 유닛 레이어 마스크
        /// </summary>
        public readonly static int Obstacle_Terrain_Unit_LayerMask = Obstacle_LayerMask | Terrain_LayerMask | UnitSet_LayerMask;
                
        /// <summary>
        /// 장해물, 지형 및 유닛 레이어 마스크
        /// </summary>
        public readonly static int Obstacle_Terrain_UnitEC_LayerMask = Obstacle_LayerMask | Terrain_LayerMask | UnitSet_ExceptCorpse_LayerMask;
        
        #endregion

        #region <Fields>

        /// <summary>
        /// 씬 설정이 변경된 경우, 해당 이벤트를 수신받는 오브젝트
        /// </summary>
        public SceneVariableChangeEventReceiver SceneVariableChangeEventReceiver { get; private set; }

        #endregion
        
        #region <Enums>
        
        public enum GameTagType
        {
            Player, Enemy, Neutral, Terrain, Obstacle
        }

        public enum GameLayerType
        {
            /* Built-In */
            Default = 0,
            
            TransparentFX = 1,
            IgnoreRaycast = 2,
            
            Water = 4,
            UI = 5,
            
            /* User */
            /// <summary>
            /// 절대 좌표계에서 랜더링되는 UI 레이어
            /// </summary>
            WorldUI = 8, 
            
            /// <summary>
            /// 지형 레이어
            /// </summary>
            Terrain = 9, 
            
            /// <summary>
            /// 카메라에 랜더링되는 장해물 레이어
            /// </summary>
            Obstacle = 10,
            
            /// <summary>
            /// 카메라에 랜더링되지 않는 장해물 레이어
            /// 맵의 가장자리 등
            /// </summary>
            Block = 11, 
            
            /// <summary>
            /// PPS 가중치 영역
            /// </summary>
            PostProcessVolume = 12, 
            
            /// <summary>
            /// 카메라 레이어
            /// </summary>
            Camera = 13,
            
            /// <summary>
            /// 카메라가 컬링하는 레이어
            /// </summary>
            CameraIgnore = 14,
            
            /* Unit */
            /// <summary>
            /// 유닛 기준 레이어
            /// </summary>
            UnitA = 15,
            
            /// <summary>
            /// 서버노드 유닛
            /// </summary>
            UnitB = 16,
            
            /// <summary>
            /// 시체 유닛
            /// </summary>
            UnitC = 17,
            
            /// <summary>
            /// 플레이어와 적대 유닛
            /// </summary>
            UnitD = 18,
            
            /// <summary>
            /// 플레이어와 동맹 유닛
            /// </summary>
            UnitE = 19,
            
            /// <summary>
            /// 플레이어와 중립 유닛
            /// </summary>
            UnitF = 20,
                        
            /// <summary>
            /// 플레이어 유닛
            /// </summary>
            UnitG = 21,
                                    
            /// <summary>
            /// 분류 없음
            /// </summary>
            UnitH = 22,
            
            /// <summary>
            /// VFX
            /// </summary>
            Vfx = 23,
        }
        
        [Flags]
        public enum GameLayerMaskType
        {
            /* Reserved */
            Everything = -1,
            Nothing = 0,
            
            /* Built-In */
            Default = 1 << GameLayerType.Default,
            TransparentFX = 1 << GameLayerType.TransparentFX,
            IgnoreRaycast = 1 << GameLayerType.IgnoreRaycast,
            Water = 1 << GameLayerType.Water,
            UI = 1 << GameLayerType.UI,
            
            /* User */
            WorldUI = 1 << GameLayerType.WorldUI,
            Terrain = 1 << GameLayerType.Terrain,
            Obstacle = 1 << GameLayerType.Obstacle,
            Block = 1 << GameLayerType.Block,
            PostProcessVolume = 1 << GameLayerType.PostProcessVolume,
            Camera = 1 << GameLayerType.Camera,
            CameraIgnore = 1 << GameLayerType.CameraIgnore,
            Vfx = 1 << GameLayerType.Vfx,
            
            /* Unit */
            UnitA = 1 << GameLayerType.UnitA,
            UnitB = 1 << GameLayerType.UnitB,
            UnitC = 1 << GameLayerType.UnitC,
            UnitD = 1 << GameLayerType.UnitD,
            UnitE = 1 << GameLayerType.UnitE,
            UnitF = 1 << GameLayerType.UnitF,
            UnitG = 1 << GameLayerType.UnitG,
            UnitH = 1 << GameLayerType.UnitH,
            
            /* LayerMask */
            UnitLayerSet = UnitA | UnitB | UnitC | UnitD | UnitE | UnitF | UnitG | UnitH,
            UnitLayerSet_Except_Corpse = UnitLayerSet & ~UnitC,
            TerrainObstacleSet = Terrain | Obstacle,
        }

        public enum GameSortingLayerType
        {
            DepthLower, DepthNormal, DepthUpper
        }
        
        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            // 이벤트 수신 오브젝트 초기화
            SceneVariableChangeEventReceiver =
                SceneEnvironmentManager.GetInstance
                    .GetEventReceiver<SceneVariableChangeEventReceiver>(
                        SceneEnvironmentManager.SceneVariableEventType.OnSceneVariableChanged, OnMapVariableChanged);
        }


        public override void OnInitiate()
        {
        }
        
        private void OnMapVariableChanged(SceneEnvironmentManager.SceneVariableEventType p_EventType, SceneVariableData.TableRecord p_Record)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[GameManager / OnMapVariableChanged] {p_EventType}");
#endif
        }
        
        #endregion
    }
}