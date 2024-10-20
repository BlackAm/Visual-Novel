using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        #region <Enum>
        
        public enum KeyInputType
        {
            Title,
            Story,
            Gallery,
        }

        public enum InputAction
        {
            None,
            UpdateDialogue,
            SkipDialogue,
            ShowHistory,
            HideUI,
        }

        #endregion
        
        #region <Fields>

        public KeyInputType[] _KeyInputTypeEnumerator;

        public IKeyInput _KeyInputObject;

        private DialogueModuleCluster<KeyInputType, IKeyInput> _KeyInputModule;

        #endregion

        #region <Callbacks>

        private void OnCreatedKeyInput()
        {
            _KeyInputTypeEnumerator = SystemTool.GetEnumEnumerator<KeyInputType>(SystemTool.GetEnumeratorType.ExceptNone);
        }

        #endregion

        #region <Method>

        public void SwitchInputAction()
        {
            _KeyInputObject = _KeyInputModule.SwitchModule();
        }

        public void SwitchInputAction(KeyInputType p_ModuleType)
        {
            _KeyInputObject = _KeyInputModule.SwitchModule(p_ModuleType);
        }
        
        public void SwitchInputAction(int p_Index)
        {
            _KeyInputObject = _KeyInputModule.SwitchModule(p_Index);
        }
        

        protected void LoadKeyInput(List<int> p_IndexList)
        {
            _KeyInputModule = new DialogueModuleCluster<KeyInputType, IKeyInput>(
                this, DialogueModuleDataTool.DialogueModuleType.KeyInput, p_IndexList);
            _KeyInputObject = (IKeyInput) _KeyInputModule.CurrentSelectedModule;
        }

        #endregion
    }
}
