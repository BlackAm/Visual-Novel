namespace BlackAm
{
    public class GameEventTimerHandlerWrapper : EventTimerHandlerWrapper<(SystemBoot.TimerType, ResourceLifeCycleType), GameEventTimerHandlerWrapper>
    {
        #region <Callbacks>

        public override void OnSpawning()
        {
        }

        public override void OnPooling()
        {
            SetEventTimer(_SubType.Item1);
        }
        
        #endregion
    }
}