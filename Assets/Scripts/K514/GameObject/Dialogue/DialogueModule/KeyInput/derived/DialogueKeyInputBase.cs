using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class DialogueKeyInputBase : DialogueModuleBase, IKeyInput
    {
        ~DialogueKeyInputBase()
        {
            Dispose();
        }

        #region <Fields>

        public DialogueGameManager.KeyInputType _KeyInputType { get; protected set; }
        
        public IKeyInputTableRecordBridge _KeyInputRecord { get; private set; }

        private Dictionary<DialogueGameManager.InputAction, Func<bool>> _ActionEventHandlerCollection;
        
        protected ControllerEventReceiver _ControllerEventReceiver;
        
        private Dictionary<ControllerTool.CommandType, DialogueGameManager.InputAction> _InputCommandMappedActionPresetTable;
        
        private List<ControllerTool.CommandType> _CommandList;
        
        private List<ControllerTool.CommandType> _AvailableCommandList;

        #endregion

        public override void OnUpdate(float p_DeltaTime)
        {
        }

        protected override void OnModuleNotify()
        {
            _ControllerEventReceiver.SetReceiverBlock(false);
        }

        protected override void OnModuleSleep()
        {
            _ControllerEventReceiver.SetReceiverBlock(true);
        }

        protected override void DisposeUnManaged()
        {
        }

        public void OnPropertyModified(ControllerTool.InputEventType p_Type, ControllerTool.ControlEventPreset p_Preset)
        {
            OnInputActionTriggered(p_Preset);
        }
        
        public void OnInputActionTriggered(ControllerTool.ControlEventPreset p_Preset)
        {
            var (validCommand, ActionType) = GetCommandType(p_Preset.CommandType);
            if (validCommand && p_Preset.ControllerInputStateType == ControllerTool.ControllerInputStateType.ReleaseKey)
            {
                if (!SaveLoad.Instance.IsActive)
                {
                    OnInputActionTriggered(ActionType);
                }
            }
        }
        
        public void OnInputActionTriggered(DialogueGameManager.InputAction p_InputAction)
        {
            _ActionEventHandlerCollection[p_InputAction].Invoke();
        }

        public IKeyInput OnInitializeKeyInput(DialogueGameManager.KeyInputType p_KeyInputType, DialogueGameManager p_MasterNode,
            IKeyInputTableRecordBridge p_KeyInputPreset)
        {
            DialogueModuleType = DialogueModuleDataTool.DialogueModuleType.KeyInput;
            _KeyInputType = p_KeyInputType;
            _MasterNode = p_MasterNode;
            _KeyInputRecord = p_KeyInputPreset;

            _ActionEventHandlerCollection = new Dictionary<DialogueGameManager.InputAction, Func<bool>>();
            _ActionEventHandlerCollection.Add(DialogueGameManager.InputAction.UpdateDialogue, OnHandleDialogueUpdate);
            _ActionEventHandlerCollection.Add(DialogueGameManager.InputAction.SkipDialogue, OnHandleSkipDialogue);
            _ActionEventHandlerCollection.Add(DialogueGameManager.InputAction.ShowHistory, OnHandleShowHistory);
            _ActionEventHandlerCollection.Add(DialogueGameManager.InputAction.HideUI, OnHandleHideUI);

            _InputCommandMappedActionPresetTable = new Dictionary<ControllerTool.CommandType, DialogueGameManager.InputAction>();
            _CommandList = new List<ControllerTool.CommandType>();
            _AvailableCommandList = new List<ControllerTool.CommandType>();
            
            _ControllerEventReceiver = ControllerTool.GetInstanceUnSafe
                .GetControllerEventSender(ControllerTool.InputEventType.DialogueControl)
                .GetEventReceiver<ControllerEventReceiver>(ControllerTool.InputEventType.DialogueControl, OnPropertyModified);
            
            BindAction(_KeyInputRecord);

            return this;
        }

        public (bool, DialogueGameManager.InputAction) GetCommandType(ControllerTool.CommandType p_CommandType)
        {
            return _InputCommandMappedActionPresetTable.TryGetValue(p_CommandType, out var o_InputAction) ? (true, o_InputAction) : (false, DialogueGameManager.InputAction.None);
        }

        public void ActionKeyInput(ControllerTool.CommandType p_CommandType)
        {
            var (validCommand, ActionType) = GetCommandType(p_CommandType);
            if (validCommand)
            {
                OnInputActionTriggered(ActionType);
            }
        }
    }

}
