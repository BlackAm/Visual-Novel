namespace BlackAm
{
    public interface IDialogue : _IDisposable
    {
        DialogueModuleDataTool.DialogueModuleType DialogueModuleType { get; }
        DialogueGameManager _MasterNode { get; }
        void OnMasterNodePooling();
        void OnMasterNodeRetrieved();
        void OnUpdate(float p_DeltaTime);
        void TryModuleNotify();
        void TryModuleSleep();
    }
}