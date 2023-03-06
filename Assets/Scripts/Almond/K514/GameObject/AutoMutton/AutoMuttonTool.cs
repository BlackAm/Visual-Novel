namespace k514
{
    public interface AutoMuttonExtraDataRecordBridge : PrefabExtraDataRecordBridge
    {
        AutoMuttonTool.AutoMuttonAffineType AutoMuttonAffineType { get; }
        AutoMuttonTool.AutoMuttonSampleType AutoMuttonSampleType { get; }
        int VectorMapIndex { get; }
        int DeployEventIndex { get; }
        SamplePositionTool.BlizzardSamplingIndexType BlizzardSamplingIndexType { get; }
        ObjectDeployTimelinePreset ObjectDeployTimelinePreset { get; }
    }
    
    public static class AutoMuttonTool
    {
        public enum AutoMuttonAffineType
        {
            Transform,
            VectorMap,
        }

        public enum AutoMuttonSampleType
        {
            Transform,
            WideStream,
            Blizzard,
        }
    }
}