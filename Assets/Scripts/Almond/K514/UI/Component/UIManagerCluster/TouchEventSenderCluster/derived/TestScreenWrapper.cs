#if !SERVER_DRIVE
namespace k514
{
    public class TestScreenWrapper : TouchEventSenderCluster
    {
        #region <Fields>

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            RegistInput<TouchEventDragButton>("TouchPanel", TouchEventRoot.TouchInputType.UnitClickEvent);
        }

        #endregion
    }
}
#endif