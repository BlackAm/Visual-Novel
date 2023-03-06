using System;
using System.IO;
using UnityEditor;

namespace k514
{
    public partial class SystemMaintenance
    {
        private static void InitBundleDirectory()
        {
            if (!Directory.Exists(BundleDirectoryBranch))
            {
                Directory.CreateDirectory(BundleDirectoryBranch);
            }
            if (!Directory.Exists(PatchPackageBranch))
            {
                Directory.CreateDirectory(PatchPackageBranch);
            }
            if (!Directory.Exists(VersionDirectoryBranch))
            {
                Directory.CreateDirectory(VersionDirectoryBranch);
            }
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// 리소스 관련 디렉터리를 검증하고 해당 디렉터리가 없다면 생성하며
        /// 그 외, 시스템 환경 멤버 들을 초기화해주는 메서드
        /// </summary>
        public static void InitSystemConfig()
        {
            // 리소스 경로 초기화
            if (SystemTool.TryGetEnumEnumerator<ResourceType>(SystemTool.GetEnumeratorType.ExceptNone, out var ResourceEnumerator))
            {
                foreach (var resourceType in ResourceEnumerator)
                {
                    var resourcePath = GetSystemResourcePath(AssetLoadType.FromUnityResource, resourceType, PathType.SystemGenerate_AbsolutePath);
                    if (!Directory.Exists(resourcePath))
                    {
                        Directory.CreateDirectory(resourcePath);
                    }

                    switch (resourceType)
                    {
                        case ResourceType.Table:
                            CreateTableDirectory();
                            break;
                        case ResourceType.Dependencies:
                            CreateDependenciesDirectory();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 테이블 디렉터리를 생성하는 메서드
        /// </summary>
        public static void CreateTableDirectory()
        {
            if (SystemTool.TryGetEnumEnumerator<TableTool.TableType>(SystemTool.GetEnumeratorType.GetAll, out var TableTypeEnumerator))
            {
                foreach (var tableType in TableTypeEnumerator)
                {
                    CreateTableDirectory(tableType);
                }
            }
        }
        
        /// <summary>
        /// 테이블 디렉터리를 생성하는 메서드
        /// </summary>
        public static void CreateTableDirectory(TableTool.TableType p_TableType)
        {
            if (SystemTool.TryGetEnumEnumerator<TableTool.TableFileType>(SystemTool.GetEnumeratorType.GetAll, out var TableFileTypeEnumerator))
            {
                foreach (var tableFileType in TableFileTypeEnumerator)
                {
                    switch (tableFileType)
                    {
                        // 에디터 테이블은 바이트코드르 생성하지 않고 읽지도 않음.
                        case TableTool.TableFileType.Bytes:
                            if (p_TableType == TableTool.TableType.EditorOnlyTable)
                            {
                            }
                            else
                            {
                                goto default;
                            }
                            break;
                        default:
                        case TableTool.TableFileType.Xml:
                        case TableTool.TableFileType.JSON:
                            var tryPath = TableTool.GetTablePath(AssetLoadType.FromUnityResource, PathType.SystemGenerate_AbsolutePath, p_TableType, tableFileType);
                            if (!Directory.Exists(tryPath))
                            {
                                Directory.CreateDirectory(tryPath);
                            }
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// 종속성 디렉터리를 생성하는 메서드
        /// </summary>
        public static void CreateDependenciesDirectory()
        {
            if (SystemTool.TryGetEnumEnumerator<DependencyResourceSubType>(SystemTool.GetEnumeratorType.ExceptNone, out var subResourceEnumerator))
            {
                foreach (var subResourceType in subResourceEnumerator)
                {
                    var tryType = GetSystemResourcePath(ResourceType.Dependencies, PathType.SystemGenerate_AbsolutePath) + subResourceType;
                    if (!Directory.Exists(tryType))
                    {
                        Directory.CreateDirectory(tryType);
                    }
                }
            }
        }
        
        private static void InitEditorDirectory()
        {
            if (!Directory.Exists(EditorOnlyAssetAbsolutePath))
            {
                Directory.CreateDirectory(EditorOnlyAssetAbsolutePath);
            } 
            if (!Directory.Exists(ThirdPartyAssetAbsolutePath))
            {
                Directory.CreateDirectory(ThirdPartyAssetAbsolutePath);
            }
            
            if (!Directory.Exists(UnusedAssetAbsolutePath))
            {
                Directory.CreateDirectory(UnusedAssetAbsolutePath);
            }
            
            if (SystemTool.TryGetEnumEnumerator<ResourceType>(SystemTool.GetEnumeratorType.ExceptNone, out var ResourceEnumerator))
            {
                foreach (var resourceType in ResourceEnumerator)
                {
                    switch (resourceType)
                    {
                        case ResourceType.None:
                        case ResourceType.AssetBundle:
                            continue;
                        default :
                        {
                            var tryPath = $"{UnusedAssetAbsolutePath}/{resourceType.ToString()}";
                            if (!Directory.Exists(tryPath))
                            {
                                Directory.CreateDirectory(tryPath);
                            }
                        }
                            break;
                    }
                }
            }
            
            AssetDatabase.Refresh(); 
        }
#endif
    }
}