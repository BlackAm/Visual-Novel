using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class Gallery : AbstractUI
    {
        #region <Fields>
        
        private GalleryPoolingManager galleryPoolingmanager;
        public UIScroll<GalleryItem> galleryItemScroll;

        public Dictionary<GalleryManager.GalleryType, List<GalleryItem>> galleryItemCollection;

        public GalleryCategorySelectButton _curCategory;
        public List<GalleryCategorySelectButton> galleryCategorySelectButtons;

        public GalleryCategorySelectButton Category
        {
            get => _curCategory;
            set
            {
                if(_curCategory != null && _curCategory != value) _curCategory.Unselected();
                _curCategory = value;
                _curCategory.Selected();
                SetGalleryItem(_curCategory.galleryType);
            }
        }

        #endregion
        
        #region <Callbacks>

        public override void Initialize()
        {
            LoadUIObjectAsync("Gallery.prefab", () =>
            {
                galleryPoolingmanager = (GalleryPoolingManager) AddComponent<GalleryPoolingManager>("Gallery/GalleryFiles/Viewport/Content").Initialize();
                galleryItemScroll = new UIScroll<GalleryItem>(galleryPoolingmanager,
                    galleryPoolingmanager.GetComponent<RectTransform>(), 0, 0);

                galleryItemCollection = new Dictionary<GalleryManager.GalleryType, List<GalleryItem>>();
                galleryCategorySelectButtons = new List<GalleryCategorySelectButton>();

                foreach (var galleryCollection in GalleryManager.GetInstanceUnSafe._GalleryCollection)
                {
                    galleryItemCollection.Add(galleryCollection.Key, new List<GalleryItem>());
                    var galleryCategorySelectButton = AddComponent<GalleryCategorySelectButton>($"Gallery/CategoryButton/{galleryCollection.Key}");
                    galleryCategorySelectButton.Initialize(galleryCollection.Key);
                    galleryCategorySelectButtons.Add(galleryCategorySelectButton);

                    foreach (var galleryInfo in galleryCollection.Value)
                    {
                        var item = galleryItemScroll.AddContent();
                        item.SetGalleryData(galleryCollection.Key, galleryInfo.Key, galleryInfo.Value);
                        galleryItemCollection[galleryCollection.Key].Add(item);
                        item.SetActive(false);
                    }
                }
                
                GetComponent<Text>("Gallery/Text").text = LanguageManager.GetContent(20040);
                GetComponent<Button>("Gallery/Back").onClick.AddListener(ReturnToMainMenu);
            }, ResourceLifeCycleType.Scene);
        }

        public void OnGallerySaveDataLoaded()
        {
            for (int i = galleryItemScroll.Count - 1; i >= 0; i--)
            {
                galleryItemScroll._contentList[i].SetThumbnailImageByType();
            }
        }
        

        #endregion

        #region <Method>

        public void SetGalleryItem(GalleryManager.GalleryType p_GalleryType)
        {
            for (int i = galleryItemScroll.Count - 1; i >= 0; i--)
            {
                galleryItemScroll._contentList[i].SetActive(false);
            }

            for (int i = galleryItemCollection[p_GalleryType].Count - 1; i >= 0; i--)
            {
                galleryItemCollection[p_GalleryType][i].SetActive(true);
            }
        }

        public void ReturnToMainMenu()
        {
            TitleMenuUI.Instance.ChangeScene(TitleMenuUI.MenuUIList.MainTitle);
        }

        #endregion
    }
}
