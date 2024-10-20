using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    public partial class SystemMaintenance
    {
#if UNITY_EDITOR
        /// <summary>
        /// 에셋번들 전용 경로를 검색하여 마지막 버전을 계산하는 메서드
        /// </summary>
        /*public static int CalculateLatestBundleVersion()
        {
            var result = PatchTool.__Version_Null;
            var headerLength = VersionDirectoryNameHeader.Length;
            if (Directory.Exists(VersionDirectoryBranch))
            {
                var _versionDirInfo = new DirectoryInfo(VersionDirectoryBranch);
                var directorySet = _versionDirInfo.GetDirectories();
                foreach (var _directoryInfo in directorySet)
                {
                    if (_directoryInfo.Name.StartsWith(VersionDirectoryNameHeader))
                    {
                        var index = _directoryInfo.Name.Substring(headerLength, _directoryInfo.Name.Length - headerLength);
                        if (int.TryParse(index, out var parsedVersion))
                        {
                            result = Mathf.Max(result, parsedVersion);
                        }
                    }
                }
            }

            return result;
        }*/
        
        /// <summary>
        /// 현재 리소스 로드 타입을 참조하여 '로드 타입이 에셋번들 타입인 리소스 타입'을 [리소스 디렉터리로부터 백업 디렉터리로] 옮기거나
        /// 혹은 '로드 타입이 유니티 리소스인 리소스 타입'을 [백업 디렉터리로부터 리소스 디렉터리로] 옮기는 메서드.
        /// </summary>
        public static async UniTask ApplyBackupResourceDirectory()
        {
            if (SystemTool.TryGetEnumEnumerator<ResourceType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                var hasAssetBundleLoadFlag = false;
                foreach (var resourceType in o_Enumerator)
                {
                    if (!resourceType.IsFixedLoadTypeResource())
                    {
                        var targetResourceLoadType = GetResourceLoadType(resourceType);
                        var backUpPath = GetBackUpAbsolutePathWithResource(resourceType);
                        var resourcePath = 
                            GetSystemResourcePath(
                                AssetLoadType.FromUnityResource,
                                resourceType,
                                PathType.SystemGenerate_AbsolutePath
                            );
                        
                        switch (targetResourceLoadType)
                        {
                            case AssetLoadType.FromAssetBundle:
                                hasAssetBundleLoadFlag = true;
                                if (Directory.Exists(resourcePath))
                                {
                                    if (Directory.Exists(backUpPath))
                                    {
                                        Directory.Delete(backUpPath, true);
                                    }
                                    Debug.LogError(resourcePath + "\n" + backUpPath);
                                    Directory.Move(resourcePath, backUpPath);
                                }
                                break;
                            case AssetLoadType.FromUnityResource:
                                if (Directory.Exists(backUpPath))
                                {
                                    if (Directory.Exists(resourcePath))
                                    {
                                        Directory.Delete(resourcePath, true);
                                    }
                                    Directory.Move(backUpPath, resourcePath);
                                }
                                break;
                        }
                    }
                }
                AssetDatabase.Refresh();

                if (!hasAssetBundleLoadFlag)
                {
                    // 리소스 리스트를 재생성시킨다.
                    await (await ResourceListData.GetInstance()).CreateDefaultTable(true);
                }
            }
        }
     
        /// <summary>
        /// 특정 경로에 존재하는 파일을 전부 검증하여, ResourceListData 테이블의 레코드로 저장하는 메서드
        /// </summary>
        public static async UniTask<bool> Load_AllResource_To_ResourceListTable(DirectoryInfo p_DirectoryInfo, bool p_TrempolineRootInvoke)
        {
            // 최초 호출시에는 테이블 레코드 갯수가 0이다.
            var resourceListData = ResourceListData.GetInstanceUnSafe;
            var resourceListTable = resourceListData.GetTable();
            var resourceListTableName = resourceListData.GetTableFileName(TableTool.TableNameType.Alter, true);
   
            // 숨김 파일은 무시한다.
            var directoryEnumerator = p_DirectoryInfo.GetDirectories()
                .Where(_directoryInfo => !_directoryInfo.Name.StartsWith("."));
            
            // 최초 디렉터리로부터 너비 우선 탐색을 진행하며, 각 에셋의 정보를 테이블 레코드 인스턴스로 만들어준다.
            var rootFile = p_DirectoryInfo.GetFiles();
            foreach (var fileInfo in rootFile)
            {
                var tryName = fileInfo.Name;
                var fileFullName = fileInfo.FullName.TurnToSlash();
                if (!resourceListTable.ContainsKey(tryName))
                {
                    if (!IsBlockedExt(tryName))
                    {
                        await UniTask.SwitchToMainThread();
                        
                        var assetRelativePath = fileFullName.CutString(UnityResourceDirectory, false, true);
                        var assetBundleInfo = AssetImporter.GetAtPath(UnityResourceDirectory + assetRelativePath);
                        var resourceType = fileFullName.TryGetUnityResourcePath(true);
                        
                        if (resourceType.IsFixedLoadTypeResource())
                        {
                            assetBundleInfo.SetAssetBundleNameAndVariant(string.Empty, null);
                        }
                        else
                        {
                            var isTerminal = !directoryEnumerator.Any();
                            switch (resourceType)
                            {
                                case ResourceType.Misc:
                                    assetBundleInfo.SetAssetBundleNameAndVariant("Misc", null);
                                    break;
                                case ResourceType.Table:
                                    if (assetRelativePath.IsSystemTablePath())
                                    {
                                        assetBundleInfo.SetAssetBundleNameAndVariant(string.Empty, null);
                                    }
                                    else
                                    {
                                        goto default;
                                    }
                                    break;
                                default:
                                    var defaultBundlePath = assetRelativePath.CutString("/", true, false);
                                    
                                    if (isTerminal)
                                    {
                                        
                                        assetBundleInfo.SetAssetBundleNameAndVariant($"{defaultBundlePath}", null);
                                    }
                                    else
                                    {
                                        string firstBundlePath = $"{defaultBundlePath.CutString("/", true, false)}";
                                        string secondBundlePath = $"{defaultBundlePath.CutString("/", false, false)}";
                                      //  if (assetRelativePath.Contains(".unity"))
                                        {
                                            
                                       //     secondBundlePath  = $"{secondBundlePath}/_{Almond.Util.Utils.GetFileNameWithoutExtention(assetRelativePath)}";
                                        }

                                        assetBundleInfo.SetAssetBundleNameAndVariant($"{firstBundlePath}/_{secondBundlePath}", null);
                                    }
                                    break;
                            }
                        }
                        var spawnedRecord = await resourceListData.AddRecord(tryName, fileFullName, assetBundleInfo == null ? string.Empty : assetBundleInfo.assetBundleName);
                        // 에셋 번들 리스트를 업데이트한다.
                        AddAssetBundleNameRecord(spawnedRecord);
                    }
                }
                else
                {
                    // 만약 시스템 리소스 리스트 테이블 파일 이외에 이미 동명의 파일이 존재했던 경우에는 에러 메시지를 출력한다.
                    var isFileNameEqualResourceListTable = tryName == resourceListTableName;
                    var isFilePathEqualResourceListTable = resourceListData.GetTableFileFullPath
                                                           (
                                                               AssetLoadType.FromUnityResource, 
                                                               PathType.SystemGenerate_AbsolutePath,
                                                               TableTool.TableNameType.Alter, 
                                                               true
                                                           )
                                                           .TrimPathSymbol() == fileFullName;

                    if (isFileNameEqualResourceListTable && isFilePathEqualResourceListTable)
                    {
                    }
                    else
                    {
                        Debug.LogError($"파일명 [{tryName}]이 중복되었습니다. 파일명이 중복되지 않도록 재지정해주세요.\n * 현재 테이블에 존재하는 파일 패스\n[{resourceListTable[tryName].GetResourceFullPath()}]\n * 중복된 파일 패스\n[{fileFullName}]\n\n");
                        // return false;
                    }
                }
            }

            // 재귀 함수 최초 진입시에만, 리소스 리스트 자체를 넣어준다.
            if (p_TrempolineRootInvoke)
            {
                var fileFullName = resourceListData.GetTableFileFullPath(AssetLoadType.FromUnityResource, PathType.SystemGenerate_AbsolutePath, TableTool.TableNameType.Alter, true).TrimPathSymbol();
                var tryRecord = 
                    await resourceListData
                        .AddRecord
                            (
                                resourceListTableName, fileFullName, 
                                resourceListData.TableType == TableTool.TableType.SystemTable 
                                    ? string.Empty 
                                    : ResourceListData.GameTableModeResourceListTableBundleName
                            );

                // 에셋 번들 리스트를 업데이트한다.
                AddAssetBundleNameRecord(tryRecord);

                // 유니티에서 자동으로 생성하는 번들, 다른 번들에 대한 메타데이터 역할
                AddDefaultBundleName();
            }

            // 서브 폴더에 대해서도 작업 수행
            foreach (var subDirectory in directoryEnumerator)
            {
                var result = await Load_AllResource_To_ResourceListTable(subDirectory, false);
                if (!result)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 지정한 에셋 리스트 레코드의 에셋번들 이름을 에셋 번들 리스트에 업데이트한다.
        /// </summary>
        private static void AddAssetBundleNameRecord(ResourceListData.ResourceListRecord p_TargetRecord)
        {
            var targetBundleNameList = ResourceListData.GetInstanceUnSafe.AssetBundleNameList;
            var resultBundleName = p_TargetRecord.GetAssetBundleName();
            if (resultBundleName != string.Empty)
            {
                if (!targetBundleNameList.Contains(resultBundleName))
                {
                    targetBundleNameList.Add(resultBundleName);
                }
            }
        }

        /// <summary>
        /// 기본 에셋 번들을 에셋 번들 리스트에 업데이트한다.
        /// </summary>
        private static void AddDefaultBundleName()
        {
            var targetBundleNameList = ResourceListData.GetInstanceUnSafe.AssetBundleNameList;
            if (!targetBundleNameList.Contains(DefaultBundleName))
            {
                targetBundleNameList.Add(DefaultBundleName);
            }
        }
#endif
    }
}