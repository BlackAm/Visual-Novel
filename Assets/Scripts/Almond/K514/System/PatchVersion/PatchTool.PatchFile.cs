using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /*public partial class PatchTool
    {
        #region <Methods>

        /// <summary>
        /// 지정한 버전 구간이 풀 패치인지 혹은 부분패치가 적합한지 평가하는 메서드
        /// </summary>
        public static PatchMode GetPatchMode(int p_From, int p_To)
        {
            // 현재 버전이 이미 지정한 버전인 경우
            if (p_From == p_To)
            {
                return PatchMode.Same;
            }
            // 지정한 버전이 다운그레이드인 경우, 즉 해당 버전의 풀버전을 덮어써야하는 경우
            else if (p_From > p_To)
            {
                // 타겟 버전이 null인 경우
                if (p_To <= __Version_Null)
                {
                    return PatchMode.None;
                }
                else
                {
                    return PatchMode.Full;
                }
            }
            // 지정한 버전이 업그레이드인 경우
            else
            {
                // 현재 버전이 Null인 경우, 즉 해당 버전의 풀버전을 덮어써야하는 경우
                if (p_From <= __Version_Null)
                {
                    return PatchMode.Full;
                }
                // 현재 버전 전용의 부분 패치 파일을 다운받아야 하는 경우
                else if(p_From >= __Version_Lower_Bound)
                {
                    return PatchMode.Partial;
                }
                // 그 외의 버전 즉, (Null 버전, __Version_Lower_Bound)의 경우에는 유효하지 않은 버전이기에 무시한다.
                // 혹은 특수한 목적을 가지는 클라이언트로 활용한다.
                else
                {
                    return PatchMode.None;
                }
            }
        }
        
        /// <summary>
        /// 지정한 버전 사이의 부분 패치 파일, 혹은 풀 패치 파일이 존재하는지 검증하는 메서드
        /// </summary>
        public static string GetLocalPatchPackageName(int p_From, int p_To)
        {
            return SystemMaintenance.GetFullPatchFilePath(PathType.SystemGenerate_AbsolutePath, GetPatchMode(p_From, p_To), p_To);
        }
        
        /// <summary>
        /// 지정한 버전 사이의 부분 패치 파일, 혹은 풀 패치 파일이 존재하는지 검증하는 메서드
        /// </summary>
        public static bool HasLocalPatchPackage(int p_From, int p_To)
        {
            return File.Exists(GetLocalPatchPackageName(p_From, p_To));
        }

        public static async UniTask PatchVersion(PatchPreset p_PatchPreset, bool p_GetPatchFromServerPatch, AsyncTaskEventHandler p_EventHandler = default)
        {
            // 배포 모드인 경우, 서버를 경유해서 다운로드 및 패치를 실행한다.
            // 배포 모드가 아닌 경우, PatchPackageBuilder를 통해 생성된 로컬저장소의 패치파일을 통해 패치를 수행한다.
            if (p_GetPatchFromServerPatch)
            {
                var patchFileSavePath = SystemMaintenance.GetPatchDownloadDirectory(PathType.SystemGenerate_AbsolutePath);
#if UNITY_EDITOR
                Debug.Log($"[Init Patch] : 패치 다운로드 디렉터리 {patchFileSavePath}");
#endif
                if (Directory.Exists(patchFileSavePath))
                {
                    Directory.Delete(patchFileSavePath, true);
                }
            }

            switch (p_PatchPreset.PatchMode)
            {
                case PatchMode.None:
                {
#if UNITY_EDITOR
                    Debug.Log("* [Init Patch] : 패치 사항을 전부 제거합니다.");
                    Debug.Log(p_PatchPreset);
#endif
                    var patchTargetDirectory = SystemMaintenance.BundleDirectoryBranch;
                    if (Directory.Exists(patchTargetDirectory))
                    {
                        Directory.Delete(patchTargetDirectory, true);
                    }

                    InitClientVersion();
                    break;
                }
                case PatchMode.Same:
                {
#if UNITY_EDITOR
                    Debug.Log("* [Notice] : 동일한 버전입니다.");
                    Debug.Log(p_PatchPreset);
#endif
                    break;
                }
                case PatchMode.Partial:
                {
                    var hasPatchFile = HasLocalPatchPackage(p_PatchPreset.CurrentVersion, p_PatchPreset.TargetVersion);
                    if (hasPatchFile)
                    {
#if UNITY_EDITOR
                        Debug.Log("* [Partial Patch] : 패치 파일 확인");
                        Debug.Log(p_PatchPreset);
#endif
                        try
                        {
#if UNITY_EDITOR
                            Debug.LogError($"* [Partial Patch] : 미구현이므로, 풀패치를 진행합니다.");
#endif
                            goto case PatchMode.Full;
                            
                            var patchTargetDirectory = SystemMaintenance.BundleDirectoryBranch;
                            if (Directory.Exists(patchTargetDirectory))
                            {
                                var currentVersion = p_PatchPreset.CurrentVersion;
                                var targetVersion = p_PatchPreset.TargetVersion;
                                var localBundleHash = GetBundleHash(patchTargetDirectory);

                                // 로컬버전의 에셋번들이 무결한 경우
                                if (VersionIndexTableData.GetInstanceUnSafe.IsValidBundle(currentVersion, localBundleHash))
                                {
                                    if (p_GetPatchFromServerPatch)
                                    {
//                                        var patchFileSavePath = SystemMaintenance.GetPatchDownloadDirectory();
//                                        for (var i = currentVersion; i < targetVersion; i++)
//                                        {
//                                            var tryVersion = i + 1;
//                                            var tryPatchFileName = SystemMaintenance.GetPartialPatchBridgeFileName(tryVersion);
//                                            var tryServerNodeURI = NetworkNodeTableData.GetInstanceUnSafe[NetworkTool.NetworkNodeType.TestTomcatMainFormat].URI;
//                                            tryServerNodeURI.SetURIFormat0(tryPatchFileName);
//
//                                            var (valid, handler) = await DownloadManager.GetInstance.RunHandler((tryServerNodeURI, patchFileSavePath), 0f);
//                                            if (valid)
//                                            {
//                                    
//                                            }
//                                        }
                                    }
                                }
                                else
                                {
#if UNITY_EDITOR
                                    Debug.LogError($"* [Partial Patch] : 현재 로컬저장소의 번들 파일이 유효하지 않으므로, 풀패치를 진행합니다.");
#endif
                                    goto case PatchMode.Full;
                                }
                            }
                            else
                            {
#if UNITY_EDITOR
                                Debug.LogError($"* [Partial Patch] : 현재 로컬저장소의 번들 파일이 존재하지 않으므로, 풀패치를 진행합니다.");
#endif
                                goto case PatchMode.Full;
                            }
                        }
#if UNITY_EDITOR
                        catch (Exception e)
                        {
                            Debug.LogError($"* [Partial Patch] : 패치 실패\n{e.Message}");
                        }
#else
                        catch
                        {
                        }
#endif
                    }
                    break;
                }
                case PatchMode.Full:
                {
                    try
                    {
                        var targetVersion = p_PatchPreset.TargetVersion;
                        var pathToDeCompress = SystemMaintenance.BundleDirectoryBranch;
                        if (Directory.Exists(pathToDeCompress))
                        {
                            Directory.Delete(pathToDeCompress, true);
                        }
                        Directory.CreateDirectory(pathToDeCompress);
                            
                        if (p_GetPatchFromServerPatch)
                        {
                            var pathToCompress = SystemMaintenance.GetDownloadedPatchFilePath(PathType.SystemGenerate_AbsolutePath, PatchMode.Full, targetVersion);
                            var patchFileSavePath = SystemMaintenance.GetPatchDownloadDirectory(PathType.SystemGenerate_AbsolutePath);
                            var tryPatchFileName = SystemMaintenance.GetFullPatchFilePath(PathType.SystemGenerate_AbsolutePath, PatchMode.Full, targetVersion);
                            var tryServerNodeURI = NetworkNodeTableData.GetInstanceUnSafe[NetworkTool.PatchNode].URI;
                            tryServerNodeURI.SetURIFormat0(tryPatchFileName);
#if UNITY_EDITOR
                            Debug.Log($"* [Full Patch] : 네트워크로부터 파일을 읽습니다. {tryServerNodeURI.URI} => {patchFileSavePath}");
                            Debug.Log(p_PatchPreset);
#endif
                            var result = await DownloadManager.GetInstance.RunHandler(p_EventHandler, (tryServerNodeURI, patchFileSavePath), 0f);
                            if (result)
                            {
#if UNITY_EDITOR
                                Debug.Log($"* [Full Patch] : 패치파일 압축 해제 중입니다. {pathToCompress} => {pathToDeCompress}");
#endif
                                await DecodeData.DecompressDirectoryAsync(ExportDataTool.CompressType.Gzip, pathToCompress, pathToDeCompress);
                            }
                            else
                            {
                                throw new Exception("[Full Patch Fail] : 네트워크로부터 풀패치파일을 다운로드하는데 실패했습니다.");
                            }
                        }
                        else
                        {
                            var pathToCompress = SystemMaintenance.GetFullPatchFilePath(PathType.SystemGenerate_AbsolutePath, PatchMode.Full, targetVersion);
#if UNITY_EDITOR
                            Debug.Log($"* [Full Patch] : 로컬 저장소로부터 파일을 압축해제　합니다. {pathToCompress} => {pathToDeCompress}");
                            Debug.Log(p_PatchPreset);
#endif
                            await DecodeData.DecompressDirectoryAsync(ExportDataTool.CompressType.Gzip, pathToCompress, pathToDeCompress);
                        }

                        UpdateClientVersion(p_PatchPreset.TargetVersion);
#if UNITY_EDITOR
                        Debug.Log($"* [Full Patch] : 패치에 성공했습니다.");
#endif
                    }
                    catch
                    {
                        throw;
                    }
                    break;
            }
        }

//            
//#if UNITY_EDITOR
//            if (CustomDebug.PrintPatchFlag)
//            {
//                Debug.Log($"패치 파일 디렉터리 : [{patchPackageFilePath}]");
//            }
//#endif
//            if (File.Exists(patchPackageFilePath))
//            {
//#if UNITY_EDITOR
//                if (CustomDebug.PrintPatchFlag)
//                {
//                    Debug.Log($"패치 파일을 찾았습니다. 압축을 해제합니다.");
//                }
//#endif
//                var patchTargetPath = SystemMaintenance.BundleDirectoryBranch;
//                if (fullPatchFlag && Directory.Exists(patchTargetPath))
//                {
//                    Directory.Delete(patchTargetPath, true);
//                }
//                Directory.CreateDirectory(patchTargetPath);
//
//                var decompressTargetPath = $"{patchPackageFilePath}{SystemMaintenance.BundleTmpBranchHeader}";
//                DecodeFile.DecompressDirectory_GZipStream(patchPackageFilePath, decompressTargetPath);
//
//                // 패치 리스트 테이블을 읽고, 해당 버전에 패치해야할 번들을 적용시켜준다.
//                ApplyPatch(decompressTargetPath, patchTargetPath);
//
//                // 파일 패치과정에서 남겨진 텅빈 디렉터리를 제거해준다.
//                SystemTool.DeleteEmptyDirectoryAt(patchTargetPath);
//                
//                // 패치파일을 압축해제했던 임시 디렉터리를 제거해준다.
//                if (Directory.Exists(decompressTargetPath))
//                {
//                    Directory.Delete(decompressTargetPath, true);
//                }
//                
//                // 클라이언트 버전을 업데이트한다.
//                PlayerPrefs.SetInt(__PLAYER_PREFERENCE_CLIENT_VERSION, p_TargetVersion);
//            }
//            else
//            {
//                OnPatchPackageFindFailed();
//            }
        }
        
        /// <summary>
        /// 번들 디렉터리에 있는 파일들이 리소스 리스트와 동일한 파일들로만 구성되어있는지 검증하는 메서드
        /// </summary>
        public static async UniTask<bool> CompareBundleList()
        {
            // 리소스 리스트를 다시 로드한다.
            await ResourceListData.GetInstanceUnSafe.ReloadTable(null);
            
            var assetBundleList = ResourceListData.GetInstanceUnSafe.AssetBundleNameList;
            var bundleBranch = SystemMaintenance.BundleDirectoryBranch;
            foreach (var assetBundleName in assetBundleList)
            {
                var targetBundleFileFullName = bundleBranch + assetBundleName;
                if (!File.Exists(targetBundleFileFullName))
                {
                    return false;
                }
                var targetBundleManifestFileFullName = targetBundleFileFullName + SystemMaintenance.BundleManifestFileExt;
                if (!File.Exists(targetBundleManifestFileFullName))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }*/
}