using System.Collections.Generic;

namespace BlackAm
{
    public interface IKeyInputTableBridge : ITableBase
    {
    }

    public interface IKeyInputTableRecordBridge : ITableBaseRecord
    {
        int KEY { get; }
        Dictionary<ControllerTool.CommandType, DialogueGameManager.InputAction> InputAction { get; }
    }

    public class DialogueKeyInputDataRoot : DialogueModuleDataRootBase<DialogueKeyInputDataRoot, DialogueGameManager.KeyInputType, IKeyInputTableBridge, IKeyInputTableRecordBridge>
    {
        #region <Methods>

        protected override DialogueModuleDataTool.DialogueModuleType GetDialogueModuleType()
        {
            return DialogueModuleDataTool.DialogueModuleType.KeyInput;
        }

        public override (DialogueGameManager.KeyInputType, IDialogue) SpawnModule(DialogueGameManager p_MasterNode, int p_TableIndex)
        {
            var (valid, labelType) = GetLabelType(p_TableIndex);
            if (valid)
            {
                var record = this[p_TableIndex];
                switch (labelType)
                {
                    case DialogueGameManager.KeyInputType.Title:
                        return (labelType, new KeyInputMain().OnInitializeKeyInput(labelType, p_MasterNode, record));
                    case DialogueGameManager.KeyInputType.Story:
                        return (labelType, new KeyInputStory().OnInitializeKeyInput(labelType, p_MasterNode, record));
                    case DialogueGameManager.KeyInputType.Gallery:
                        return (labelType, new KeyInputGallery().OnInitializeKeyInput(labelType, p_MasterNode, record));
                }
            }

            return default;
        }

        #endregion
    }
}