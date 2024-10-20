namespace BlackAm
{
    public interface IKeyInput : IDialogue
    {
        DialogueGameManager.KeyInputType _KeyInputType { get; }
        IKeyInputTableRecordBridge _KeyInputRecord { get; }
        IKeyInput OnInitializeKeyInput(DialogueGameManager.KeyInputType p_KeyInputType, DialogueGameManager p_MasterNode, IKeyInputTableRecordBridge p_KeyInputPreset);
        (bool, DialogueGameManager.InputAction) GetCommandType(ControllerTool.CommandType p_CommandType);
        void ActionKeyInput(ControllerTool.CommandType p_CommandType);
    }
}