namespace k514
{
    public class UnitMindPassivityPresetData : UnitMindPresetDataBase<UnitMindPassivityPresetData, UnitMindPassivityPresetData.MindTableRecord>, IThinkablePassivityTableBridge
    {
        public class MindTableRecord : MindTableBaseRecord, IThinkablePassivityTableRecordBridge
        {
            public float SearchDistance { get; private set; }
            public bool FindTargetWhenActSpellFlag { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitMindPassivityPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 100;
        }

        public override UnitAIDataRoot.UnitMindType GetThisLabelType()
        {
            return UnitAIDataRoot.UnitMindType.Passivity_Base;
        }
    }
}