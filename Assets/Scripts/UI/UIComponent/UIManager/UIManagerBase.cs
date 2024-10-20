#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public abstract class UIManagerBase : PrefabInstance, ISceneChange
    {
        #region <Fields>

        /// <summary>
        /// 해당 UI 매니저를 포함하는 캔버스 레퍼런스
        /// </summary>
        protected Canvas _WrapperCanvas;
      
        /// <summary>
        /// 해당 UI가 랜더링될 캔버스 타입
        /// </summary>
        protected RenderMode _RenderMode;
      
        /// <summary>
        /// 해당 UI 가시 플래그, 풀링오브젝트는 풀링될 때 해당 플래그와 별도로 가시성이 초기화 되는 것에 주의
        /// </summary>
        public bool _HideFlag { get; protected set; }

        /// <summary>
        /// RectTransform 캐시
        /// </summary>
        public RectTransform _RectTransform;

        /// <summary>
        /// RectTransform 트래커
        /// </summary>
        protected UITool.RectTransformPreset _RectTransformPreset;

        /// <summary>
        /// 해당 UIManager가 상위 UIManagetCluster로부터 관리를 받고 있는지 정보를 기술하는 프리셋
        /// </summary>
        private (bool t_IsSlaveNode, IUIManagerCluster t_MasterNode) _SlaveNodePreset;

        protected bool _OnSceneTransitionNoDisable;

        protected PrefabInstance _UIObject;

        #endregion

        #region <Callbacks>

        public override void InitializeAffine(float p_Scale)
        {
            _RectTransform = transform as RectTransform;
            
            base.InitializeAffine(p_Scale);
        }
      
        public override void OnSpawning()
        {
            base.OnSpawning();
          
            var extraData = _PrefabKey._PrefabExtraPreset.Canvas;

            if (extraData.Item1)
            {
                _WrapperCanvas = extraData.Item3;
                _RenderMode = extraData.Item2;
                _Transform.SetParent(_WrapperCanvas.transform, false);  
            }
          
            _TransformPreset = _Transform.GetTransformPreset();
            _RectTransformPreset = _RectTransform.GetTransformPreset();
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
          
            if (_SlaveNodePreset.t_IsSlaveNode)
            {
                var masterNode = _SlaveNodePreset.t_MasterNode;
                masterNode.SlaveNodes.Remove(this);
                _SlaveNodePreset = default;
            }
        }


        public virtual void OnEnableUI()
        {
        }

        public virtual void OnDisableUI()
        {
        }

        public abstract void OnUpdateUI(float p_DeltaTime);

        public virtual async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public virtual void OnSceneStarted()
        {
        }

        public virtual void OnSceneTerminated()
        {
        }

        public virtual void OnSceneTransition()
        {
            if(!_OnSceneTransitionNoDisable)
            {
                Set_UI_Hide(true);
            }
        }
      
        #endregion

        #region <Methods>

        public virtual void Set_UI_Hide(bool p_HideFlag)
        {
            if (_HideFlag != p_HideFlag)
            {
                gameObject.SetActiveSafe(_HideFlag);
                _HideFlag = p_HideFlag;
                if (_HideFlag)
                {
                    OnDisableUI();
                }
                else
                {
                    OnEnableUI();
                }
            }
        }

        public void Toggle_UI_Hide()
        {
            Set_UI_Hide(!_HideFlag);
        }

        public void SetMasterCluster(IUIManagerCluster p_MasterNode)
        {
            _SlaveNodePreset = (true, p_MasterNode);
            p_MasterNode.SlaveNodes.Add(this);
        }

        public override void SetScale(float p_ScaleRate)
        {
            ObjectScale = ObjectScale.ApplyScale(p_ScaleRate);
            _RectTransform.localScale = ObjectScale.CurrentValue * Vector3.one;
        }
        
        public PrefabInstance GetUIObject()
        {
            return _UIObject;
        }
        #endregion
    }
}
#endif