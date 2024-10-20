namespace BlackAm
{
    public interface IPlayModeTableBridge : ITableBase
    {
    }

    public interface IPlayModeTableRecordBridge : ITableBaseRecord
    {
        int KEY { get; }
        
        uint PreDelay { get; }
    }

    public class DialoguePlayModeDataRoot : DialogueModuleDataRootBase<DialoguePlayModeDataRoot, DialogueGameManager.DialoguePlayMode, IPlayModeTableBridge, IPlayModeTableRecordBridge>
    {
        #region <Methods>

        protected override DialogueModuleDataTool.DialogueModuleType GetDialogueModuleType()
        {
            return DialogueModuleDataTool.DialogueModuleType.DialoguePlayMode;
        }

        public override (DialogueGameManager.DialoguePlayMode, IDialogue) SpawnModule(DialogueGameManager p_MasterNode, int p_TableIndex)
        {
            var (valid, labelType) = GetLabelType(p_TableIndex);
            if (valid)
            {
                var record = this[p_TableIndex];
                switch (labelType)
                {
                    case DialogueGameManager.DialoguePlayMode.BasicPlay:
                        return (labelType, new PlayModeBasic().OnInitializePlayMode(labelType, p_MasterNode, record));
                    case DialogueGameManager.DialoguePlayMode.AutoPlay:
                        return (labelType, new PlayModeAuto().OnInitializePlayMode(labelType, p_MasterNode, record));
                    case DialogueGameManager.DialoguePlayMode.SkipPlay:
                        return (labelType, new PlayModeSkip().OnInitializePlayMode(labelType, p_MasterNode, record));
                }
            }

            return default;
        }

        #endregion
    }
}