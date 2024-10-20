
namespace BlackAm
{
    public class DialogueKeyInputMainPresetData : DialogueKeyInputDataBase<DialogueKeyInputMainPresetData, DialogueKeyInputMainPresetData.KeyInputTableRecord>
    {
        public class KeyInputTableRecord : KeyInputTableBaseRecord
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialogueKeyInputMainPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 100;
        }

        public override DialogueGameManager.KeyInputType GetThisLabelType()
        {
            return DialogueGameManager.KeyInputType.Title;
        }
    }
}