using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class GalleryItem : AbstractUI
    {
        public Button button;

        public int galleryKey;
        public List<int> imageKeyList;
        
        public Image ThumbnailImage;
        public int ThumbnailImageKey;
        public int SceneKey;

        public GalleryManager.GalleryType galleryType;
        
        public override void Initialize()
        {
            ThumbnailImage = GetComponent<Image>("ThumbnailImage");

            button = GetComponent<Button>();
            button.onClick.AddListener(ShowGallery);

            imageKeyList = new List<int>();
        }

        public void ShowGallery()
        {
            switch (galleryType)
            {
                case GalleryManager.GalleryType.CharacterImage:
                case GalleryManager.GalleryType.EventCG:
                    break;
                case GalleryManager.GalleryType.Scene:
                    break;
            }
        }

        public void SetGalleryData(GalleryManager.GalleryType p_GalleryType, int p_GalleryKey, IIndexableGalleryRecordBridge p_GalleryData)
        {
            galleryKey = p_GalleryKey;
            galleryType = p_GalleryType;
            SetThumbnailKey(p_GalleryData.ThumbnailImageKey);

            if (galleryType == GalleryManager.GalleryType.Scene)
            {
                var sceneGallery = SceneGallery.GetInstanceUnSafe[p_GalleryKey];
                SceneKey = sceneGallery.SceneKey;
            }
        }

        public void SetThumbnailKey(int p_Key)
        {
            ThumbnailImageKey = p_Key;
        }

        public void SetThumbnailImageByType()
        {
            switch (galleryType)
            {
                case GalleryManager.GalleryType.CharacterImage:
                case GalleryManager.GalleryType.EventCG:
                    SetImageThumbnailImage();
                    break;
                case GalleryManager.GalleryType.Scene:
                    SetSceneThumbnailImage();
                    break;
            }
        }

        public void SetImageThumbnailImage()
        {
            int imageKey = 0;
            var galleryUnLockInfoCollection =
                SaveLoadManager.GetInstanceUnSafe.gallerySaveData.GallerySaveInfo[galleryType][galleryKey]
                    .GalleryUnLockInfo;

            foreach (var galleryUnLockInfo in galleryUnLockInfoCollection)
            {
                if (galleryUnLockInfo.Value)
                {
                    imageKey = galleryUnLockInfo.Key;
                    break;
                }
            }
            
            SetThumbnailImage(imageKey);
        }

        public void SetSceneThumbnailImage()
        {
            int imageKey = 0;
            var galleryUnLockInfoCollection =
                SaveLoadManager.GetInstanceUnSafe.gallerySaveData.GallerySaveInfo[galleryType][galleryKey]
                    .GalleryUnLockInfo;
            
            if (galleryUnLockInfoCollection[SceneKey])
            {
                imageKey = ThumbnailImageKey;
            }

            SetThumbnailImage(imageKey);
        }

        public void SetThumbnailImage(int p_ImageKey)
        {
            ThumbnailImage.sprite = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(ResourceType.Image, ResourceLifeCycleType.Scene,
                ImageNameTableData.GetInstanceUnSafe.GetTableData(p_ImageKey).ResourceName).Item2;
        }
    }
}