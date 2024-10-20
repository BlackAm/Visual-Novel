
namespace BlackAm
{
    public partial class DialogueKeyInputBase
    {
        public void BindAction(IKeyInputTableRecordBridge p_ActionSet)
        {
            var actionRecords = p_ActionSet.InputAction;
            if (!ReferenceEquals(null, actionRecords))
            {
                foreach (var actionRecord in actionRecords)
                {
                    var commandType = actionRecord.Key;
                    var inputAction = actionRecord.Value;
                    BindAction(commandType, inputAction);
                }
            }
        }

        public void BindAction(ControllerTool.CommandType p_CommandType, DialogueGameManager.InputAction p_InputAction)
        {
            _CommandList.Add(p_CommandType);
            if (_InputCommandMappedActionPresetTable.TryGetValue(p_CommandType, out var o_InputAction))
            {
                _InputCommandMappedActionPresetTable[p_CommandType] = p_InputAction;
            }
            else
            {
                _InputCommandMappedActionPresetTable.Add(p_CommandType, p_InputAction);
            }
        }
    }
}