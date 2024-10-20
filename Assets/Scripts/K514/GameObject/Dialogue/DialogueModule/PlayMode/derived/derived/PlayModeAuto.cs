namespace BlackAm
{
    public class PlayModeAuto : DialoguePlayModeBase
    {
        protected DialogueGameManager.DialogueState CurrentDialogueState;
        
        private SafeReference<object, GameEventTimerHandlerWrapper> _TimerEventHandler;
        
        protected override void OnModuleNotify()
        {
            base.OnModuleNotify();

            // var isDialogueActable = _MasterNode.SetDialogue(_MasterNode.NextDialogueKey);
            CurrentDialogueState = DialogueGameManager.DialogueState.None;
        }

        protected override void OnModuleSleep()
        {
            base.OnModuleSleep();
            
            var (valid, eventHandler) = _TimerEventHandler.GetValue();
            if (valid)
            {
                eventHandler.PauseEvent();
            }
        }
        
        public override void OnUpdate(float p_DeltaTime)
        {
            if (CurrentDialogueState == DialogueGameManager.DialogueState.None && _MasterNode.isDialogueEventEnd &&
                !MainGameUI.Instance.mainUI.IsUIActive(MainUI.UIList.TopCenter))
            {
                UpdateDialogueState(DialogueGameManager.DialogueState.TimerRunning);

                GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _TimerEventHandler, this,
                    SystemBoot.TimerType.GameTimer, false);
                var (_, eventHandler) = _TimerEventHandler.GetValue();
                eventHandler.AddEvent(PlayMode.PreDelay, handler =>
                {
                    handler.Arg1.SetDialogue(handler.Arg1.NextDialogueKey);
                    handler.PauseEvent();
                    return true;
                }, null, _MasterNode);
                eventHandler.StartEvent();
            }
        }

        public override void UpdateDialogueState(DialogueGameManager.DialogueState p_DialogueState)
        {
            CurrentDialogueState = p_DialogueState;
        }
    }
}