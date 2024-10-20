namespace BlackAm
{
    public partial class DialogueKeyInputBase
    {
        public bool OnHandleDialogueUpdate()
        {
            if (!MainGameUI.Instance.mainUI.IsUIActive(MainUI.UIList.BottomCenter))
            {
                OnHandleHideUI();
                return true;
            }
                
            
            _MasterNode.SetDialogue(_MasterNode.NextDialogueKey);
            return true;
        }

        public bool OnHandleSkipDialogue()
        {
            return true;
        }
    }
}