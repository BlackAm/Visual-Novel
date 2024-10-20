
namespace BlackAm
{
    public class DialogueKeyInputStoryPresetData : DialogueKeyInputDataBase<DialogueKeyInputStoryPresetData, DialogueKeyInputStoryPresetData.KeyInputTableRecord>
    {
        public class KeyInputTableRecord : KeyInputTableBaseRecord
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialogueKeyInputStoryPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100;
            EndIndex = 200;
        }

        public override DialogueGameManager.KeyInputType GetThisLabelType()
        {
            return DialogueGameManager.KeyInputType.Story;
        }
    }
}