using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace k514
{
    /// <summary>
    /// 에셋 번들의 빌드는 다음과 같은 프로세스를 가진다.
    ///
    ///    1. 에셋번들빌더를 통한 번들 파일을 '번들 개발 폴더(bundle_dev)'에 생성
    ///    2. 해당 번들을 기준으로 Version 폴더에 '패치파일(BundlePatchList)' 생성
    ///    3. 해당 패치파일을 기준으로 '클라이언트 버전' 조정. 버전 선택시 '번들 폴더(bundle)'에 패치파일의 압축을 풂
    /// 
    /// </summary>
    /*public class AssetBundleBuilder : Singleton<AssetBundleBuilder>
    {
        #region <Consts>

        public const string _DefaultVersionDescription = "여기에 패치 요약을 작성해주세요.";

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 현재 버전
        /// </summary>
        private int currentAssetBundleVersion;

        /// <summary>
        /// 현재 에셋빌더 상태
        /// </summary>
        private AssetBuilderHelperState _currentState;

        /// <summary>
        /// 현재 리소스 폴더와 최신버전의 번들 사이에 변경사항이 있는지 체크하는 플래그
        /// </summary>
        private bool _resourceModifiedFlag;
        
        #endregion

        #region <Enums>

        /// <summary>
        /// 현재 에셋번들 빌더 상태
        /// </summary>
        public enum AssetBuilderHelperState
        {
            /// <summary>
            /// 선택된 작업이 없음
            /// </summary>
            NonSelected,
            
            /// <summary>
            /// 신규 번들 생성
            /// </summary>
            CreateNewBundle,
            
            /// <summary>
            /// 번들 다음 버전으로 업그레이드
            /// </summary>
            Update,
            
            /// <summary>
            /// 작업 수행중
            /// </summary>
            Confirmed,
            
            /// <summary>
            /// 변경사항이 없는 경우
            /// </summary>
            NonModified,
            
            /// <summary>
            /// 에셋번들을 파기하는 경우
            /// </summary>
            Terminate,
        }

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
        }

        public override void OnInitiate()
        {
            SystemMaintenance.InitSystemMaintenance();
            CurrentState = AssetBuilderHelperState.NonSelected;
            CheckVersion();
        }
        
        #endregion
        
        #region <Method/BuildBundle>

        /// <summary>
        /// 현재 솔루션의 리소스가, 어셋 번들의 리소스 목록과 차이가 있는지에 관한 플래그를 리턴한다.
        /// </summary>
        public bool GetVersionModifiedFlag() => _resourceModifiedFlag;

        /// <summary>
        /// 현재 에셋번들 빌더의 상태를 리턴한다.
        /// </summary>
        public AssetBuilderHelperState CurrentState
        {
            get => _currentState;
            set => _currentState = value;
        }

        /// <summary>
        /// 현재 에셋번들 빌더의 상태를 기술할 문자열을 리턴한다.
        /// </summary>
        public string GetCurrentDescription()
        {
            switch (CurrentState)
            {
                case AssetBuilderHelperState.NonSelected:
                    return " >> 현재 선택된 기능이 없습니다.";
                case AssetBuilderHelperState.CreateNewBundle:
                    return $" >> 신규 에셋 번들을 작성합니다. 최초 버전의 넘버링은 {PatchTool.__Version_Lower_Bound}가 됩니다.";
                case AssetBuilderHelperState.Update:
                    return $" >> 신규 에셋 번들은 작성합니다. 해당 번들의 버전은 {currentAssetBundleVersion + 1} 이 됩니다.";
                case AssetBuilderHelperState.NonModified :
                    return $" >> 현재 최신 버전 [{currentAssetBundleVersion}] 과 프로젝트 리소스에 변경사항이 없습니다.";
                case AssetBuilderHelperState.Terminate :
                    return $" >> 모든 에셋번들, 버전, 패치 파일을 파기합니다.";
            }

            return String.Empty;
        }

        /// <summary>
        /// 에셋번들 전용 경로를 검색하여 현재 버전을 계산하는 메서드
        /// </summary>
        private void CheckVersion()
        {
            currentAssetBundleVersion = SystemMaintenance.CalculateLatestBundleVersion();
            if (currentAssetBundleVersion >= PatchTool.__Version_Lower_Bound)
            {
                _resourceModifiedFlag = !ResourceListComparison(currentAssetBundleVersion);
            }
        }

        /// <summary>
        /// 현재 솔루션의 리소스를 리스트화하여, 지정한 버전의 리스트 테이블과 비교후 내용이 같은 경우 참을 리턴하는 논리메서드
        /// </summary>
        private bool ResourceListComparison(int p_TargetVersion)
        {
            var tryPath = ResourceListData.GetInstanceUnSafe.GetTableFileFullPath(AssetLoadType.FromUnityResource, PathType.SystemGenerate_AbsolutePath, TableTool.TableNameType.Alter, true);
            var targetPath = SystemMaintenance.GetVersionDirectoryAbsolutePath(p_TargetVersion) + ResourceListData.GetInstanceUnSafe.GetTableFileName(TableTool.TableNameType.Alter, true);
            var result = false;
            
            // 두 파일이 모두 존재하는 경우
            var tryVersionListExist = File.Exists(tryPath);
            var latestVersionListExist = File.Exists(targetPath);
            if (tryVersionListExist && latestVersionListExist)
            {
                result = File.ReadAllBytes(targetPath).IsSameFile(File.ReadAllBytes(tryPath));
            }

#if UNITY_EDITOR

            if (CustomDebug.PrintAssetBundleBuilder)
            {
                Debug.Log($"Try Version of ResourceList Path : {tryPath} : Exist ? {tryVersionListExist}");
                Debug.Log($"Latest Version of ResourceList Path : {targetPath} : Exist ? {latestVersionListExist}");
                Debug.Log($"Comparison Result : {result}");
            }
#endif
            return result;
        }

        /// <summary>
        /// 현재 에셋번들빌더 상태에 따라 에셋번들을 신규생성하는 메서드
        /// </summary>
        public async UniTask ActSelectedTask(AssetBundleBuilderEventPreset p_EventPreset)
        {
            switch (CurrentState)
            {
                case AssetBuilderHelperState.CreateNewBundle:
                    await ResourceListData.GetInstanceUnSafe.CreateDefaultTable(true);
                    await BuildAssetBundle(p_EventPreset, PatchTool.__Version_Lower_Bound);
                    break;
                case AssetBuilderHelperState.Update:
                    await ResourceListData.GetInstanceUnSafe.CreateDefaultTable(true);
                    await BuildAssetBundle(p_EventPreset, currentAssetBundleVersion + 1);
                    break;
                case AssetBuilderHelperState.Terminate:
                    Directory.Delete(SystemMaintenance.AssetBundleAbsolutePath, true);
                    Dispose();
                    PatchTool.InitClientVersion();
                    break;
            }

            _currentState = AssetBuilderHelperState.NonSelected;
            AssetDatabase.Refresh();
            await SystemFlag.GetInstanceUnSafe.UpdateSystemFlagReleasedVersion();
        }

        /// <summary>
        /// 에셋 번들 디렉터리에 에셋 번들을 생성하는 메서드
        /// </summary>
        private async UniTask BuildAssetBundle(AssetBundleBuilderEventPreset p_EventPreset, int p_Version)
        {
            SetBuildGraphicsSetting(false, false);
            // 비교용으로 이전버전 번들을 백업한다.
            BackupAssetBundle();
            
            var fullBundleStorePath = SystemMaintenance.BundleDirectoryDevBranch;
            var assetBundleManifest = default(AssetBundleManifest);
            try
            {
                assetBundleManifest = BuildPipeline.BuildAssetBundles
                (
                    fullBundleStorePath,
                    BuildAssetBundleOptions.None,
                    /*BuildAssetBundleOptions.ChunkBasedCompression,#1#
                    EditorUserBuildSettings.activeBuildTarget
                );
            }
            catch
            {
                HandleAssetBundleBuildFail(p_Version);
            }

            // 번들 생성에 실패한 경우, 
            if (assetBundleManifest == null)
            {
                // 백업한 번들을 되돌린다.
                HandleAssetBundleBuildFail(p_Version);
            }
            else
            {
                try
                {
                    // 해당 블록에 진입했다면, 현재 에셋번들 생성이 성공해서 dev브랜치에
                    // 풀 패키지 파일이 존재하고, 이전 에셋번들은 dev브랜치에서 prev브랜치로
                    // 이동했다는 것을 의미한다.
                    CopyResourceList(fullBundleStorePath);
                    await UpdateAssetBundle(p_Version);
                    await UpdateVersionDescription(p_EventPreset, p_Version);
                    CheckVersion();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    Debug.LogError(e.StackTrace);
                    HandleAssetBundleBuildFail(p_Version);
                }
            }

            // 번들 백업 디렉터리를 제거해준다.
            RemovePrevAssetBundle();
        }

        /// <summary>
        /// lightMap, Fog 관련 셋팅을 초기화한다
        /// </summary>
        public void SetBuildGraphicsSetting(bool lightMapMode, bool FogMode)
        {
            var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            var serializedObject = new SerializedObject(graphicsSettingsObj);
            var LightMapStripping = serializedObject.FindProperty("m_LightmapStripping");
            var FOGStripping = serializedObject.FindProperty("m_FogStripping");

            LightMapStripping.boolValue = lightMapMode;
            FOGStripping.boolValue = FogMode;

            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 번들 생성 실패시 처리할 로직
        /// </summary>
        private void HandleAssetBundleBuildFail(int p_TargetVersion)
        {
            if (p_TargetVersion == PatchTool.__Version_Lower_Bound)
            {
                RemovePrevAssetBundle();
            }
            RevertPrevAssetBundle();
        }

        /// <summary>
        /// Bundle_dev 를 Bundle_prev 에 이동시켜서
        /// Bundle_dev 에 신규 에셋번들을 기록할 수 있도록 하는 메서드
        /// 최초 버전의 경우 백업할 번들이 없기 때문에 prev 번들 폴더가 생성되지도, 남지도 않는다.
        /// </summary>
        private void BackupAssetBundle()
        {
            var fullBundleStorePath = SystemMaintenance.BundleDirectoryDevBranch;
            var fullBundleBackupPath = SystemMaintenance.BundlePrevDirectoryBranch;
            
            if (Directory.Exists(fullBundleBackupPath))
            {
                Directory.Delete(fullBundleBackupPath, true);
            }
            if (Directory.Exists(fullBundleStorePath))
            {
                Directory.Move(fullBundleStorePath, fullBundleBackupPath);
            }
            Directory.CreateDirectory(fullBundleStorePath);
        }
        
        /// <summary>
        /// 백업한 Bundle_prev 를 Bundle_dev 로 되돌리는 메서드
        /// </summary>
        private void RevertPrevAssetBundle()
        {
            var fullBundleBackupPath = SystemMaintenance.BundlePrevDirectoryBranch;
            var fullBundleStorePath = SystemMaintenance.BundleDirectoryDevBranch;
            
            if (Directory.Exists(fullBundleStorePath))
            {
                Directory.Delete(fullBundleStorePath, true);
            }

            if (Directory.Exists(fullBundleBackupPath))
            {
                Directory.Move(fullBundleBackupPath, fullBundleStorePath);
            }
        }
        
        /// <summary>
        /// 백업한 Bundle_prev 를 제거하는 메서드
        /// </summary>
        private void RemovePrevAssetBundle()
        {
            var fullBundleBackupPath = SystemMaintenance.BundlePrevDirectoryBranch;

            if (Directory.Exists(fullBundleBackupPath))
            {
                Directory.Delete(fullBundleBackupPath, true);
            }
        }

        /// <summary>
        /// prev브랜치에 백업된 풀패키지와 dev브랜치의 풀패키지를 비교하여
        /// 패치변경사항(PatchHistory)을 지정한 버전의 버전폴더에 신규생성하는 메서드
        /// </summary>
        private async UniTask UpdateAssetBundle(int p_TargetVersion)
        {
            // 지정한 버전과 그 이전 버전 사이의 패치 변경사항을 가져온다.
            var (valid, patchHistory) = await PatchTool.GetPatchChanged
                                        (
                                            p_TargetVersion - 1, SystemMaintenance.BundlePrevDirectoryBranch,
                                            p_TargetVersion, SystemMaintenance.BundleDirectoryDevBranch
                                        );

            // 변경사항 작성에 성공한 경우
            if (valid)
            {
                var patchHistoryTable = patchHistory._ChangeHistory;
                var versionDirectoryPath = SystemMaintenance.GetVersionDirectoryAbsolutePath(p_TargetVersion);
                var metaDirectoryPath = versionDirectoryPath + SystemMaintenance.BundleMetaDirectoryBranchHeader;

                // 해당 버전 디렉터리에 변경사항을 PatchHistoryTable로 인코딩하여 업로드한다.
                await PatchHistoryTable.GetInstanceUnSafe.ReplaceTable(patchHistoryTable);
                await PatchHistoryTable.GetInstanceUnSafe.UpdateTableFile(ExportDataTool.WriteType.Overlap, metaDirectoryPath);
                
                // 두 브랜치 사이에서 변경된 파일을 버전 디렉터리에 적용해준다.
                foreach (var _assetBundleNamePair in patchHistoryTable)
                {
                    switch (_assetBundleNamePair.Value.Flag)
                    {
                        case PatchTool.BundlePatchType.Added:
                        case PatchTool.BundlePatchType.Modified:
                            var _assetBundleName = _assetBundleNamePair.Key;
                            var bundleDevDirectoryPath = SystemMaintenance.BundleDirectoryDevBranch;
                            
                            // 에셋번들이 디렉터리 파싱 심볼 /를 가지지 않는 경우에는 따로 디렉터리를 만들 필요가 없다.
                            if (_assetBundleName.Contains('/'))
                            {
                                // 에셋번들이 디렉터리 파싱 심볼 /를 가지는 경우에는, 맨 오른쪽의 디렉터리 심볼 우측이 번들 파일 이름이 되므로, 그부분을 자르고 디렉터리를 만든다.
                                var targetBundleDirectory = versionDirectoryPath + _assetBundleName.CutString("/", true, false);
                                if (!Directory.Exists(targetBundleDirectory))
                                {
                                    Directory.CreateDirectory(targetBundleDirectory);
                                }
                            }
                            
                            var tryManifest = bundleDevDirectoryPath + _assetBundleName;
                            var targetManifest = versionDirectoryPath + _assetBundleName;
                            File.Copy(tryManifest, targetManifest);
                            File.Copy(tryManifest + SystemMaintenance.BundleManifestFileExt, targetManifest + SystemMaintenance.BundleManifestFileExt);                   
                            break;
                        case PatchTool.BundlePatchType.Removed:
                            break;
                    }
                }

                // 패치 테이블은 바로 이전버전과의 변경점 만을 기술하며,
                // 현재 버전에 대한 전체적인 데이터는 ResourceList Table에 기술되어 있다.
                CopyResourceList(metaDirectoryPath);
            }
        }

        /// <summary>
        /// 버전 생성/업데이트시의 이력을 테이블로 작성하는 메서드
        /// </summary>
        private async UniTask UpdateVersionDescription(AssetBundleBuilderEventPreset p_EventPreset, int p_Target)
        {
            var versionIndexTableData = await VersionIndexTableData.GetInstance();
            await versionIndexTableData.AddVersionDescription
            (
                p_Target, 
                p_EventPreset.VersionDescription, 
                PatchTool.GetBundleHash(SystemMaintenance.BundlePrevDirectoryBranch), 
                PatchTool.GetBundleHash(SystemMaintenance.BundleDirectoryDevBranch)
            );
            await versionIndexTableData.UpdateTableFile(ExportDataTool.WriteType.Overlap, SystemMaintenance.BundleVersionIndexBranch);
        }

        /// <summary>
        /// 리소스 리스트를 번들 디렉터리에 복사하는 메서드. 게임 구동 자체에는 번들만 필요하므로, 해당 리소스 데이터를
        /// 실제 플랫폼에 다운로드 시킬 필요는 없다.
        /// 어디까지나 개발중에 에셋번들 버전 관리를 위해 복사하는 것.
        /// </summary>
        private void CopyResourceList(string p_TargetPath)
        {
            var currentResourceListPath = ResourceListData.GetInstanceUnSafe.GetTableFileFullPath(AssetLoadType.FromUnityResource, PathType.SystemGenerate_AbsolutePath, TableTool.TableNameType.Alter, true);
            var targetPath = p_TargetPath + ResourceListData.GetInstanceUnSafe.GetTableFileName(TableTool.TableNameType.Alter, true);
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            File.Copy(currentResourceListPath, targetPath);
        }

        #endregion

        #region <Structs>

        public struct AssetBundleBuilderEventPreset
        {
            public string VersionDescription;
        }

        #endregion
    }*/
}