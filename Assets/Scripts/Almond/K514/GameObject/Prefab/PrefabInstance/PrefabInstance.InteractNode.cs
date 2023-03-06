using UnityEngine;

namespace k514
{
    public partial class PrefabInstance
    {
        #region <Fields>

        /// <summary>
        /// 종속 유닛
        /// </summary>
        public MasterNode MasterNode { get; private set; }

        /// <summary>
        /// 대상 유닛
        /// </summary>
        public FocusNode FocusNode { get; private set; }

        #endregion

        #region <Callbacks>

        private void OnAwakeNode()
        {
            MasterNode = new MasterNode(this, OnMasterNodeChanged);
            FocusNode = new FocusNode(this, OnFocusingNodeChanged);
        }
        
        private void OnRetrieveNode()
        {
            MasterNode.ResetNode();
            FocusNode.ResetNode();
        }
        
        protected virtual void OnMasterNodeChanged(Unit p_Prev, Unit p_Current)
        {
        }

        protected virtual void OnFocusingNodeChanged(Unit p_Prev, Unit p_Current)
        {
        }
        
        #endregion
    }
}