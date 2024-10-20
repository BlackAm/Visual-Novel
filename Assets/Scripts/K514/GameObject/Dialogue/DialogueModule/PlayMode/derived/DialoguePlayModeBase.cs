namespace BlackAm
{
    public partial class DialoguePlayModeBase : DialogueModuleBase, IPlayMode
    {
        ~DialoguePlayModeBase()
        {
            Dispose();
        }

        #region <Fields>

        public DialogueGameManager.DialoguePlayMode _PlayMode { get; protected set; }
        
        public IPlayModeTableRecordBridge PlayMode { get; private set; }

        #endregion

        #region <CallBacks>

        protected override void OnModuleNotify()
        {
        }

        protected override void OnModuleSleep()
        {
        }
        
        public override void OnUpdate(float p_DeltaTime)
        {
        }

        protected override void DisposeUnManaged()
        {
        }

        public IPlayMode OnInitializePlayMode(DialogueGameManager.DialoguePlayMode p_PlayMode, DialogueGameManager p_MasterNode,
            IPlayModeTableRecordBridge p_PlayModePreset)
        {
            DialogueModuleType = DialogueModuleDataTool.DialogueModuleType.DialoguePlayMode;
            _PlayMode = p_PlayMode;
            _MasterNode = p_MasterNode;
            PlayMode = p_PlayModePreset;

            return this;
        }

        #endregion

        #region <Methods>

        public virtual void UpdateDialogueState(DialogueGameManager.DialogueState p_DialogueState)
        {
            
        }

        #endregion

    }
}