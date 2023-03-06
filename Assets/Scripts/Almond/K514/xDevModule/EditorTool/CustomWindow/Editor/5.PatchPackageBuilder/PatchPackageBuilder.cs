using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 각 버전의 변경 사항 테이블(PatchTool)을 참조하여 특정 버전으로의 패치파일을 생성하는 클래스
    /// </summary>
    /*public class PatchPackageBuilder : Singleton<PatchPackageBuilder>
    {
        #region <Fields>

        /// <summary>
        /// 풀버전 패치파일 생성시 번들 폴더로부터 압축에서 제외해야할 파일명 리스트
        /// </summary>
        private List<string> _ExceptedFileNameset_FullPatch;
        
        /// <summary>
        /// 부분 패치파일 생성시 번들 폴더로부터 압축에서 제외해야할 파일명 리스트
        /// </summary>
        private List<string> _ExceptedFileNameset_PartialPatch;

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _ExceptedFileNameset_FullPatch = new List<string>();
            _ExceptedFileNameset_FullPatch.Add(ResourceListData.GetInstanceUnSafe.GetTableFileName(TableTool.TableNameType.Default, true));
            
            _ExceptedFileNameset_PartialPatch = new List<string>();
            _ExceptedFileNameset_PartialPatch.Add(ResourceListData.GetInstanceUnSafe.GetTableFileName(TableTool.TableNameType.Default, true));
        }

        public override void OnInitiate()
        {
        }

        #endregion

        #region <Methods>

        public async UniTask BuildPatchPackageAndPatch(int p_From, int p_To, bool p_GetPatchFromServerPatch)
        {
            var hasPatchFile = PatchTool.HasLocalPatchPackage(p_From, p_To);
            if (!hasPatchFile)
            {
                BuildPatchPackage(p_From, p_To);
            }
            
            await PatchTool.PatchVersion(p_To, p_GetPatchFromServerPatch);
        }

        /// <summary>
        /// 지정한 버전 p_From의 클라이언트가 특정한 버전 p_To에서 동작하도록 두 버전간의 패치파일을 생성하는 메서드
        /// </summary>
        public void BuildPatchPackage(int p_From, int p_To)
        {
            switch (PatchTool.GetPatchMode(p_From, p_To))
            {
                case PatchTool.PatchMode.Partial:
                {
                    BuildPartialPatchPackage(p_To);
                }
                    break;
                case PatchTool.PatchMode.Full:
                {
                    BuildFullPatchPackage(p_To);
                }
                    break;
            }
        }

        /// <summary>
        /// 지정한 버전으로의 부분 패치 패키지 파일을 생성하는 메서드
        /// </summary>
        public void BuildPartialPatchPackage(int p_To)
        {
            p_To = Mathf.Max(p_To, PatchTool.__Version_Lower_Bound);
            var decompressFlag = true;

#if UNITY_EDITOR
            if (CustomDebug.PrintPatchFileBuild)
            {
                Debug.Log($" * 부분 패치 파일 생성을 시작 : [{p_To}]");
            }
#endif
            var partialCompressDirectory = SystemMaintenance.GetPatchFilePathHeader(PathType.SystemGenerate_AbsolutePath, PatchTool.PatchMode.Partial);
            if (Directory.Exists(partialCompressDirectory))
            {
                Directory.Delete(partialCompressDirectory, true);
            }
            Directory.CreateDirectory(partialCompressDirectory);
  
            for (int i = PatchTool.__Version_Lower_Bound; i <= p_To; i++)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPatchFileBuild)
                {
                    if (i <= PatchTool.__Version_Lower_Bound)
                    {
                        Debug.Log($"  - 초기 버전 파일 생성 : [{PatchTool.__Version_Lower_Bound}]");
                    }
                    else
                    {
                        Debug.Log($"  - 브릿지 패치 파일 생성 : [{i - 1}] => [{i}]");
                    }

                }
#endif
                var partialVersionPath = SystemMaintenance.GetVersionDirectoryAbsolutePath(i);
                var compressFilePath = SystemMaintenance.GetFullPatchFilePath(PathType.SystemGenerate_AbsolutePath, PatchTool.PatchMode.Partial, i);
                
                // 패치 파일이 생성될 디렉터리를 생성한다.
                // 지정한 버전 디렉터리를 압축한다.
                EncodeData.CompressDirectory(ExportDataTool.CompressType.Gzip, partialVersionPath, compressFilePath, _ExceptedFileNameset_PartialPatch);

                // 테스트 용 : 압축된 패치파일을 그 자리에 해제하는 코드
                if (decompressFlag)
                {
                    DecodeData.DecompressDirectory(ExportDataTool.CompressType.Gzip, compressFilePath);
                }
            }
        }

        /// <summary>
        /// 지정한 버전으로의 풀 패치 패키지 파일을 생성하는 메서드
        /// </summary>
        public void BuildFullPatchPackage(int p_TargetVersion)
        {
            p_TargetVersion = Mathf.Max(p_TargetVersion, PatchTool.__Version_Lower_Bound);
            var decompressFlag = true;
            
#if UNITY_EDITOR
            if (CustomDebug.PrintPatchFileBuild)
            {
                Debug.Log($" * 풀버전({p_TargetVersion}) 패치 파일 생성을 시작");
            }
#endif
            // 압축을 해제할 디렉터리를 초기화시킨다.
            var compressDirectory = SystemMaintenance.GetPatchFilePathHeader(PathType.SystemGenerate_AbsolutePath, PatchTool.PatchMode.Full);
            var compressFilePath = SystemMaintenance.GetFullPatchFilePath(PathType.SystemGenerate_AbsolutePath, PatchTool.PatchMode.Full, p_TargetVersion);
            
            // 패치를 누적시킬 임시 디렉터리를 초기화시킨다.
            var tmpPath = $"{compressDirectory}{SystemMaintenance.BundleTmpBranchHeader}";
            if (Directory.Exists(tmpPath))
            {
                Directory.Delete(tmpPath, true);
            }
            Directory.CreateDirectory(tmpPath);
            
            // 최소버전 번들 디렉터리를 임시 디렉터리로 복사해온다.
            var zeroVersionPath = SystemMaintenance.GetVersionDirectoryAbsolutePath(PatchTool.__Version_Lower_Bound);
            SystemTool.CopyDirectoryTo(zeroVersionPath, tmpPath);
#if UNITY_EDITOR
            if (CustomDebug.PrintPatchFileBuild)
            {
                Debug.Log($" - 기저 버전 [{PatchTool.__Version_Lower_Bound}] 복사");
            }
#endif
            for (int i = PatchTool.__Version_Lower_Bound; i < p_TargetVersion; i++)
            {
                // 복사된 최소버전에, [최소버전 이후로부터 지정한 버전까지]의 패치를 적용시켜준다.
                var applyVersion = i + 1;
#if UNITY_EDITOR
                if (CustomDebug.PrintPatchFileBuild)
                {
                    Debug.Log($" - 버전 [{applyVersion}] => [{i}] 병합");
                }
#endif
                var targetPatchVersionPath = SystemMaintenance.GetVersionDirectoryAbsolutePath(applyVersion);
                PatchTool.ApplyPatch(targetPatchVersionPath, tmpPath);
            }
          
            // p_SpawnPath에 패치가 적용된 파일들을 fullPatchFileDirectory에 압축해준다.
            EncodeData.CompressDirectory(ExportDataTool.CompressType.Gzip, tmpPath, compressFilePath, _ExceptedFileNameset_FullPatch);
                
            // 테스트 용 : 압축된 패치파일을 그 자리에 해제하는 코드
            if (decompressFlag)
            {
                DecodeData.DecompressDirectory(ExportDataTool.CompressType.Gzip, compressFilePath);
            }
                
            // 패치 패키지 생성용 임시 디렉터리를 제거한다.
            if (Directory.Exists(tmpPath))
            {
                Directory.Delete(tmpPath, true);
            }
        }

        #endregion
    }*/
}