using UnityEngine;

namespace k514
{
    /// <summary>
    /// 특정 에셋 이름을 관리 및 해당 이름의 에셋을 리턴하는 테이블의 추상 클래스
    /// </summary>
    /// <typeparam name="M">서브 클래스 테이블 타입</typeparam>
    /// <typeparam name="K">서브 클래스 키 타입</typeparam>
    /// <typeparam name="T">서브 클래스 레코드 타입</typeparam>
    /// <typeparam name="Me">저장할 유니티 에셋 타입</typeparam>
    public abstract class BaseResourceNameTable<M, K, T, Me> : GameTable<M, K, T> 
        where M : BaseResourceNameTable<M, K, T, Me>, new() 
        where T : BaseResourceNameTable<M, K, T, Me>.BaseTableRecord, new()
        where Me : Object
    {
        public abstract class BaseTableRecord : GameTableRecordBase
        {
            /// <summary>
            /// 해당 리소스의 이름
            /// </summary>
            public string ResourceName { get; protected set; }
            
            /// <summary>
            /// 해당 리소스가 멀티 리소스인지 표시하는 플래그
            /// </summary>
            public bool IsMultiResource { get; protected set; }
        }

        #region <Methods>

        /// <summary>
        /// 지정한 키의 에셋이름을 가지는 리소스를 동기 로드하여 리턴하는 메서드 
        /// </summary>
        public virtual (AssetPreset, Me) GetResource(K p_Key, ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
            var resourceName = GetTableData(p_Key).ResourceName;
            return LoadAssetManager.GetInstanceUnSafe.LoadAsset<Me>(p_ResourceType, p_ResourceLifeCycleType, resourceName);
        }
        
        /// <summary>
        /// 지정한 키의 에셋이름을 가지는 리소스를 동기 로드하여 리턴하는 메서드 
        /// </summary>
        public virtual (AssetPreset, Me[]) GetResources(K p_Key, ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
            var resourceName = GetTableData(p_Key).ResourceName;
            return LoadAssetManager.GetInstanceUnSafe.LoadMultipleAsset<Me>(p_ResourceType, p_ResourceLifeCycleType, resourceName);
        }
        
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
        
        #endregion
    }
}