using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public partial class PatchTool
    {
        /*#region <Methods>
        
        /// <summary>
        /// p_FromPath 디렉터리에 존재하는 패치리스트 테이블(PatchHistoryTable)을 읽어들여
        /// p_TargetPath 디렉터리에 각 번들의 추가/삭제를 적용시키는 메서드
        /// </summary>
        public static async UniTask ApplyPatch(string p_FromPath, string p_TargetPath)
        {
            var patchHistoryTable = await PatchHistoryTable.GetInstance();
            // 지정한 경로에 패치 테이블이 존재하는지 검증을 수행한다.
            if (patchHistoryTable.HasTableCollectionFromAbsolutePath(p_FromPath))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPatchFileBuild)
                {
                    Debug.Log($" -- 경로 [{p_FromPath}]에서 패치 테이블을 찾았습니다.");
                }
#endif
                // 해당 경로의 패치 테이블을 읽고 가져온다.
                var patchTable = await patchHistoryTable.CreateTableFromAbsolutePath(p_FromPath);
                foreach (var patchRecord in patchTable)
                {
                    ApplyPatchRecord(p_FromPath, p_TargetPath, patchRecord.Value);
                }
            }
            // 패치 테이블이 없는 경우, 디렉터리를 일괄 복사시킨다.
            else
            {
                SystemTool.CopyDirectoryTo(p_FromPath, p_TargetPath);
            }
        }
        
        /// <summary>
        /// 패치리스트 테이블(PatchHistoryTable)의 레코드에 따라 두 브랜치 간의 패치를 적용시키는 메서드
        /// </summary>
        public static void ApplyPatchRecord(string p_FromPath, string p_TargetPath, PatchHistoryTable.TableRecord p_Record)
        {
            switch (p_Record.Flag)
            {
                // 특정 번들이 신규 추가되거나 수정된 경우, 해당 번들 및 매니피스트 파일을 타겟 경로로 옮겨준다.
                case BundlePatchType.Added:
                case BundlePatchType.Modified:
                {
                    var tryBundleFullName = $"{p_FromPath}{p_Record.KEY}";
                    var tryManifestFullName = $"{tryBundleFullName}{SystemMaintenance.BundleManifestFileExt}";
                    SystemTool.CopyFileTo(p_FromPath, tryBundleFullName, p_TargetPath, true);
                    SystemTool.CopyFileTo(p_FromPath, tryManifestFullName, p_TargetPath, true);
                }
                    break;
                // 삭제된 번들을 제거해준다.
                case BundlePatchType.Removed:
                {
                    var targetBundleFullName = $"{p_TargetPath}{p_Record.KEY}";
                    var targetBundleManifestFullName = $"{targetBundleFullName}{SystemMaintenance.BundleManifestFileExt}";
                    if (File.Exists(targetBundleFullName))
                    {
                        File.Delete(targetBundleFullName);
                    }
                    if (File.Exists(targetBundleManifestFullName))
                    {
                        File.Delete(targetBundleManifestFullName);
                    }
                }
                    break;
            }
        }
                
        /// <summary>
        /// p_FromPath 디렉터리에 존재하는 패치리스트 테이블(PatchHistoryTable)의 레코드의 버전 필드를 통해
        /// 각 번들 항목의 버전 디렉터라로부터 패치 변경사항을 p_TargetPath 디렉터리에 적용시키는 메서드
        /// </summary>
        public static void ApplyPatchRecord(string p_TargetPath, PatchHistoryTable.TableRecord p_Record)
        {
            var targetBundleVersion = p_Record.LatestVersion;
            var targetPatchVersionPath = SystemMaintenance.GetVersionDirectoryAbsolutePath(targetBundleVersion);
            ApplyPatchRecord(targetPatchVersionPath, p_TargetPath, p_Record);
        }
        
        #endregion*/
    }
}