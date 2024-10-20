using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class GalleryCategorySelectButton : AbstractUI
    {
        public GalleryManager.GalleryType galleryType;

        public void Initialize(GalleryManager.GalleryType p_GalleryType)
        {
            galleryType = p_GalleryType;
            GetComponent<Button>().onClick.AddListener(OnClickButton);

            Unselected();
        }

        public void OnClickButton()
        {
            TitleMenuUI.Instance.gallery.Category = this;
        }

        public void Selected()
        {
        }

        public void Unselected()
        {
            
        }
    }
}