namespace k514
{
    public class IntRecord : ValueRecord<int>
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