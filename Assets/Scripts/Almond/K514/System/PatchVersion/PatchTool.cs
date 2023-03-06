using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 최신버전의 에셋번들을 패치하는 기능을 포함하는 정적클래스
    /// </summary>
    /*public static partial class PatchTool
    {
        #region <Callbacks>

        public static async void OnSystemBoot()
        {
            if (SystemMaintenance.IsPlayOnTargetPlatform())
            {
                var (valid, latestVersionRecord) = VersionIndexTableData.GetInstanceUnSafe.GetLatestRecord();
                var latestVersion = latestVersionRecord.BundleVersion;
                
                if (SystemConfigData.GetInstanceUnSafe.HasBundleResource)
                {
#if UNITY_EDITOR
                    // 에디터 모드의 경우에는 현재 자동 패치모드인지 체크한다.
                    if(SystemFlag.IsAutoPatchMode())
#else
                    // 빌드 플랫폼에서는 해당 어플리케이션이 변조되었는지 체크한다.
                    if (Application.genuine)
#endif
                    {
                        var clientVersion = GetClientVersion();

                        // 현재 클라이언트 버전보다 높은 버전이 서버에 존재하는 경우
                        if (latestVersion > clientVersion)
                        {
                            // 최신버전으로 업데이트 한다.
                            // PatchVersion(latestVersion, true);
                            
                            // 패치된 번들리스트와 에디터에서 제작한 번들리스트를 비교한다.
                            var isBundleCorrect = await CompareBundleList();
                            
                            // 번들이 일치하지 않는 경우             
                            if (!isBundleCorrect)
                            {
#if UNITY_EDITOR
                                Debug.LogError($"[Patch Error] Occured.\n패치파일을 확인해주세요.\n(현재 버전 : {clientVersion} ,릴리스 버전 : {latestVersion})");
#endif
                                InitClientVersion();
                            }
                        }
                    }
                    else
                    {
#if UNITY_EDITOR
#else
                        // 변조된 클라이언트인 경우
                        Application.Quit();
#endif
                    }
                }
                // 번들을 사용하지 않는 프로젝트 였던 경우, 클라이언트 버전만 바꿔준다.
                else
                {
                    UpdateClientVersion(latestVersion);
                }
            }
        }

        #endregion

        #region <Structs>

        public struct PatchPreset
        {
            #region <Fields>

            public PatchMode PatchMode { get; private set; }
            public int CurrentVersion;
            public int TargetVersion;
            
            #endregion

            #region <Constructors>

            public PatchPreset(int p_From, int p_To)
            {
                CurrentVersion = p_From;
                TargetVersion = p_To;
                PatchMode = GetPatchMode(CurrentVersion, TargetVersion);
            }

            public PatchPreset(int p_To)
            {
                CurrentVersion = GetClientVersion();
                TargetVersion = p_To;
                PatchMode = GetPatchMode(CurrentVersion, TargetVersion);
            }

            #endregion

            #region <Operator>

            public static implicit operator PatchPreset((int t_From, int t_To) p_VersionInverval)
            {
                return new PatchPreset(p_VersionInverval.t_From, p_VersionInverval.t_To);
            }
            
            public static implicit operator PatchPreset(int p_To)
            {
                return new PatchPreset(p_To);
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                return $"PatchMode : {PatchMode}\nCurrentVersion : {CurrentVersion}\nTargetVersion : {TargetVersion}";
            }
#endif
            
            #endregion

            #region <Methods>

            public void SwitchPatchMode(PatchMode p_Type)
            {
                PatchMode = p_Type;
            }

            #endregion
        }

        #endregion

        #region <Structs>

        public struct PatchHistoryPreset
        {
            #region <Fields>

            public int _FromVersion { get; private set; }
            public int _ToVersion { get; private set; }
            public Dictionary<string, PatchHistoryTable.TableRecord> _ChangeHistory { get; private set; }

            #endregion

            #region <Constructor>

            public PatchHistoryPreset(int p_From, int p_To, Dictionary<string, PatchHistoryTable.TableRecord> p_Table)
            {
                _FromVersion = p_From;
                _ToVersion = p_To;
                _ChangeHistory = p_Table;
            }

            #endregion
        }

        #endregion
    }*/
}