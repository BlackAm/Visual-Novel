using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /*public partial class PatchTool
    {
        #region <Fields>

        /// <summary>
        /// 매니피스트 파일을 비교할 때 무시해야하는 심볼 그룹
        /// </summary>
        public readonly static List<string> ManifestIgnoreSymbolSet = new List<string>{ "CRC: " };
        
        /// <summary>
        /// 해시코드 시드에 활용할 manifest 파일 내부 심볼
        /// </summary>
        public readonly static List<string> ManifestHashSeedSymbolSet = new List<string>{ "Hash: " };

        #endregion
        
        #region <Enums>

        /// <summary>
        /// 패치 리스트 간의 변경 사항 타입
        /// </summary>
        public enum BundlePatchType
        {
            /// <summary>
            /// 신규 추가
            /// </summary>
            Added,
            
            /// <summary>
            /// 수정
            /// </summary>
            Modified,
            
            /// <summary>
            /// 삭제
            /// </summary>
            Removed
        }

        #endregion

        #region <Methods>
        
        /// <summary>
        /// 지정한 두 브랜치에 포함되어 있는, 번들 리스트 및 매니피스트 파일을 비교하여
        /// 변경사항을 연산하는 메서드.
        ///
        /// 각 브랜치에는 임의의 넘버링을 부여할 수 있다.
        /// 모든 넘버링은 최소버전 값만큼의 하한을 가지고 있으며, 두 넘버링 값이 같은 경우에는
        /// 번들 리스트의 모든 항목을 Add 상태로 취급하여 리턴한다.
        /// </summary>
        public static async UniTask<(bool, PatchHistoryPreset)> GetPatchChanged(int p_From, string p_FromBranch, int p_To, string p_ToBranch)
        {
            // 기준 넘버링을 보정해준다.
            p_From = Mathf.Max(p_From, __Version_Lower_Bound);
            p_To = Mathf.Max(p_To, __Version_Lower_Bound);
            
            var table = new Dictionary<string, PatchHistoryTable.TableRecord>();
            var result = new PatchHistoryPreset(p_From, p_To, table);

            var (fromValid, fromBundleNameSet) = await ResourceListData.GetInstanceUnSafe.GetAssetBundleNameList(p_FromBranch);
            var (toValid, toBundleNameSet) = await ResourceListData.GetInstanceUnSafe.GetAssetBundleNameList(p_ToBranch);

            if (toValid)
            {
                // 현재 버전의 에셋 번들 이름 목록에 대해
                foreach (var currentAssetBundleName in toBundleNameSet)
                {
                    /* SE Cond #1#
                    // 1. 두 버전이 다른 경우
                    // 2. 비교 대상인 from 패스에 에셋번들 이름 리스트 파일이 존재하는 경우
                    // 3. from 에셋번들 이름 리스트에 currentAssetBundleName가 포함되어 있던 경우
                    if (
                        p_From != p_To
                        && fromValid 
                        && fromBundleNameSet.Contains(currentAssetBundleName)
                    )
                    {
                        var tryAssetBundleManifestName = currentAssetBundleName + SystemMaintenance.BundleManifestFileExt;
                        var fromVersionManifestPath = p_FromBranch + tryAssetBundleManifestName;
                        var fromManifest = SystemTool.TryReadLines(fromVersionManifestPath);
                        var toVersionManifestPath = p_ToBranch + tryAssetBundleManifestName;
                        var toManifest = SystemTool.TryReadLines(toVersionManifestPath);

                        if (toManifest.t_Valid && fromManifest.t_Valid)
                        {
                            var currentManifestLines = toManifest.t_Lines;
                            var currentManifestLineCount = currentManifestLines.Length;
                            var prevManifestLines = fromManifest.t_Lines;
                            var prevManifestLineCount = prevManifestLines.Length;

                            if (currentManifestLineCount == prevManifestLineCount)
                            {
                                for (int i = 0; i < currentManifestLineCount; i++)
                                {
                                    var tryCurrentLine = currentManifestLines[i];
                                    if (tryCurrentLine.ContainSymbol(ManifestIgnoreSymbolSet))
                                    {
                                        // 무시해야하는 심볼을 가진 경우
                                    }
                                    else
                                    {
                                        var tryPrevLine = prevManifestLines[i];
                                        if (tryCurrentLine == tryPrevLine)
                                        {
                                            // 두 문자열(라인)이 같은 경우
                                        }
                                        else
                                        {
                                            // 두 문자열(라인)이 다른 경우
                                            var record = new PatchHistoryTable.TableRecord();
                                            await record.SetRecord(currentAssetBundleName, new object[] {BundlePatchType.Modified, p_To});
                                            table.Add(currentAssetBundleName, record);
                                            await record.OnRecordAdded();
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // 두 매니피스트 파일의 라인 숫자가 다른 경우
                                var record = new PatchHistoryTable.TableRecord();
                                await record.SetRecord(currentAssetBundleName, new object[] {BundlePatchType.Modified, p_To});
                                table.Add(currentAssetBundleName, record);
                                await record.OnRecordAdded();
                            }
                        }
                        else
                        {
                            // 두 매니피스트 파일이 유효하지 않은 경우
                        }
                                
                        // 비교가 끝난 번들 이름을 지워준다.
                        fromBundleNameSet.Remove(currentAssetBundleName);
                    }
                    else
                    {
                        var record = new PatchHistoryTable.TableRecord();
                        await record.SetRecord(currentAssetBundleName, new object[] {BundlePatchType.Added, p_To});
                        table.Add(currentAssetBundleName, record);
                        await record.OnRecordAdded();
                    }
                }
            }

            if (fromValid)
            {
                // 위의 반복문에 의해, backUpVersionAssetBundleNameSet에는 현재 버전에 포함되지 않은 번들 이름만이 남는다.
                foreach (var removedBundleName in fromBundleNameSet)
                {
                    var record = new PatchHistoryTable.TableRecord();
                    await record.SetRecord(removedBundleName, new object[] {BundlePatchType.Removed, p_From});
                    table.Add(removedBundleName, record);
                    await record.OnRecordAdded();
                }
            }

            return (true, result);
        }

        /// <summary>
        /// 지정한 두 버전 브랜치의 변경사항을 연산하는 메서드
        /// </summary>
        public static async UniTask<(bool, PatchHistoryPreset)> GetPatchChanged(int p_From, int p_To)
        {
            return await GetPatchChanged
            (
                p_From, SystemMaintenance.GetVersionDirectoryAbsolutePath(p_From), 
                p_To, SystemMaintenance.GetVersionDirectoryAbsolutePath(p_To)
            );
        }

        #endregion
    }*/
}