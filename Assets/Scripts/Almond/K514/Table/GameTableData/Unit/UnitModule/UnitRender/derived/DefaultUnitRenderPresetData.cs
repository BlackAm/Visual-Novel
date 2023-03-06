namespace k514
{
    public class DefaultUnitRenderPresetData : UnitRenderPresetDataBase<DefaultUnitRenderPresetData, DefaultUnitRenderPresetData.RenderableTableRecord>
    {
        public class RenderableTableRecord : RenderableTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "DefaultUnitRenderPresetTable";
        }

        public override MultiTableIndexer<int, UnitRenderDataRoot.UnitRenderType, IRenderableTableRecordBridge> GetMultiGameIndex()
        {
            return UnitRenderDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 10000;
        }

        public override UnitRenderDataRoot.UnitRenderType GetThisLabelType()
        {
            return UnitRenderDataRoot.UnitRenderType.Default;
        }
    }
}