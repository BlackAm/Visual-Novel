using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
namespace k514
{
    /*public partial class Patcher
    {
        #region <Consts>

        /// <summary>
        /// 동시에 다운받을 에셋 숫자
        /// </summary>
        private const int __Download_Width = 15;

        #endregion

        #region <Callbacks>
        protected override void OnEntryPhaseLoop()
        {
        }
        protected override void _OnTerminatePhaseLoop()
        {
            SystemBoot.GetInstance.OnPatchSuccess();
        }
        #endregion
        #region <Methods>
        private async UniTask TryGetVersionIndex(IAsyncTaskRequest p_AsyncTaskRequest)
        {
            if (SystemMaintenance.IsPlayOnTargetPlatform())
            {
#if UNITY_EDITOR
                Debug.LogWarning("네트워크 연결 설정이 초기화를 시작합니다.");
#endif
                await VersionIndexTableData.GetInstanceUnSafe.TryLoadVersionIndexFromServer();
            }
            else
            {
                await VersionIndexTableData.GetInstanceUnSafe.TryLoadVersionIndexFromLocal();
            }
            var (valid, versionRecord) = VersionIndexTableData.GetInstanceUnSafe.GetLatestRecord();
            if (valid)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"네트워크 연결 설정이 완료되었습니다. 현재 서버 버전 : {versionRecord.APKVersion}\n{versionRecord.BundleVersion}\n{versionRecord.VersionDriveDate}\n\n{versionRecord.VersionDescription}");
#endif
                await UniTask.SwitchToMainThread();
                if (Application.version != versionRecord.APKVersion)
                {
                    var uri = $"{(await NetworkNodeTableData.GetInstance())[NetworkTool.PatchNode].URI.URI.CutLastSlashGetForward()}/apk/Lamiere2021a.apk";
                    Application.OpenURL(uri);
                }
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"네트워크 연결 설정이 완료되었으나, 서버에서 유효한 테이블을 찾을 수 없었습니다.");
            }
#endif
        }
        private async UniTask CompareVersion(IAsyncTaskRequest p_AsyncTaskRequest)
        {
            if (p_AsyncTaskRequest is AsyncPatchTaskRequestHandler c_Handler)
            {
                var clientVersion = PatchTool.GetClientVersion();
                var (valid, serverLatestVersionRecord) = VersionIndexTableData.GetInstanceUnSafe.GetLatestRecord();
                if (valid)
                {
                    var serverLatestVersion = serverLatestVersionRecord.BundleVersion;
#if UNITY_EDITOR
                    Debug.LogWarning($"버전을 비교합니다. c:{clientVersion} <-> s:{serverLatestVersion}");
                    Debug.LogWarning($"비교할 버전 디렉터리 : {SystemMaintenance.GetBundleFullPathOnPlayPlatform()}");
#endif
                    var result = new AsyncPatchTaskRequestResult();
                    result.SetVersion(clientVersion, serverLatestVersion);
                    c_Handler.SetResult(result);
                    // TODO<k514> : 테이블의 해시값과 현재 번들의 해시값을 비교하는 로직이 추가되어야함. 현재는 단순히 버전 번호만 비교해서, 에셋 변조를 체크해낼 수 없음
                    /*
                                        var clientBundleHash = await UniTask.RunOnThreadPool(() => PatchTool.GetBundleHash(SystemMaintenance.GetBundleFullPathOnPlayPlatform()));
                                        // 현재 클라이언트 번들이 유효한 경우에
                                        if (VersionIndexTableData.GetInstanceUnSafe.IsValidBundle(clientVersion, clientBundleHash))
                                        {
                    #if UNITY_EDITOR
                                            Debug.LogWarning($"현재 클라이언트 버전은 유효합니다. {clientVersion}");
                    #endif
                                            if (clientVersion < serverLatestVersion)
                                            {
                                                result.SetPatchMode(PatchTool.PatchMode.Partial);
                                            }
                                        }
                                        // 현재 클라이언트 번들이 유효하지 않은 경우에
                                        else
                                        {
                                            // 풀패치를 수행한다.
                                            result.SetPatchMode(PatchTool.PatchMode.Full);
                                        }
                    #if UNITY_EDITOR
                                        Debug.LogWarning($"버전을 비교 결과 p:{result.PatchPreset}");
                    #endif
                    #1#
                }
                else
                {
                    throw new Exception($"패치 서버와 연결에 실패했거나 올바른 패치파일을 받는데 실패했습니다.");
                }
            }
        }
        private async UniTask TryGetVersionAssetList(IAsyncTaskRequest p_AsyncTaskRequest)
        {
            if (SystemMaintenance.IsPlayOnTargetPlatform())
            {
                await PatchHistoryTable.GetInstanceUnSafe.TryLoadPatchHistoryFromServer(_PatchCheckResultPreset.PatchPreset);
            }
#if UNITY_EDITOR
            else
            {
                await PatchHistoryTable.GetInstanceUnSafe.TryLoadPatchHistoryFromLocal(_PatchCheckResultPreset.PatchPreset);
            }
#endif
        }
        private async UniTask PatchFile(IAsyncTaskRequest p_AsyncTaskRequest)
        {
            var tryTable = PatchHistoryTable.GetInstanceUnSafe.GetTable();
            var currentCount = 0;
            var downloadCount = tryTable.Count;
            var targetHeader = SystemMaintenance.BundleDirectoryBranch;
#if UNITY_EDITOR
            Debug.LogWarning($"다운로드 받을 번들 갯수 : {downloadCount}");
#endif
            if (SystemMaintenance.IsPlayOnTargetPlatform())
            {
                var downloadTaskSet = new List<UniTask<(bool, UnityWebRequestHandler)>>();
                var resourceHeader = $"{(await NetworkNodeTableData.GetInstance())[NetworkTool.PatchNode].URI.URI.CutLastSlashGetForward()}/{SystemMaintenance.GetFullPatchFileBranch(PathType.SystemGenerate_RelativePath, _PatchCheckResultPreset.PatchPreset.PatchMode, _PatchCheckResultPreset.PatchPreset.TargetVersion)}";
                var localCount = 0;
                SetLockProgress(true);
                foreach (var recordKV in tryTable)
                {
                    currentCount++;
                    var tryRecord = resourceHeader + recordKV.Key;
                    var targetPath = targetHeader + recordKV.Key;
                    var targetPathHeader = targetPath.GetUpperPath();
                    if (localCount < 1)
                    {
                        //SetLabelText($"{recordKV.Key} 다운로드 중\n{currentCount}/{downloadCount}");
                        SetLabelText($"번들 다운로드 중 {currentCount}/{downloadCount}");
                        SetProgress((float)currentCount / downloadCount, true);
                    }
#if UNITY_EDITOR
                    Debug.LogWarning($"[패치 테이블] : {recordKV.Key} 다운로드 시도 : {tryRecord} ===> {targetPathHeader}");
#endif
                    downloadTaskSet.Add(DownloadManager.GetInstance.RunHandler(new NetworkTool.UnityWebRequestParams(tryRecord, NetworkTool.DefaultTimeOutSecond, targetPathHeader), 1f));
                    downloadTaskSet.Add(DownloadManager.GetInstance.RunHandler(new NetworkTool.UnityWebRequestParams(tryRecord + SystemMaintenance.BundleManifestFileExt, NetworkTool.DefaultTimeOutSecond, targetPathHeader), 1f));
                    localCount++;

                    if (localCount > __Download_Width)
                    {
                        var resultTask = await UniTask.WhenAll(downloadTaskSet);
                        foreach (var result in resultTask)
                        {
                            if (!result.Item1)
                            {
#if UNITY_EDITOR
                                Debug.LogWarning($"[패치 테이블] : {result.Item2.WebRequest.url} 다운로드 실패");
#endif
                                SetLockProgress(false);
                                throw new Exception($"{result.Item2.WebRequest.url} 다운로드 실패");
                            }
                        }
                        downloadTaskSet.Clear();
                        localCount = 0;
                    }
                }

                if (localCount > 0)
                {
                    var resultTask = await UniTask.WhenAll(downloadTaskSet);
                    foreach (var result in resultTask)
                    {
                        if (!result.Item1)
                        {
#if UNITY_EDITOR
                            Debug.LogWarning($"[패치 테이블] : {result.Item2.WebRequest.url} 다운로드 실패");

#endif
                            SetLockProgress(false);
                            throw new Exception($"{result.Item2.WebRequest.url} 다운로드 실패");
                        }
                    }
                    downloadTaskSet.Clear();
                    localCount = 0;
                }

                {
                    resourceHeader += SystemMaintenance.BundleMetaDirectoryBranchHeader;
                    var targetPathHeader = targetHeader + SystemMaintenance.BundleMetaDirectoryBranchHeader;
                    if (!Directory.Exists(targetPathHeader))
                    {
                        Directory.CreateDirectory(targetPathHeader);
                    }
                    var webRequestHandler1 = DownloadManager.GetInstance.RunHandler(new NetworkTool.UnityWebRequestParams(resourceHeader + ResourceTool.ResourceListFileNameWithExt, NetworkTool.DefaultTimeOutSecond, targetPathHeader), 1f);
                    var webRequestHandler2 = DownloadManager.GetInstance.RunHandler(new NetworkTool.UnityWebRequestParams(resourceHeader + PatchHistoryTable.GetInstanceUnSafe.GetTableFileName(TableTool.TableNameType.Alter, true), NetworkTool.DefaultTimeOutSecond, targetPathHeader), 1f);
                    var result = await UniTask.WhenAll(webRequestHandler1, webRequestHandler2);
                    if (!result.Item1.Item1 || !result.Item2.Item1)
                    {
#if UNITY_EDITOR
                        Debug.LogWarning($"[패치 테이블] : 메타 테이블 다운로드 실패");
#endif
                        SetLockProgress(false);
                        throw new Exception($"메타 테이블 다운로드 실패");
                    }
                }
            }
            else
            {
                var resourceHeader = SystemMaintenance.GetFullPatchFileBranch(PathType.SystemGenerate_AbsolutePath, _PatchCheckResultPreset.PatchPreset.PatchMode, _PatchCheckResultPreset.PatchPreset.TargetVersion);
                foreach (var recordKV in tryTable)
                {
                    var tryRecord = resourceHeader + recordKV.Key;
                    var targetPath = targetHeader + recordKV.Key;
                    var targetPathHeader = targetPath.GetUpperPath();
                    if (!Directory.Exists(targetPathHeader))
                    {
                        Directory.CreateDirectory(targetPathHeader);
                    }
                    File.Copy(tryRecord, targetPath, true);
                    File.Copy(tryRecord + SystemMaintenance.BundleManifestFileExt, targetPath + SystemMaintenance.BundleManifestFileExt, true);
                }
                {
                    resourceHeader += SystemMaintenance.BundleMetaDirectoryBranchHeader;
                    var targetPathHeader = targetHeader + SystemMaintenance.BundleMetaDirectoryBranchHeader;
                    if (!Directory.Exists(targetPathHeader))
                    {
                        Directory.CreateDirectory(targetPathHeader);
                    }
                    File.Copy(resourceHeader + ResourceTool.ResourceListFileNameWithExt, targetPathHeader + ResourceTool.ResourceListFileNameWithExt, true);
                    File.Copy(resourceHeader + PatchHistoryTable.GetInstanceUnSafe.GetTableFileName(TableTool.TableNameType.Alter, true), targetPathHeader + PatchHistoryTable.GetInstanceUnSafe.GetTableFileName(TableTool.TableNameType.Alter, true), true);
                }
            }
            SetLockProgress(false);
            PatchTool.UpdateClientVersion(_PatchCheckResultPreset.PatchPreset.TargetVersion);
        }
        private async UniTask PatchTerminate(IAsyncTaskRequest p_AsyncTaskRequest)
        {
            await UniTask.DelayFrame(1);
        }
        #endregion
    }*/
}