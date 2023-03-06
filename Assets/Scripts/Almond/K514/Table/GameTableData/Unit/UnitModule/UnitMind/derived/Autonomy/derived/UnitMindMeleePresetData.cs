namespace k514
{
    public class UnitMindMeleePresetData : UnitMindAutonomyPresetData<UnitMindMeleePresetData, UnitMindMeleePresetData.MindTableRecord>
    {
        public class MindTableRecord : AutonomyMindTableBaseRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitMindMeleePresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 500;
            EndIndex = 600;
        }

        public override UnitAIDataRoot.UnitMindType GetThisLabelType()
        {
            return UnitAIDataRoot.UnitMindType.Autonomy_MeleeSimple;
        }
    }
}