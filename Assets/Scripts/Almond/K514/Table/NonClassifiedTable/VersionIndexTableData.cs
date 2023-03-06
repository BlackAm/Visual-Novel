using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /*public class VersionIndexTableData : TableBase<VersionIndexTableData, int, VersionIndexTableData.TableRecord>
    {
        /// <summary>
        /// 버전의 해쉬값을 기준으로 정렬된 테이블
        /// </summary>
        private Dictionary<string, TableRecord> HashSortedTable;
        
        public class TableRecord : TableRecordBase
        {
            public string APKVersion;
            public int BundleVersion;
            public string VersionDescription;
            public string VersionDriveDate;
            public string PrevHash;
            public string CurrentHash;

            public override async UniTask SetRecord(int p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);

                APKVersion = (string) p_RecordField[0];
                BundleVersion = (int) p_RecordField[1];
                VersionDescription = (string) p_RecordField[2];
                VersionDriveDate = (string) p_RecordField[3];
                PrevHash = (string) p_RecordField[4];
                CurrentHash = (string) p_RecordField[5];
            }
        }

        protected override async UniTask OnCreated()
        {
            // 테이블 읽기, 쓰기 과정이 임의의 메서드에 의해 제어되므로, OnCreated를
            // 오버라이드하여 비워놓는다.
            CheckTable();
            TableSerializeType = TableTool.TableSerializeType.NoneSerialize;
            HashSortedTable = new Dictionary<string, TableRecord>();
            
            await UniTask.CompletedTask;
        }
        
        public override async UniTask OnInitiate()
        {
            await base.OnInitiate();

            HashSortedTable.Clear();
            
            var table = GetTable();
            foreach (var tableRecordKV in table)
            {
                var tryRecord = tableRecordKV.Value;
                var hash = tryRecord.CurrentHash;

                if (HashSortedTable.ContainsKey(hash))
                {
                    HashSortedTable[hash] = tryRecord;
                }
                else
                {
                    HashSortedTable.Add(hash, tryRecord);
                }
            }            
        }

        public int GetLatestRecordIndex()
        {
            return GetTable().Keys.Max();
        }
        
        public (bool, TableRecord) GetLatestRecord()
        {
            if (GetTable().Count > 0)
            {
                return (true, this[GetLatestRecordIndex()]);
            }
            else
            {
                return default;
            }
        }

        public int GetVersionIndex(int p_Target)
        {
            return GetTable().Keys.Where(key => key == p_Target).Max();
        }
        
        public TableRecord GetVersionRecord(int p_Target)
        {
            return this[GetVersionIndex(p_Target)];
        }
        
        public (bool, TableRecord) GetHashRecord(string p_Hash)
        {
            if (HashSortedTable.TryGetValue(p_Hash, out var o_Record))
            {
                return (true, o_Record);
            }
            else
            {
                return default;
            }
        }
        
        public bool IsValidBundle(int p_TryVersion, string p_Hash)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[버전 테이블] : 해당 클라이언트 해시 {p_TryVersion} / {p_Hash}");
#endif
            if (HashSortedTable.TryGetValue(p_Hash, out var o_Record))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[버전 테이블] : 버전 테이블 해시 버전 {o_Record.BundleVersion}");
#endif
                return o_Record.BundleVersion == p_TryVersion;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[버전 테이블] : 일치하는 해시가 없습니다.");
#endif
                return false;
            }
        }
                
        /// <summary>
        /// 서버의 최신 버전 프리셋을 로드하는 메서드
        /// </summary>
        public async UniTask TryLoadVersionIndexFromLocal()
        {
#if UNITY_EDITOR
            Debug.LogWarning("[버전 테이블] : 버젼 프리셋 정보 로드를 시작합니다.");
#endif
            await ReplaceTable(SystemMaintenance.BundleVersionIndexBranch);
#if UNITY_EDITOR
            Debug.LogWarning($"[버전 테이블] : 로드된 레코드 개수 [{GetTable().Count}]");
#endif
        }
        
        /// <summary>
        /// 서버의 최신 버전 프리셋을 로드하는 메서드
        /// </summary>
        public async UniTask TryLoadVersionIndexFromServer(int p_TimeOut = NetworkTool.DefaultTimeOutSecond, string p_SaveDirectory = "")
        {
#if UNITY_EDITOR
            Debug.LogWarning("[버전 테이블] : 버젼 프리셋 정보 로드를 시작합니다. : " + NetworkTool.PatchNode);
#endif
            var URI = (await NetworkNodeTableData.GetInstance())[NetworkTool.PatchNode].URI;
            Debug.LogWarning("URI : " + URI);
            URI.SetURIFormat0(SystemMaintenance.VersionDirectoryBranchHeader + GetTableFileName(TableTool.TableNameType.Default, true));
#if UNITY_EDITOR
            Debug.LogWarning($"[버전 테이블] : 대상 URI {URI}");
#endif
            var webRequestParams = new NetworkTool.UnityWebRequestParams(URI, p_TimeOut, p_SaveDirectory);
            var (valid, webRequestHandler) = await DownloadManager.GetInstance.RunHandler(webRequestParams, 1f);
            if (valid)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[버전 테이블] : 버젼 테이블이 서버로부터 클래스에 로드되었습니다. {webRequestHandler.WebRequest.result}");
#endif
                var webReqeust = webRequestHandler.WebRequest;
                var text = webReqeust.downloadHandler.text;
                await LoadTableWithRawText(text, false);
                DownloadManager.GetInstance.CancelRequest(webRequestParams);
#if UNITY_EDITOR
                Debug.LogWarning($"[버전 테이블] : 로드된 레코드 개수 [{GetTable().Count}]");
#endif
            }
        }
        
#if UNITY_EDITOR
        protected override async UniTask GenerateDefaultRecordSet()
        {
            await base.GenerateDefaultRecordSet();
            
            await AddVersionDescription(0, "Table Spawned", "114", "514");
        }

        public async UniTask AddVersionDescription(int p_BundleVersion, string p_Description, string p_PrevHash, string p_CurrentHash)
        {
            var nextKey = _Table.Keys.Count;
            await AddRecord(nextKey, new object[]{Application.version, p_BundleVersion, p_Description, $"{DateTime.Now.ToLongDateString()}, {DateTime.Now.ToLongTimeString()}", p_PrevHash, p_CurrentHash});
        }
#endif

        protected override string GetDefaultTableFileName()
        {
            return "VersionIndex";
        }
    
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }*/
}