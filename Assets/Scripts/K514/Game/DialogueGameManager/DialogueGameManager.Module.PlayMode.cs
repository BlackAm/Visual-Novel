using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        #region <Const>

        public const int DefaultPlayModeKey = 0;

        #endregion

        #region <Enum>

        public enum DialoguePlayMode
        {
            BasicPlay,
            AutoPlay,
            SkipPlay,
        }

        public enum DialogueState
        {
            None,
            
            TimerRunning,
            
            DialogueAction,
        }

        #endregion

        #region <Fields>

        public IPlayMode _PlayModeObject;

        private DialogueModuleCluster<DialoguePlayMode, IPlayMode> _PlayModeModule;
        
        public DialoguePlayMode _PlayMode;

        #endregion

        #region <Callbacks>

        private void OnInitiatePlayMode()
        {
            var playMode = DialoguePlayModePresetData.GetInstanceUnSafe[DefaultPlayModeKey];

            _PlayModeModule = new DialogueModuleCluster<DialoguePlayMode, IPlayMode>(
                this, DialogueModuleDataTool.DialogueModuleType.DialoguePlayMode, playMode.PlayModeList);
            SwitchPlayMode();
        }

        public void OnTogglePlayMode(DialoguePlayMode p_PlayMode)
        {
            switch (p_PlayMode)
            {
                case DialoguePlayMode.BasicPlay:
                    OnUpdatePlayMode(p_PlayMode);
                    break;
                case DialoguePlayMode.AutoPlay:
                case DialoguePlayMode.SkipPlay:
                    OnUpdatePlayMode(p_PlayMode == _PlayMode ? DialoguePlayMode.BasicPlay : p_PlayMode);
                    break;
            }
        }

        public void OnUpdatePlayMode(DialoguePlayMode p_PlayMode)
        {
            // Auto 버튼 UI 변경
            SwitchPlayMode(p_PlayMode);

            _PlayMode = p_PlayMode;
        }

        #endregion

        #region <Method>

        public void SwitchPlayMode()
        {
            _PlayModeObject = _PlayModeModule.SwitchModule();
        }

        public void SwitchPlayMode(DialoguePlayMode p_PlayMode)
        {
            _PlayModeObject = _PlayModeModule.SwitchModule(p_PlayMode);
        }

        public void SwitchPlayMode(int p_Index)
        {
            _PlayModeObject = _PlayModeModule.SwitchModule(p_Index);
        }

        public void UpdateDialogueState(DialogueState p_DialogueState)
        {
            _PlayModeObject.UpdateDialogueState(p_DialogueState);
        }

        #endregion
    }
}