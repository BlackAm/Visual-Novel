using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlackAm
{
    public partial class SaveLoadManager
    {
        #region <Fields>

        public GallerySaveData gallerySaveData;

        #endregion

        #region <Callbacks>

        public void OnGallerySaveDataCreated()
        {
            var galleryData = GetDataFileInDirectory<GallerySaveData>(SystemMaintenance.SaveGalleryFile);
            if (galleryData.Item1)
            {
                gallerySaveData = galleryData.Item2;
            }
            else
            {
                MakeGallerySaveDataFile();
            }

            var isGalleryAdded = CompareGalleryCount();
            if (!isGalleryAdded)
            {
                AddGalleryFile();
            }

            CompareGalleryImageCount();

            TitleMenuUI.Instance.gallery.OnGallerySaveDataLoaded();
        }

        #endregion

        #region <Methods>

        public void MakeGallerySaveDataFile()
        {
            gallerySaveData = new GallerySaveData();

            foreach (var galleryData in GalleryManager.GetInstanceUnSafe._GalleryCollection)
            {
                gallerySaveData.GallerySaveInfo.Add(galleryData.Key, new Dictionary<int, GalleryUnLockData>());

                foreach (var galleryInfo in galleryData.Value)
                {
                    SetGallerySaveDataFileByGalleryType(galleryData.Key, galleryInfo.Key, galleryInfo.Value.ImageKey);
                }
            }
        }

        public void SetGallerySaveDataFileByGalleryType(GalleryManager.GalleryType p_GalleryType, int p_GalleryKey, List<int> p_ImageList)
        {
            switch (p_GalleryType)
            {
                case GalleryManager.GalleryType.CharacterImage:
                case GalleryManager.GalleryType.EventCG:
                    gallerySaveData.GallerySaveInfo[p_GalleryType].Add(p_GalleryKey, new GalleryUnLockData(p_ImageList));
                    break;
                case GalleryManager.GalleryType.Scene:
                    var sceneGallery = SceneGallery.GetInstanceUnSafe[p_GalleryKey];
                    gallerySaveData.GallerySaveInfo[p_GalleryType].Add(p_GalleryKey, new GalleryUnLockData(sceneGallery.SceneKey));
                    break;
            }
        }

        public bool CompareGalleryCount()
        {
            return gallerySaveData.GallerySaveInfo.Count != GalleryManager.GetInstanceUnSafe.GetGalleryCount();
        }

        public bool CompareGalleryImageCount()
        {
            var galleryDataCollection = GalleryManager.GetInstanceUnSafe._GalleryCollection;
            foreach (var galleryData in galleryDataCollection)
            {
                if (galleryData.Key == GalleryManager.GalleryType.Scene) continue;
                foreach (var galleryInfo in galleryData.Value)
                {
                    if (gallerySaveData.GallerySaveInfo[galleryData.Key][galleryInfo.Key].GalleryUnLockInfo.Count <
                        galleryInfo.Value.ImageKey.Count)
                    {
                        UnityEngine.Debug.LogWarning($"Gallery Update GalleryType : {galleryData.Key} GalleryKey : {galleryInfo.Key} CurrentCount {gallerySaveData.GallerySaveInfo[galleryData.Key][galleryInfo.Key].GalleryUnLockInfo.Count} Update To {galleryInfo.Value.ImageKey.Count}");

                        for (int i = 0; i < galleryInfo.Value.ImageKey.Count; i++)
                        {
                            if (!gallerySaveData.GallerySaveInfo[galleryData.Key][galleryInfo.Key].GalleryUnLockInfo
                                .ContainsKey(galleryInfo.Value.ImageKey[i]))
                            {
                                gallerySaveData.GallerySaveInfo[galleryData.Key][galleryInfo.Key].GalleryUnLockInfo.Add(galleryInfo.Value.ImageKey[i], false);
                            }
                        }
                    }
                    else if (gallerySaveData.GallerySaveInfo[galleryData.Key][galleryInfo.Key].GalleryUnLockInfo
                        .Count > galleryInfo.Value.ImageKey.Count)
                    {
                        UnityEngine.Debug.LogError($"Gallery Update Error GalleryType : {galleryData.Key} GalleryKey : {galleryInfo.Key} CurrentCount {gallerySaveData.GallerySaveInfo[galleryData.Key][galleryInfo.Key].GalleryUnLockInfo.Count} Info Count {galleryInfo.Value.ImageKey.Count}");
                    }
                }
            }

            return true;
        }

        public void AddGalleryFile()
        {
            var galleryDataCollection = GalleryManager.GetInstanceUnSafe._GalleryCollection;
            foreach (var galleryData in galleryDataCollection)
            {
                foreach (var galleryInfo in galleryData.Value)
                {
                    if (!gallerySaveData.GallerySaveInfo[galleryData.Key].ContainsKey(galleryInfo.Key))
                    {
                        gallerySaveData.GallerySaveInfo[galleryData.Key].Add(galleryInfo.Key, new GalleryUnLockData(galleryInfo.Value.ImageKey));
                    }
                }
            }
        }

        #endregion
    }
}
