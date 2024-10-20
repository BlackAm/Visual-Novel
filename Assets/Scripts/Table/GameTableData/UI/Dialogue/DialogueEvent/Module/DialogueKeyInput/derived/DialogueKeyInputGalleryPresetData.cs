
namespace BlackAm
{
    public class DialogueKeyInputGalleryPresetData : DialogueKeyInputDataBase<DialogueKeyInputGalleryPresetData, DialogueKeyInputGalleryPresetData.KeyInputTableRecord>
    {
        public class KeyInputTableRecord : KeyInputTableBaseRecord
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialogueKeyInputGalleryPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 200;
            EndIndex = 300;
        }

        public override DialogueGameManager.KeyInputType GetThisLabelType()
        {
            return DialogueGameManager.KeyInputType.Gallery;
        }
    }
}