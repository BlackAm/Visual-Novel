
namespace BlackAm
{
    public class GalleryPoolingManager : AbstractPrefabPoolingManager<GalleryItem>
    {
        private int debug = 0;
        public override UnityPrefabObjectPool<GalleryItem> Initialize()
        {
            base.Initialize();
            prefabName = "GalleryItem.prefab";
            return this;
        }

        protected override void OnCreate(GalleryItem obj)
        {
            obj.name = (++debug).ToString();
            obj.Initialize();
        }

        protected override void OnActive(GalleryItem obj)
        {
            obj.SetActive(true);
        }

        protected override void OnPooled(GalleryItem obj)
        {
            obj.SetActive(false);
        }
    }
}