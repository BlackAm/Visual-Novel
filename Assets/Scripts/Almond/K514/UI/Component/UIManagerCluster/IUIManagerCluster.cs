#if !SERVER_DRIVE
using System.Collections.Generic;

namespace k514
{
    /// <summary>
    /// UIManagerBase 구현체들을 내부에 리스트로 가지면서, 리스트 내부에 이벤트를 전파하도록 하는 기능을 제공하는 인터페이스
    /// </summary>
    public interface IUIManagerCluster
    {
        List<UIManagerBase> SlaveNodes { get; }
        void OnUpdateUI(float p_DeltaTime);
        void OnEnableUI();
        void OnDisableUI();
        void Set_UI_Hide(bool p_HideFlag);
        void OnSceneTransition();
    }

    /// <summary>
    /// IUIManagerCluster 기본 구현체
    /// </summary>
    public abstract class UIManagerClusterBase : UIManagerBase, IUIManagerCluster
    {
        #region <Fields>

        public List<UIManagerBase> SlaveNodes { get; private set; }

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            SlaveNodes = new List<UIManagerBase>();
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
            for (int i = SlaveNodes.Count - 1; i > -1; i--)
            {
                SlaveNodes[i].OnUpdateUI(p_DeltaTime);
            }
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            ReleaseSlaveNode();
        }
        
        public override void OnEnableUI()
        {
            base.OnEnableUI();
            
            foreach (var slaveNode in SlaveNodes)
            {
                slaveNode.OnEnableUI();
            }
        }

        public override void OnDisableUI()
        {
            base.OnDisableUI();
            
            foreach (var slaveNode in SlaveNodes)
            {
                slaveNode.OnDisableUI();
            }
        }

        #endregion

        #region <Fields>

        public void Set_UI_Hide(bool p_HideFlag, bool p_SpreadToSubFlag)
        {
            if (p_SpreadToSubFlag)
            {
                Set_UI_Hide(p_HideFlag);
            }
            else
            {
                base.Set_UI_Hide(p_HideFlag);
            }
        }

        public override void Set_UI_Hide(bool p_HideFlag)
        {
            base.Set_UI_Hide(p_HideFlag);

            foreach (var slaveNode in SlaveNodes)
            {
                slaveNode.Set_UI_Hide(p_HideFlag);
            }
        }

        public override void OnSceneTransition()
        {
            base.OnSceneTransition();
            foreach (var slaveNode in SlaveNodes)
            {
                slaveNode.OnSceneTransition();
            }
        }

        public void AddSlaveNode(UIManagerBase p_UIManager)
        {
            p_UIManager.SetMasterCluster(this);
        }

        public void ReleaseSlaveNode()
        {
            for (int i = SlaveNodes.Count - 1; i > -1; i--)
            {
                var targetSlaveNode = SlaveNodes[i];
                targetSlaveNode.RetrieveObject();
            }
        }

        #endregion
    }
}
#endif