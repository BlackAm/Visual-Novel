namespace k514
{
    public class FloatRecord : ValueRecord<float>
    {
        protected override void SyncTo()
        {
            CurrentRecordedValue = PreviousRecordedValue + CurrentFocusedValue;
            PreviousRecordedValue = default;
            CurrentFocusedValue = default;
            _CurrentPhase = RecordPhase.Synced; 
        }
    }
}