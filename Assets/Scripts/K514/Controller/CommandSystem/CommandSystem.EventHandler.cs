namespace BlackAm
{
    public partial class CommandSystem
    {
        #region <Consts>

        private const string VERTICAL = "Vertical";
        private const string HORIZON = "Horizontal";

        #endregion

        #region <Fields>

        private ControllerTool.OnCheckKeyState GetKeyUp;
        private ControllerTool.OnCheckKeyState GetKeyDown;
        private ControllerTool.OnMoveEventHandler OnMoveEventOccured;
        private ControllerTool.OnMoveEventHandler OnMoveEventTerminated;
        private ControllerTool.OnSendCommandInput OnSendHoldingCommandInput;
        private ControllerTool.OnSendCommandInput OnSendSequenceCommandInput;
        private ControllerTool.OnSendCommandInput OnSendMonoCommandInput;
        private ControllerTool.OnSendCommandInput OnSendReleaseCommandInput;
        
        #endregion
        
        #region <Callbacks>

        private void OnCreatePartialInput()
        {
            ControllerTool.GetInstanceUnSafe.GetCheckKeyHandler(InputControllerType, ref GetKeyUp, ref GetKeyDown);
            ControllerTool.GetInstanceUnSafe.GetMoveEventHandler(ControllerTool.ArrowEventHandlerType.XZMove, ref OnMoveEventOccured, ref OnMoveEventTerminated);
     
            OnSendHoldingCommandInput = ControllerTool.GetInstanceUnSafe.SendHoldingCommandInput;
            OnSendSequenceCommandInput = ControllerTool.GetInstanceUnSafe.SendSequenceCommandInput;
            OnSendMonoCommandInput = ControllerTool.GetInstanceUnSafe.SendMonoCommandInput;
            OnSendReleaseCommandInput = ControllerTool.GetInstanceUnSafe.SendReleaseCommandInput;
        }

        #endregion
        
        #region <Methods>

        public bool GetKey(int p_KeyCodeType)
        {
            var keyCode = KeyValueTable[p_KeyCodeType];
            return HasKeyBit(keyCode);
        }

        #endregion
    }
}