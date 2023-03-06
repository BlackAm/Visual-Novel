using UnityEngine;

namespace k514
{
    public class ImageNameTableData : BaseResourceNameTable<ImageNameTableData, int, ImageNameTableData.TableRecord, Sprite>
    {
        public class TableRecord : BaseTableRecord
        {
#if !SERVER_DRIVE
            public bool IsSprite { get; private set; }
            public int ModelKey { get; private set; }
#endif
            public int EquipmentKey { get; private set; }
            public int AutoItemKey { get; private set; }
        }

#if !SERVER_DRIVE
        /// <summary>
        /// 지정한 키의 에셋이름을 가지는 리소스를 동기 로드하여 리턴하는 메서드 
        /// </summary>
        public virtual (AssetPreset, Texture) GetTexture(int p_Key, ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
            var targetRecord = GetTableData(p_Key);
            var resourceName = targetRecord.ResourceName;
            if (targetRecord.IsSprite)
            {
                var spriteTuple = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(p_ResourceType, p_ResourceLifeCycleType, resourceName);
                return (spriteTuple.Item1, spriteTuple.Item2.texture);
            }
            else
            {
                return LoadAssetManager.GetInstanceUnSafe.LoadAsset<Texture>(p_ResourceType, p_ResourceLifeCycleType, resourceName);
            }
        }
#endif

        protected override string GetDefaultTableFileName()
        {
            return "ImageNameTable";
        }
    }
}
