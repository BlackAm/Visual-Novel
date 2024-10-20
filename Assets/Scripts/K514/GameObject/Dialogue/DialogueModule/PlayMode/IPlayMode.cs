namespace BlackAm
{
    public interface IPlayMode : IDialogue
    {
        DialogueGameManager.DialoguePlayMode _PlayMode { get; }

        void UpdateDialogueState(DialogueGameManager.DialogueState p_DialogueState);
    }
}