namespace k514
{
    public abstract class PrefabExtraData_AutoMuttonBase<M, T> : PrefabExtraDataIntTable<M, T>, PrefabExtraDataTableBridge where M : PrefabExtraData_AutoMuttonBase<M, T>, new() where T : PrefabExtraData_AutoMuttonBase<M, T>.AutoMuttonBaseTableRecord, new()
    {
        public abstract class AutoMuttonBaseTableRecord : PrefabExtraDataRecord, AutoMuttonExtraDataRecordBridge
        {
            public AutoMuttonTool.AutoMuttonAffineType AutoMuttonAffineType { get; protected set; }
            public AutoMuttonTool.AutoMuttonSampleType AutoMuttonSampleType { get; protected set; }
            public int VectorMapIndex { get; protected set; }
            public int DeployEventIndex { get; protected set; }
            public SamplePositionTool.BlizzardSamplingIndexType BlizzardSamplingIndexType { get; protected set; }
            public ObjectDeployTimelinePreset ObjectDeployTimelinePreset { get; protected set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}