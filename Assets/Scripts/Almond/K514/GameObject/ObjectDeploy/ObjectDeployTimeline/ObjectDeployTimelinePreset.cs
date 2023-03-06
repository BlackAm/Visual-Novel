namespace k514
{
    public struct ObjectDeployTimelinePreset
    {
        public int Count;
        public float Interval;
        public float Scale;

        public ObjectDeployTimelinePreset(int p_Count, float p_Interval, float p_Scale)
        {
            Count = p_Count;
            Interval = p_Interval;
            Scale = p_Scale;
        }
    }
}