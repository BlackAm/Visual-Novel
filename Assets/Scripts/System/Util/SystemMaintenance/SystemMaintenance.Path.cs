using UnityEngine;

namespace BlackAm
{
    public partial class SystemMaintenance
    {
        /// <summary>
        /// 해당 프로젝트의 절대경로를 리턴하는 메서드
        /// </summary>
        public static string ProjectAbsolutePath => $"{Application.dataPath.CutLastSlashGetForward()}/";

        /// <summary>
        /// 프로젝트 폴더 기준으로 유니티 리소스 디렉터리 헤더
        /// </summary>
        public const string UnityResourceDirectory = "Assets/Resources/";
        
        /// <summary>
        /// 프로젝트 폴더 기준으로 에셋번들 디렉터리 헤더
        /// </summary>
        public const string AssetBundleBaseDirectory = "BundleResources/";
        
        /// <summary>
        /// 프로젝트 폴더 기준으로 백업 디렉터리 헤더
        /// </summary>
        public const string BackUpDirectory = "BackUp/";
       
        /// <summary>
        /// 프로젝트 폴더 기준으로 개발 전용 리소스 디렉터리 헤더
        /// </summary>
        public const string EditorOnlyResourceDirectory = "Assets/xEditorOnlyResources/";
        
        /// <summary>
        /// Asset/Resources 에 포함되지 않는 프로젝트 에셋 디렉터리 헤더
        /// </summary>
        public const string UnusedDirectory = "Assets/xUnusedResources/";
        
        /// <summary>
        /// 외부 패키지 디렉터리 헤더
        /// </summary>
        public const string ThirdPartyDirectory = "Assets/3rd-Party/";
        
        /// <summary>
        /// 프로젝트 세이브 데이터 파일 저장 디렉터리
        /// </summary>
        public static readonly string SaveDataFileDirectory = $"{Application.persistentDataPath}/SaveData/File/";
        
        /// <summary>
        /// 프로젝트 세이브 데이터 이미지 저장 디렉터리
        /// </summary>
        public static readonly string SaveDataImageDirectory = $"{Application.persistentDataPath}/SaveData/Image/";

        /// <summary>
        ///  읽은 대화 키 저장 디렉터리
        /// </summary>
        public static readonly string SaveReadDialogueDirectory = $"{Application.persistentDataPath}/SaveData/ReadDialogue/";

        /// <summary>
        /// 갤러리 저장 디렉터리
        /// </summary>
        public static readonly string SaveGalleryDirectory = $"{Application.persistentDataPath}/SaveData/Gallery/";

        /// <summary>
        /// 읽은 대화 키 저장 파일
        /// </summary>
        public static readonly string SaveReadDialogueFile = $"{SaveReadDialogueDirectory}File";

        /// <summary>
        /// 갤러리 저장 파일
        /// </summary>
        public static readonly string SaveGalleryFile = $"{SaveGalleryDirectory}File";
        
        /// <summary>
        /// 해당 프로젝트의 유니티 리소스 폴더 디렉터리
        /// </summary>
        public static readonly string UnityResourceAbsolutePath = $"{ProjectAbsolutePath}{UnityResourceDirectory}";
        
        /// <summary>
        /// 해당 프로젝트의 에셋번들 리소스 폴더 디렉터리
        /// </summary>
        public static readonly string AssetBundleAbsolutePath = $"{ProjectAbsolutePath}{AssetBundleBaseDirectory}";
        
        /// <summary>
        /// 해당 프로젝트의 백업 리소스 폴더 디렉터리
        /// </summary>
        public static readonly string BackUpAbsolutePath = $"{ProjectAbsolutePath}{BackUpDirectory}";
        
        /// <summary>
        /// 해당 프로젝트의 에디터 전용 리소스 폴더 디렉터리
        /// </summary>
        public static readonly string EditorOnlyAssetAbsolutePath = $"{ProjectAbsolutePath}{EditorOnlyResourceDirectory}";
        
        /// <summary>
        /// 해당 프로젝트의 리소스 폴더에 포함되지 않는 에셋 용 디렉터리
        /// </summary>
        public static readonly string UnusedAssetAbsolutePath = $"{ProjectAbsolutePath}{UnusedDirectory}";
                
        /// <summary>
        /// 해당 프로젝트의 외부 패키지 에셋 디렉터리
        /// </summary>
        public static readonly string ThirdPartyAssetAbsolutePath = $"{ProjectAbsolutePath}{ThirdPartyDirectory}";
        
        /// <summary>
        /// 에셋번들 번들 디렉터리 헤더
        /// </summary>
        public const string BundleMetaDirectoryBranchHeader = "Meta/";
        
        /// <summary>
        /// 에셋번들 번들 디렉터리 헤더
        /// </summary>
        public const string BundleDirectoryBranchHeader = "Bundle/";
             
        /// <summary>
        /// 에셋번들 버전관리 폴더 헤더
        /// </summary>
        public const string VersionDirectoryNameHeader = "415KVersion";
        
        /// <summary>
        /// 개발모드에 의해 생성된 번들 디렉터리 헤더
        /// </summary>
        public const string BundleDevDirectoryBranchHeader = "Bundle_dev/";
        
        /// <summary>
        /// 에셋번들 버전 디렉터리 헤더
        /// </summary>
        public const string VersionDirectoryBranchHeader = "Version/";
        
        /// <summary>
        /// 이전버전의 번들 디렉터리 헤더
        /// </summary>
        public const string BundlePrevDirectoryBranchHeader = "Bundle_prevVersion/";
                
        /// <summary>
        /// 임시 디렉터리 헤더
        /// </summary>
        public const string BundleTmpBranchHeader = "Tmp/";
            
        /// <summary>
        /// 결과 디렉터리 헤더
        /// </summary>
        public const string BundleResultBranchHeader = "Result/";
        
        /// <summary>
        /// 패치 패키지 디렉터리 헤더
        /// </summary>
        public const string PatchPackageBranchHeader = "Patch/";
                     
        /// <summary>
        /// 풀 패치 패키지 생성 디렉터리 헤더
        /// </summary>
        public const string FullPatchPackageStorageBranchHeader = "FP/";
                
        /// <summary>
        /// 부분 패치 패키지 생성 디렉터리 헤더
        /// </summary>
        public const string PartialPatchPackageStorageBranchHeader = "PP/";
        
        /// <summary>
        /// 다운로드 디렉터리 헤더
        /// </summary>
        public const string DownloadBranchHeader = "Download/";

        /// <summary>
        /// 기본 번들(다른 번들의 메타데이터를 가진 번들)의 이름은 어째선지 번들이 생성된 폴더명이 된다.
        /// </summary>
        public static readonly string DefaultBundleName = BundleDevDirectoryBranchHeader.CutLastSlashGetForward();
       
        /// <summary>
        /// 번들 관련 파일의 루트 디렉터리
        /// </summary>
        public static readonly string BundleRootDirectory = GetSystemResourcePath(AssetLoadType.FromAssetBundle, ResourceType.None, PathType.SystemGenerate_AbsolutePath);
        
        /// <summary>
        /// 번들 관련 파일의 루트 디렉터리
        /// </summary>
        public static readonly string BundleVersionIndexBranch = BundleRootDirectory + VersionDirectoryBranchHeader;
        
        /// <summary>
        /// 에셋번들 관련 파일의 루트 디렉터리
        /// </summary>
        public static readonly string AssetBundleRootDirectory = GetSystemResourcePath(AssetLoadType.FromAssetBundle, ResourceType.AssetBundle, PathType.SystemGenerate_AbsolutePath);
        
        /// <summary>
        /// 에셋번들 번들 디렉터리
        /// </summary>
        public static readonly string BundleDirectoryBranch = AssetBundleRootDirectory + BundleDirectoryBranchHeader;
        
        /// <summary>
        /// 개발모드에 의해 생성된 번들 디렉터리
        /// </summary>
        public static readonly string BundleDirectoryDevBranch = AssetBundleRootDirectory + BundleDevDirectoryBranchHeader;

        /// <summary>
        /// 에셋번들 버전 디렉터리
        /// </summary>
        public static readonly string VersionDirectoryBranch = AssetBundleRootDirectory + VersionDirectoryBranchHeader;
        
        /// <summary>
        /// 이전버전의 번들 디렉터리
        /// </summary>
        public static readonly string BundlePrevDirectoryBranch = AssetBundleRootDirectory + BundlePrevDirectoryBranchHeader;
        
        /// <summary>
        /// 해당 프로젝트의 패치 패키지 폴더 디렉터리
        /// </summary>
        public static readonly string PatchPackageBranch = AssetBundleRootDirectory + PatchPackageBranchHeader;

        /// <summary>
        /// 현재 시스템이 실제 플랫폼을 상정한 로직을 수행중인 경우
        /// 그에 맞는 번들 패스를 리턴하는 메서드
        /// </summary>
        public static string GetBundleFullPathOnPlayPlatform()
        {
            return IsPlayOnTargetPlatform() ? BundleDirectoryBranch : BundleDirectoryDevBranch;
        }

        /// <summary>
        /// 패치 파일 디렉터리를 리턴하는 메서드
        /// </summary>
        /*public static string GetPatchFilePathHeader(PathType p_PathType, PatchTool.PatchMode p_Mode)
        {
            switch (p_PathType)
            {
                default:
                case PathType.SystemGenerate_AbsolutePath:
                    switch (p_Mode)
                    {
                        case PatchTool.PatchMode.Partial:
                             return $"{PatchPackageBranch}{PartialPatchPackageStorageBranchHeader}";
                        default:
                        case PatchTool.PatchMode.Full:
                            return $"{PatchPackageBranch}{FullPatchPackageStorageBranchHeader}";
                    }
                    break;
                case PathType.SystemGenerate_RelativePath:
                    switch (p_Mode)
                    {
                        case PatchTool.PatchMode.Partial:
                            return $"{PatchPackageBranchHeader}{PartialPatchPackageStorageBranchHeader}";
                        default:
                        case PatchTool.PatchMode.Full:
                            return $"{PatchPackageBranchHeader}{FullPatchPackageStorageBranchHeader}";
                    }
                    break;
            }
        }*/
        
        /// <summary>
        /// 패치 파일명을 리턴하는 메서드
        /// </summary>
        /*public static string GetPatchFileName(PatchTool.PatchMode p_Mode, int p_ToVersion)
        {
            switch (p_Mode)
            {
                case PatchTool.PatchMode.Partial:
                    return $"PartialPatch_{p_ToVersion}{PatchFileExt}";
                default:
                case PatchTool.PatchMode.Full:
                    return $"FullPatch_{p_ToVersion}{PatchFileExt}";
            }
        }*/
        
        /// <summary>
        /// 패치 파일 디렉터리를 리턴하는 메서드
        /// </summary>
        /*public static string GetPatchFileBranch(PatchTool.PatchMode p_Mode, int p_ToVersion)
        {
            switch (p_Mode)
            {
                case PatchTool.PatchMode.Partial:
                    return $"PartialPatch_{p_ToVersion}/";
                default:
                case PatchTool.PatchMode.Full:
                    return $"FullPatch_{p_ToVersion}/";
            }
        }*/
        
        /// <summary>
        /// 풀패치 파일 풀패스를 리턴하는 메서드
        /// </summary>
        /*public static string GetFullPatchFilePath(PathType p_PathType, PatchTool.PatchMode p_Mode, int p_ToVersion)
        {
            return $"{GetPatchFilePathHeader(p_PathType, p_Mode)}{GetPatchFileName(p_Mode, p_ToVersion)}";
        }*/
        
        /// <summary>
        /// 풀패치 파일 브랜치를 리턴하는 메서드
        /// </summary>
        /*public static string GetFullPatchFileBranch(PathType p_PathType, PatchTool.PatchMode p_Mode, int p_ToVersion)
        {
            return $"{GetPatchFilePathHeader(p_PathType, p_Mode)}{GetPatchFileBranch(p_Mode, p_ToVersion)}";
        }*/

        /// <summary>
        /// 패치 파일이 다운로드되는 디렉터리를 리턴하는 메서드
        /// </summary>
        public static string GetPatchDownloadDirectory(PathType p_PathType)
        {
            switch (p_PathType)
            {
                case PathType.SystemGenerate_AbsolutePath:
                    return $"{PatchPackageBranch}{DownloadBranchHeader}";
                case PathType.SystemGenerate_RelativePath:
                    return DownloadBranchHeader;
            }

            return null;
        }
        
        /// <summary>
        /// 다운로드된 풀패치 파일 경로를 리턴하는 메서드
        /// </summary>
        /*public static string GetDownloadedPatchFilePath(PathType p_PathType, PatchTool.PatchMode p_Mode, int p_ToVersion)
        {
            return $"{GetPatchDownloadDirectory(p_PathType)}{GetPatchFileName(p_Mode, p_ToVersion)}";
        }*/

        /// <summary>
        /// 지정한 버전 디렉터리의 절대경로를 리턴하는 메서드
        /// </summary>
        public static string GetVersionDirectoryAbsolutePath(int p_Version)
        {
            return $"{VersionDirectoryBranch}{VersionDirectoryNameHeader}{p_Version}/";
        }

        public static string GetPlatformPathWithResourceType(this string p_PlatformPath, ResourceType p_ResourceType)
        {
            return  p_ResourceType == ResourceType.None ? $"{p_PlatformPath}" : $"{p_PlatformPath}{p_ResourceType.ToString()}/";
        }

        public static string GetDependencyResourcePathBranch(DependencyResourceSubType p_ResourceSubType)
        {
            return $"{ResourceType.Dependencies}/{p_ResourceSubType}/";
        }
        
        /// <summary>
        /// 리소스 폴더 백업 경로를 리턴하는 메서드
        /// </summary>
        public static string GetBackUpAbsolutePathWithResource(ResourceType p_ResourceType)
        {
            return $"{BackUpAbsolutePath}{p_ResourceType}/";
        }

        /// <summary>
        /// 지정한 리소스 타입의 로드 방식 타입을 리턴한다.
        /// 해당 타입은 SystemConfigData 테이블을 참조하지만,
        /// 에셋 번들과 시스템 테이블 리소스 타입은 참조하지 않고 무조건 유니티 로드 타입을 리턴한다.
        /// </summary>
        public static AssetLoadType GetResourceLoadType(this ResourceType p_ResourceType)
        {
            if (p_ResourceType.IsFixedLoadTypeResource())
            {
                return AssetLoadType.FromUnityResource;
            }
            else
            {
                return SystemConfigData.GetInstanceUnSafe.GetTableData(p_ResourceType).ResourceLoadType;
            }
        }

        /// <summary>
        /// 특정한 리소스 타입의 경로를 리턴하는 메서드
        /// </summary>
        public static string GetSystemResourcePath(ResourceType p_ResourceType, PathType p_PathType)
        {
            // 해당 리소스 타입의 로드 타입을 불러온다.
            var _LoadType = GetResourceLoadType(p_ResourceType);
            return GetSystemResourcePath(_LoadType, p_ResourceType, p_PathType);
        }
        
        /// <summary>
        /// 특정한 리소스 타입의 경로를 리턴하는 메서드
        /// 리소스 로드 타입을 수동으로 지정할 수 있다.
        /// </summary>
        public static string GetSystemResourcePath(AssetLoadType p_LoadType, ResourceType p_ResourceType, PathType p_PathType)
        {
            var _isBundle = false;
            switch (p_LoadType)
            {
                case AssetLoadType.FromAssetBundle :
                    _isBundle = true;
                    break;
                case AssetLoadType.FromUnityResource :
                    switch (p_ResourceType)
                    {
                        case ResourceType.AssetBundle :
                            _isBundle = true;
                            break;
                    }
                    break;
            }
            
            var _PlatformBranch = string.Empty;
            switch (p_PathType)
            {
                case PathType.SystemGenerate_RelativePath :
                    break;
                case PathType.SystemGenerate_AbsolutePath :
                    switch (Application.platform)
                    {
                        // 빌드 플랫폼
                        case RuntimePlatform.Android :
                        case RuntimePlatform.Switch :
                        case RuntimePlatform.WindowsPlayer :
                            _PlatformBranch = _isBundle ? $"{Application.persistentDataPath}/{AssetBundleBaseDirectory}" : $"{Application.persistentDataPath}/";
                            break;
                        // 개발 플랫폼
                        case RuntimePlatform.WindowsEditor :
                            _PlatformBranch = _isBundle ? AssetBundleAbsolutePath : UnityResourceAbsolutePath;
                            break;
                    }
                    break;
            }
 
            return _PlatformBranch.GetPlatformPathWithResourceType(p_ResourceType);
        }
    }
}