using Cysharp.Threading.Tasks;

namespace BlackAm
{
    public class SystemConfigData : SystemTable<SystemConfigData, ResourceType, SystemConfigData.SystemConfigPresetRecord>
    {
        public class SystemConfigPresetRecord : SystemTableRecordBase
        {
            public AssetLoadType ResourceLoadType { get; private set; }

            public override async UniTask SetRecord(ResourceType p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                ResourceLoadType = (AssetLoadType)p_RecordField[0];
            }
        }

        /// <summary>
        /// 해당 프로젝트에서 하나라도 에셋번들 로드 타입의 리소스 타입을 가지는지 검증하는 메서드.
        /// 대부분의 프로젝트의 경우 True 이다.
        /// </summary>
        public bool HasBundleResource;

        protected override async UniTask OnCreated()
        {
            // 테이블을 읽고 컬렉션을 초기화 시킨다.
            await base.OnCreated();

            if (SystemTool.TryGetEnumEnumerator<ResourceType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var resourceType in o_Enumerator)
                {
                    if (resourceType.GetResourceLoadType() == AssetLoadType.FromAssetBundle)
                    {
                        HasBundleResource = true;
                        break;
                    }
                }
            }
        }
        
        protected override async UniTask CheckMissedRecordSet()
        {
            await base.CheckMissedRecordSet();
            
            if (SystemTool.TryGetEnumEnumerator<ResourceType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var resourceType in o_Enumerator)
                {
                    if (!resourceType.IsFixedLoadTypeResource() && !HasKey(resourceType))
                    {
                        await AddRecord(resourceType, AssetLoadType.FromUnityResource);
                    }
                }
            }
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// 테이블 파일이 업데이트 된 경우 수행할 작업을 기술하는 콜백
        /// </summary>
        public override async UniTask OnUpdateTableFile()
        {
            await base.OnUpdateTableFile();
            
            // 1. 리소스 로드 타입이 변경되는 경우에는 씬 로드 타입이 변경되는 경우도 포함되어 있기 때문에
            // 빌드 세팅을 체크해주어야한다.
            await SystemMaintenance.CreateBuildSetting();
        }

        /// <summary>
        /// 해당 테이블에 들어갈 기본 값을 테이블 컬렉션에 레코드 인스턴스로 추가하는 메서드
        /// </summary>
        protected override async UniTask GenerateDefaultRecordSet()
        {
            await base.GenerateDefaultRecordSet();

            // 모든 리소스 로드 타입에 대해 각 리소스 타입이 '유니티 로드 타입'으로 가지도록 레코드를 테이블에 세트한다.
            if (SystemTool.TryGetEnumEnumerator<ResourceType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var resourceType in o_Enumerator)
                {
                    // FixedLoadType은 로드 타입이 항상 유니티 로드 타입으로 고정이므로 고려할 필요가 없다.
                    if (resourceType.IsFixedLoadTypeResource())
                    {
                        continue;

                    }
                    else
                    {
                        await AddRecord(resourceType, AssetLoadType.FromUnityResource);
                    }
                }
            }
        }
#endif

        protected override string GetDefaultTableFileName()
        {
            return "SystemConfig";

        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}