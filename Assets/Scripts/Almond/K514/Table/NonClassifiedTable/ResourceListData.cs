using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class ResourceListData : TableBase<ResourceListData, string, ResourceListData.ResourceListRecord>
    {
        /// <summary>
        /// 리소스 리스트는 다른 리소스를 기술하기 위해서 별도의 번들 이름을 가져야한다.
        /// </summary>
        public const string GameTableModeResourceListTableBundleName = "Kutsushitadasaimoko";
        
        public class ResourceListRecord : TableRecordBase
        {
            /// <summary>
            /// 에셋 번들 이름
            /// </summary>
            private string AssetBundleName;
            
            /// <summary>
            /// 원본 에셋 위치
            /// </summary>
            private string ResourceFullPath;

            public override async UniTask SetRecord(string p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                ResourceFullPath = p_RecordField[0] as string;
                AssetBundleName = p_RecordField[1] as string;
            }

            public string SetAssetBundleName(string p_TargetName) => AssetBundleName = p_TargetName;
            public string GetAssetBundleName() => AssetBundleName;
            public string GetResourceFullPath() => ResourceFullPath;
        }

        /// <summary>
        /// [에셋이름, 컬렉션유니티 리소스 로드 메서드(Resources.Load)를 통해 로드할 수 있는 포맷]
        /// </summary>
        private Dictionary<string, string> UnityResourceLoadPath;

        /// <summary>
        /// 에셋 번들 이름을 저장하는 리스트
        /// </summary>
        public List<string> AssetBundleNameList { get; private set; }

#if RESOURCE_LIST_TABLE_INTO_GAMETABLE
        /// <summary>
        /// 리소스 리스트 테이블이 게임 데이터에 포함되어있고, 에셋번들로부터 로드되는 경우
        /// </summary>
        public AssetBundle ResourceListAssetBundle;
#endif

        protected override async UniTask OnCreated()
        {
            UnityResourceLoadPath = new Dictionary<string, string>();

#if RESOURCE_LIST_TABLE_INTO_GAMETABLE
            TableType = TableTool.TableType.WholeGameTable;
#else
            TableType = TableTool.TableType.SystemTable;
#endif

            // 시스템 배포 모드가 되었다는 것은 더 이상 리소스 추가가 없다는 것을 의미하므로, 리소스 리스트를 업데이트 할 필요가 없다.
            if (SystemMaintenance.IsPlayOnTargetPlatform())
            {
                // 생성된 테이블을 읽고 컬렉션을 초기화 시킨다.
                await base.OnCreated();
            }
#if UNITY_EDITOR
            // 배포 모드가 아닌 경우, 리소스 리스트 문서를 업데이트하고 그 값을 기반으로 테이블을 작성 혹은 업데이트한다.
            else
            {
                await UniTask.SwitchToMainThread();
                var isPlaying = Application.isPlaying;
                await UniTask.SwitchToThreadPool();
                if (isPlaying)
                {
                    if (SystemFlag.GetAutoUpdateResourceListFlag())
                    {
                        await CreateDefaultTable(true);
                    }
                }
                else
                {
                    await CreateDefaultTable(false);
                }
                
                // 생성된 테이블을 읽고 컬렉션을 초기화 시킨다.
                await base.OnCreated();
            }
#endif
        }

        public override async UniTask OnInitiate()
        {
            await base.OnInitiate();
            
            if (SystemMaintenance.IsPlayOnTargetPlatform())
            {
                AssetBundleNameList = GetAssetBundleNameList().Item2;
            }
            else
            {
                // 개발 모드에서는 CreateDefaultTable에 의해
                // AssetBundleNameList가 초기화 되지만, Reload테이블 메서드에 의해
                // AssetBundleNameList가 날아갔을지도 모르므로, 아래와 같이 처리한다.
                if (AssetBundleNameList == null)
                {
                    AssetBundleNameList = GetAssetBundleNameList().Item2;
                }
            }
            
#if RESOURCE_LIST_TABLE_INTO_GAMETABLE
            ReleaseThisBundle();
#endif
        }

        protected override void OnTableBlowUp()
        {
            base.OnTableBlowUp();
            UnityResourceLoadPath.Clear();
            AssetBundleNameList = null;
        }
        
#if RESOURCE_LIST_TABLE_INTO_GAMETABLE
        /// <summary>
        /// 유니티 리소스 폴더에 위치해야하는 해당 테이블의 경로를 리턴하는 메서드
        /// </summary>
        public string GetResourceListTableAtUnityResourcePath()
        {
            return GetTableFileFullPath(AssetLoadType.FromUnityResource, true, PathType.SystemGenerate_AbsolutePath).CutStringWithPivot(SystemMaintenance.UnityResourceDirectory, false, true);
        }

        /// <summary>
        /// 해당 테이블이 게임테이블로서 취급되는 경우, 에셋 로드타입에 따라 적합한 TextAsset을 리턴하는 메서드
        /// </summary>
        public TextAsset GetResourceListTableTextAsset()
        {
            var assetLoadType = SystemConfig.GetResourceLoadType(ResourceType.GameDataTable);
            switch (assetLoadType)
            {
                case AssetLoadType.FromAssetBundle:
                    if (ResourceListAssetBundle == null)
                    {
                        var bundlePath = SystemMaintenance.GetBundleFullPathOnPlayPlatform() + GameTableDataBundleName;
                        if (File.Exists(bundlePath))
                        {
                            ResourceListAssetBundle = AssetBundle.LoadFromFile(bundlePath);
                        }
                    }
                    return ResourceListAssetBundle.LoadAsset<TextAsset>(GetTableFileNameWithExtention(true));
                case AssetLoadType.FromUnityResource:
                    return SystemTool.Load<TextAsset>(GetTableFileFullPath(AssetLoadType.FromUnityResource, false, PathType.SystemGenerate_RelativePath));
            }

            return null;
        }
        
        /// <summary>
        /// 해당 테이블이 게임테이블로서 취급되는 경우, 전용 번들로 저장되는데 해당 번들을 메모리로부터
        /// 릴리스하는 메서드
        /// </summary>
        public void ReleaseThisBundle()
        {
            if (ResourceListAssetBundle != null)
            {
                ResourceListAssetBundle.Unload(true);
                ResourceListAssetBundle = null;
            }
        }
#endif
        /// <summary>
        /// 현재 테이블에 등록된 에셋번들 이름 리스트를 새로 생성하여 리턴하는 메서드
        /// </summary>
        public (bool, List<string>) GetAssetBundleNameList()
        {
            return GetAssetBundleNameList(GetTable());
        }
        
        /// <summary>
        /// 현재 테이블에 등록된 에셋번들 이름 리스트를 새로 생성하여 리턴하는 메서드
        /// </summary>
        public async UniTask<(bool, List<string>)> GetAssetBundleNameList(string p_AbsolutePath)
        {
            var tryResourceList = await CreateTableFromAbsolutePath(p_AbsolutePath);
            return GetAssetBundleNameList(tryResourceList);
        }
        
        /// <summary>
        /// 시스템에서 지정한 에셋번들 이름 및 리소스 리스트 테이블로부터 에셋 번들 이름을 입력받아
        /// 지정한 포맷으로 인코딩한 리스트로 리턴하는 메서드
        /// </summary>
        public (bool, List<string>) GetAssetBundleNameList(Dictionary<string, ResourceListRecord> p_TargetTable)
        {
            if (ReferenceEquals(null, p_TargetTable))
            {
                return default;
            }
            else
            {
                var targetBundleNameList = new List<string>();
                foreach (var record in p_TargetTable)
                {
                    var tryAssetBundleName = record.Value.GetAssetBundleName();
                    if (tryAssetBundleName != null)
                    {
                        if (!targetBundleNameList.Contains(tryAssetBundleName))
                        {
                            targetBundleNameList.Add(tryAssetBundleName);
                        }
                    }
                }

                // 유니티에서 자동으로 생성하는 번들로 다른 번들에 대한 메타데이터 역할을 하는 에셋번들의 이름
                if (!targetBundleNameList.Contains(SystemMaintenance.DefaultBundleName))
                {
                    targetBundleNameList.Add(SystemMaintenance.DefaultBundleName);
                }

                // 공백 문자열의 경우에는 포함시키지 않는다.
                if (targetBundleNameList.Contains(string.Empty))
                {
                    targetBundleNameList.Remove(string.Empty);
                }

                return (true, targetBundleNameList);
            }
        }

        /// <summary>
        /// 지정한 파일 명으로부터 유니티 리소스 로드 메서드(Resources.Load)를 통해 로드할 수 있는
        /// 파일 경로 문자열을 검색하여 반환하는 메서드
        /// </summary>
        public bool TryGetUnityResourceLoadPath(string p_AssetName, out string o_Result)
        {
            if (UnityResourceLoadPath.TryGetValue(p_AssetName, out o_Result))
            {
                return true;
            }
            else
            {
                var tryTable = GetTable();
                if (tryTable != null && tryTable.TryGetValue(p_AssetName, out var resourceListRecord))
                {
                    var resourceRecord = resourceListRecord;
                    var fileFullName = resourceRecord.GetResourceFullPath();
                    var resourceType = fileFullName.TryGetUnityResourcePath(true);
                    var assetRelativePath = fileFullName.CutString(SystemMaintenance.UnityResourceDirectory, false, true);

                    if (resourceType.IsFixedLoadTypeResource())
                    {
                        o_Result = assetRelativePath.CutString(".", true, false);
                        UnityResourceLoadPath.Add(p_AssetName, o_Result);
                        return true;
                    }
                    else
                    {
                        var resourceLoadType = resourceType.GetResourceLoadType();
                        switch (resourceLoadType)
                        {
                            case AssetLoadType.FromUnityResource:
                                o_Result = assetRelativePath.CutString(".", true, false);
                                UnityResourceLoadPath.Add(p_AssetName, o_Result);
                                return true;
                            default :
                                return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 해당 테이블에 들어갈 기본 값을 테이블 컬렉션에 레코드 인스턴스로 추가하는 메서드
        /// </summary>
        protected override async UniTask GenerateDefaultRecordSet()
        {
            await base.GenerateDefaultRecordSet();
            
            // 에셋번들 이름 리스트를 생성한다.
            AssetBundleNameList = new List<string>();
            
            // 테이블 타입이 반대였던 경우에 남아있을지모르는 테이블 파일을 제거해준다.
            var alternativeTablePath =
                TableTool.GetTablePath(
                    AssetLoadType.FromUnityResource, 
                    PathType.SystemGenerate_AbsolutePath,
                    TableType,
                    TableTool.TableFileType.Xml
                ) 
                + GetTableFileName(TableTool.TableNameType.Alter, true);
                
            if (File.Exists(alternativeTablePath))
            {
                File.Delete(alternativeTablePath);
            }

            // 유니티 리소스 폴더로부터 에셋을 읽고, xml 파일로 만든다.
            var directoryInfo = new DirectoryInfo(SystemMaintenance.UnityResourceAbsolutePath);
            var result = await SystemMaintenance.Load_AllResource_To_ResourceListTable(directoryInfo, true);

            if (!result)
            {
#if UNITY_EDITOR
                Debug.LogError("중복된 파일명으로 인해 리소스리스트 테이블이 초기화되지 않았습니다.");
#endif
                ClearTable();
            }
                
#if RESOURCE_LIST_TABLE_INTO_GAMETABLE
            // 생성된 해당 테이블의 xml 파일의 에셋번들을 지정해준다.
            var assetBundleInfo = AssetImporter.GetAtPath(GetResourceListTableAtUnityResourcePath());
            assetBundleInfo.SetAssetBundleNameAndVariant(GameTableDataBundleName, null);
#endif
        }
#endif

        protected override string GetDefaultTableFileName()
        {
            return ResourceTool.ResourceListFileName;
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}