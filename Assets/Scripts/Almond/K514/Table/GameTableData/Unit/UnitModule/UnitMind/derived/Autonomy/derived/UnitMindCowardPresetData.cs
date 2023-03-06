namespace k514
{
    public class UnitMindCowardPresetData : UnitMindAutonomyPresetData<UnitMindCowardPresetData, UnitMindCowardPresetData.MindTableRecord>
    {
        public class MindTableRecord : AutonomyMindTableBaseRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitMindCowardPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 600;
            EndIndex = 700;
        }

        public override UnitAIDataRoot.UnitMindType GetThisLabelType()
        {
            return UnitAIDataRoot.UnitMindType.Autonomy_Coward;
        }
    }
}