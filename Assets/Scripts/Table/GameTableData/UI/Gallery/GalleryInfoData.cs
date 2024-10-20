using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public abstract class BaseGalleryData<M, T> 
        : MultiTableBase<M, int, T, GalleryManager.GalleryType, IIndexableGalleryRecordBridge>, IIndexableGalleryTableBridge
        where M : BaseGalleryData<M, T>, new()
        where T : BaseGalleryData<M, T>.BaseGalleryContent, new()
    {
        public class BaseGalleryContent : GameTableRecordBase, IIndexableGalleryRecordBridge
        {
            public int ThumbnailImageKey { get; protected set; }
            
            public List<int> ImageKey { get; protected set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override MultiTableIndexer<int, GalleryManager.GalleryType, IIndexableGalleryRecordBridge> GetMultiGameIndex()
        {
            return GalleryManager.GetInstanceUnSafe.GameDataTableCluster;
        }
    }

    public class CharacterImageGallery : BaseGalleryData<CharacterImageGallery, CharacterImageGallery.GalleryContent>
    {
        public class GalleryContent : BaseGalleryContent
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "CharacterImageGallery";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 10000;
        }

        public override GalleryManager.GalleryType GetThisLabelType()
        {
            return GalleryManager.GalleryType.CharacterImage;
        }
    }
    
    public class EventCGGallery : BaseGalleryData<EventCGGallery, EventCGGallery.GalleryContent>
    {
        public class GalleryContent : BaseGalleryContent
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "EventCGGallery";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 10000;
            EndIndex = 20000;
        }

        public override GalleryManager.GalleryType GetThisLabelType()
        {
            return GalleryManager.GalleryType.EventCG;
        }
    }
    
    public class SceneGallery : BaseGalleryData<SceneGallery, SceneGallery.GalleryContent>
    {
        public class GalleryContent : BaseGalleryContent
        {
            public int SceneKey;
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneGallery";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 20000;
            EndIndex = 30000;
        }

        public override GalleryManager.GalleryType GetThisLabelType()
        {
            return GalleryManager.GalleryType.Scene;
        }
    }
}
