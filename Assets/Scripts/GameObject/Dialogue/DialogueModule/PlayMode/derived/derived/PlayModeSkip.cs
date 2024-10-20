namespace BlackAm
{
    public class PlayModeSkip : DialoguePlayModeBase
    {
        protected override void OnModuleNotify()
        {
            base.OnModuleNotify();

            // var isDialogueActable = _MasterNode.SetDialogue(_MasterNode.NextDialogueKey);
        }

        protected override void OnModuleSleep()
        {
            base.OnModuleSleep();
        }
        
        public override void OnUpdate(float p_DeltaTime)
        {
            if (_MasterNode.isDialogueEventEnd && !MainGameUI.Instance.mainUI.IsUIActive(MainUI.UIList.TopCenter))
            {
                _MasterNode.SetDialogue(_MasterNode.NextDialogueKey);
            }
        }
    }
}