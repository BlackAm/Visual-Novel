#if !SERVER_DRIVE
using System;
using UnityEngine;
using System.Collections.Generic;
using Almond.RPC;

namespace BlackAm
{
    public class MainGameUI : UIManagerClusterBase
    {
        #region mapRatio

        public float infoX = 3.9f, infoY = 3.9f;
        public float normalX = 510 * -1f, normalY = 510 * -1f;

        //맵 마커 위치 배율은 SetActiveMapSize에서 변경
        public float _MiniMapSize;
        public float _WorldMapSize;

        #endregion
        
        
        private static MainGameUI _instance;
        public static MainGameUI Instance
        {
            get => _instance;
        }

        public MainUI mainUI;
        public FunctionUI functionUI;
        public PopUpUIManager popUpUI;
        // public TopMenu topMenu;
        public UIEffect _UIEffect;
        
        private Dictionary<string, Sprite[]> uiSpritePool = new Dictionary<string, Sprite[]>();

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            _instance = _instance ? _instance : this;

            mainUI = _Transform.Find("MainUI").gameObject.AddComponent<MainUI>();
            mainUI.CheckAwake();
            
            SlaveNodes.Add(mainUI);
            
            functionUI = _Transform.Find("FunctionUI").gameObject.AddComponent<FunctionUI>();
            popUpUI = _Transform.Find("PopUpObjects").gameObject.AddComponent<PopUpUIManager>();
            // topMenu = _Transform.Find("TopMenu").gameObject.AddComponent<TopMenu>();
            _UIEffect = _Transform.Find("UIEffect").gameObject.AddComponent<UIEffect>();

            Initialize();
        }
        
        public void Initialize()
        {
            functionUI.Initialize();
            popUpUI.Init();
            //infoUI.Initialize();
            // topMenu.Initialize();
            _UIEffect.Initialize();
            
            /// 테스트 디버그 패널.
            //debugPanel = _Transform.Find("DebugPanel").gameObject.AddComponent<TestLog>();
            //debugPanel._initType = TestLog.InitType.Main;
            //debugPanel.Initialize();
        }

        public override void OnSceneTerminated()
        {
            _UIEffect.OnSceneTerminated();
        }
        
        public Sprite GetUISpritePool(string spriteName)
        {
            string[] path = spriteName.Split('/');

            Sprite[] sprites = null;
            Sprite sprite = null;
            if (uiSpritePool.ContainsKey(path[0]))
            {
                sprites = uiSpritePool[path[0]];
                if (sprites.Length == 1)
                {
                    return sprites[0];
                }
                else
                {
                    foreach (var data in sprites)
                    {
                        if (data.name == path[1])
                        {
                            return data;
                        }
                    }
                }
            }
            
            if (path.Length == 1)
            {
                sprites = new Sprite[1];
                sprites[0] = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.Free_Condition, spriteName).Item2;
                sprite = sprites[0];
            }
            else
            {
                sprites = LoadAssetManager.GetInstanceUnSafe.LoadMultipleAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.Free_Condition, $"{path[0]}.png").Item2;
                foreach (var data in sprites)
                {
                    if (data.name == path[1])
                    {
                        sprite = data;
                        break;
                    }
                }
            }
            if (!uiSpritePool.ContainsKey(path[0]))
            {
                uiSpritePool.Add(path[0], sprites);
            }
            return sprite;
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
            base.OnUpdateUI(p_DeltaTime);
            
            if (!ReferenceEquals(null, functionUI))
                functionUI.OnUpdateUI(p_DeltaTime);
        }

        ///카메라 모드로 바뀌면서 UI 생성 크기 및 위치 옮김 버그 문제로 인하여 메소드 임시 추가
        ///크기 및 Z위치 보정
        public static void FixCameraModeUIObject(Transform p_UIObject, float p_ScaleX = 1, float p_ScaleY = 1){
            p_UIObject.transform.localPosition = new Vector3(p_UIObject.transform.localPosition.x, p_UIObject.transform.localPosition.y,0);
            p_UIObject.transform.localScale = new Vector3(p_ScaleX, p_ScaleY, 1);
        }
    }
}
#endif