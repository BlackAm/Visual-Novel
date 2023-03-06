#if !SERVER_DRIVE
using BDG;

namespace UI2020
{
    public class DropItemPoolingManager : AbstractPrefabPoolingManager<DropItem>
    {
        private int debug=0;
        public override UnityPrefabObjectPool<DropItem> Initialize()
        {
            base.Initialize();
            prefabName = "DropItemInfoObject.prefab";
            return this;
        }

        protected override void OnCreate(DropItem obj)
        {
            obj.Initialize();
            obj.name = (debug++).ToString();
        }

        protected override void OnActive(DropItem obj)
        {
            obj.SetAlpha(1f);
            obj.SetActive(true);
        }

        protected override void OnPooled(DropItem obj)
        {
            obj.SetActive(false);
        }
    }
}
#endif