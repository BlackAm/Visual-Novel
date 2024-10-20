using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /*public class PatchHistoryTable : TableBase<PatchHistoryTable, string, PatchHistoryTable.TableRecord>
    {
        public class TableRecord : TableRecordBase
        {
            public PatchTool.BundlePatchType Flag;
            public int LatestVersion;
            
            public override async UniTask SetRecord(string p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                Flag = (PatchTool.BundlePatchType) p_RecordField[0];
                LatestVersion = (int) p_RecordField[1];
            }
        }

        protected override async UniTask OnCreated()
        {
            // 테이블 읽기, 쓰기 과정이 임의의 메서드에 의해 제어되므로, OnCreated를
            // 오버라이드하여 비워놓는다.
            CheckTable();
            TableSerializeType = TableTool.TableSerializeType.NoneSerialize;
            
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// 서버의 최신 버전 프리셋을 로드하는 메서드
        /// </summary>
        public async UniTask TryLoadPatchHistoryFromLocal(PatchTool.PatchPreset p_PatchPreset)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[패치 테이블] : 패치 요약 정보 로드를 시작합니다.");
            Debug.Log(p_PatchPreset.PatchMode);
            Debug.Log(p_PatchPreset.TargetVersion);
#endif
            var patchMode = p_PatchPreset.PatchMode;
            switch (patchMode)
            {
                case PatchTool.PatchMode.Partial:
                case PatchTool.PatchMode.Full:
                    await ReplaceTable(SystemMaintenance.GetFullPatchFileBranch(PathType.SystemGenerate_AbsolutePath, patchMode, p_PatchPreset.TargetVersion) + SystemMaintenance.BundleMetaDirectoryBranchHeader);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// 서버의 최신 버전 프리셋을 로드하는 메서드
        /// </summary>
        public async UniTask TryLoadPatchHistoryFromServer(PatchTool.PatchPreset p_PatchPreset, int p_TimeOut = NetworkTool.DefaultTimeOutSecond, string p_SaveDirectory = "")
        {
#if UNITY_EDITOR
            Debug.LogWarning("[패치 테이블] : 패치 요약 정보 로드를 시작합니다.");
            Debug.Log(p_PatchPreset.PatchMode);
            Debug.Log(p_PatchPreset.TargetVersion);
#endif
            var URI = (await NetworkNodeTableData.GetInstance())[NetworkTool.PatchNode].URI;
            var patchMode = p_PatchPreset.PatchMode;
            switch (patchMode)
            {
                case PatchTool.PatchMode.Partial:
                case PatchTool.PatchMode.Full:
                    URI.SetURIFormat0(SystemMaintenance.GetFullPatchFileBranch(PathType.SystemGenerate_RelativePath, patchMode, p_PatchPreset.TargetVersion) + SystemMaintenance.BundleMetaDirectoryBranchHeader + GetTableFileName(TableTool.TableNameType.Default, true));
                    break;
                default:
                    return;
            }
            
#if UNITY_EDITOR
            Debug.LogWarning($"[패치 테이블] : 대상 URI {URI}");
#endif

            var webRequestParams = new NetworkTool.UnityWebRequestParams(URI, p_TimeOut, p_SaveDirectory);
            var (valid, webRequestHandler) = await DownloadManager.GetInstance.RunHandler(webRequestParams, 1f);
            if (valid)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[패치 테이블] : 패치 테이블이 서버로부터 클래스에 로드되었습니다. {webRequestHandler.WebRequest.result}");
#endif
                var webReqeust = webRequestHandler.WebRequest;
                var text = webReqeust.downloadHandler.text;
                await LoadTableWithRawText(text, false);
                DownloadManager.GetInstance.CancelRequest(webRequestParams);
#if UNITY_EDITOR
                Debug.LogWarning($"[패치 테이블] : 로드된 레코드 개수 [{GetTable().Count}]");
#endif
            }
        }
        
        protected override string GetDefaultTableFileName()
        {
            return "BundlePatchList";
        }
    
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        /// <summary>
        /// 지정한 버전의 디렉터리에 존재하는 패치 리스트 테이블을 로드하여 컬렉션으로 리턴하는 메서드
        /// </summary>
        public async UniTask<Dictionary<string, TableRecord>> GetVersionHistory(int p_Version)
        {
            var targetTablePath = SystemMaintenance.GetVersionDirectoryAbsolutePath(p_Version);
            return await CreateTableFromAbsolutePath(targetTablePath);
        }
        
        /// <summary>
        /// 지정한 버전의 디렉터리에 존재하는 패치 리스트 테이블을 로드하는 메서드
        /// </summary>
        public async UniTask LoadPatchTableFrom(int p_Version)
        {
            var targetTablePath = SystemMaintenance.GetVersionDirectoryAbsolutePath(p_Version);
            await ReplaceTable(targetTablePath);
        }
    }*/
}