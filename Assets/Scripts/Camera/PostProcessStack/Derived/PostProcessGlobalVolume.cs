#if !SERVER_DRIVE && APPLY_PPS
namespace BlackAm
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