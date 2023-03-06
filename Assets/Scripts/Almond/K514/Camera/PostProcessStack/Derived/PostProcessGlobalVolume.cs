#if !SERVER_DRIVE && APPLY_PPS
namespace k514
{
    public class PostProcessGlobalVolume : PostProcessObjectBase
    {
        public override void OnSpawning()
        {
            base.OnSpawning();
            _Volume.isGlobal = true;
        }
    }
}
#endif