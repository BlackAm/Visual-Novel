namespace k514
{
    public interface IValueRecord
    {
        void OnRecordStart();
        void OnRecordTerminate();
    }

    public abstract class ValueRecord<T> : IValueRecord
    {
        #region <Fields>

        public T PreviousRecordedValue { get; set; }
        public T CurrentRecordedValue { get; set; }
        public T CurrentFocusedValue { get; set; }
        protected RecordPhase _CurrentPhase;
        
        #endregion

        #region <Enums>

        public enum RecordPhase
        {
            Synced,
            Recording,
        }

        #endregion

        #region <Callbacks>

        public void OnRecordStart()
        {
            switch (_CurrentPhase)
            {
                case RecordPhase.Synced:
                    PreviousRecordedValue = CurrentRecordedValue;
                    CurrentRecordedValue = default;
                    CurrentFocusedValue = default;
                    _CurrentPhase = RecordPhase.Recording;
                    break;
                case RecordPhase.Recording:
                    break;
            }
        }

        public void OnRecordTerminate()
        {
            switch (_CurrentPhase)
            {
                case RecordPhase.Synced:
                    break;
                case RecordPhase.Recording:
                    SyncTo();
                    break;
            }
        }

        #endregion
        
        #region <Methods>

        protected abstract void SyncTo();

        #endregion
    }
}