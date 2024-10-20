using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    public interface IIndexableGalleryTableBridge : ITableBase
    {
    }

    public interface IIndexableGalleryRecordBridge : ITableBaseRecord
    {
        int ThumbnailImageKey { get; }
        List<int> ImageKey { get; }
    }

    public class GalleryManager : MultiTableProxy<GalleryManager, int ,GalleryManager.GalleryType, IIndexableGalleryTableBridge, IIndexableGalleryRecordBridge>
    {
        #region <Fields>

        public Dictionary<GalleryType, Dictionary<int, IIndexableGalleryRecordBridge>> _GalleryCollection;

        #endregion
        
        #region <Enum>

        public enum GalleryType
        {
            CharacterImage,
            EventCG,
            Scene,
        }

        #endregion

        protected override async UniTask OnCreated()
        {
            await base.OnCreated();

            _GalleryCollection = new Dictionary<GalleryType, Dictionary<int, IIndexableGalleryRecordBridge>>();
            foreach (var galleryType in GameDataTableCluster._LabelEnumerator)
            {
                _GalleryCollection.Add(galleryType, new Dictionary<int, IIndexableGalleryRecordBridge>());
                SetGalleryData(galleryType, GameDataTableCluster[galleryType].GetValidKeyEnumerator());
            }
        }

        public void SetGalleryData(GalleryType p_GalleryType, List<int> p_GalleryKeys)
        {
            foreach (var galleryKey in p_GalleryKeys)
            {
                var targetRecord = GetInstanceUnSafe[galleryKey];
                _GalleryCollection[p_GalleryType].Add(galleryKey, targetRecord);
            }
        }

        public int GetGalleryCount()
        {
            return _GalleryCollection.Count;
        }

        public (bool, GalleryType, int, int) GetGalleryImageData(int p_ImageKey)
        {
            foreach (var gallery in _GalleryCollection)
            {
                foreach (var galleryInfo in gallery.Value)
                {
                    for (int i = 0; i < galleryInfo.Value.ImageKey.Count; i++)
                    {
                        if (galleryInfo.Value.ImageKey[i] == p_ImageKey)
                        {
                            return (true, gallery.Key, galleryInfo.Key, i);
                        }
                    }
                }
            }

            return (false, GalleryType.CharacterImage, 0, 0);
        }

        protected override MultiTableIndexer<int, GalleryType, IIndexableGalleryRecordBridge> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<GalleryType, IIndexableGalleryRecordBridge>();
        }
    }
}