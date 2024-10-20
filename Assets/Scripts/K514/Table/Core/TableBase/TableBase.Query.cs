using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class TableBase<M, K, T>
    {
        #region <Record>
        
        /// <summary>
        /// 기본 레코드 쿼리 메서드
        /// </summary>
        public T GetTableData(K p_Key) => _Table[p_Key];

        /// <summary>
        /// 인덱스 쿼리
        /// </summary>
        public T this[K p_Key] => _Table[p_Key];

        public async UniTask<T> SpawnRecord(K p_Key, params object[] p_Params)
        {
            var spawnedRecord = new T();
            await spawnedRecord.SetRecord(p_Key, p_Params);
            await spawnedRecord.OnRecordDecoded();
            return spawnedRecord;
        }

        public bool HasKey(K p_Key) => _Table.ContainsKey(p_Key);
        
        public async UniTask AddRecord(T p_Record)
        {
            _Table.Add(p_Record.KEY, p_Record);
            await p_Record.OnRecordAdded();
        }
        
        public async UniTask<T> AddRecord(K p_Key, params object[] p_Params)
        {
            var spawnedRecord = await SpawnRecord(p_Key, p_Params);
            await AddRecord(spawnedRecord);
            return spawnedRecord;
        }

        public async UniTask ReplaceRecord(K p_Key, params object[] p_Params)
        {
            if (_Table.ContainsKey(p_Key))
            {
                _Table.Remove(p_Key);
            }

            await AddRecord(p_Key, p_Params);
        }
        
        #endregion

        #region <Table>

        /// <summary>
        /// 해당 테이블 타입과 동일한 텅빈 컬렉션 하나를 인스턴스화 하여 리턴하는 메서드
        /// </summary>
        public Dictionary<K, T> SpawnEmptyTable()
        {
            return new Dictionary<K, T>();
        }
        
        /// <summary>
        /// 테이블 컬렉션의 생성됨을 보장하는 메서드
        /// </summary>
        protected void CheckTable()
        {
            if (_Table == null)
            {
                _Table = SpawnEmptyTable();
            }
        }
        
        /// <summary>
        /// 테이블을 비우는 메서드
        /// </summary>
        public void ClearTable()
        {
            CheckTable();
            OnTableBlowUp();
            _Table.Clear();
        }
        
        
        /// <summary>
        /// 해당 테이블에 초기화시, 테이블에 반드시 존재해야하는 레코드 셋을 탐색하는 메서드
        /// </summary>
        protected virtual async UniTask CheckMissedRecordSet()
        {
            await UniTask.CompletedTask;
        }
        
#if UNITY_EDITOR
        public async UniTask CreateDefaultTable(bool p_OverlapLegacyTableFlag)
        {
            if (this.IsUnityResourceTable())
            {
                // 해당 테이블 타입에 맞는 디렉터리가 있는지 체크하고 없다면 생성한다.
                SystemMaintenance.CreateTableDirectory(TableType);
                var tryPath = GetTableFileRootPath(AssetLoadType.FromUnityResource, PathType.SystemGenerate_AbsolutePath);
                var tryFullPath = tryPath + GetTableFileName(TableTool.TableNameType.Alter, true);
                try
                {
                    // #SE Condition
                    // 1. 덮어쓰기 플래그가 set인 경우
                    // 2. 지정한 위치에 테이블이 존재하지 않는 경우
                    //
                    if (p_OverlapLegacyTableFlag || !File.Exists(tryFullPath))
                    {
                        // 테이블 컬렉션을 정리한다.
                        ClearTable();
                        // 기본 테이블 레코드 인스턴스로 테이블 컬렉션을 채운다.
                        await GenerateDefaultRecordSet();
                        // 테이블 컬렉션을 테이블 파일로 기술한다.
                        await UpdateTableFile(ExportDataTool.WriteType.Overlap, tryPath);
                    }
                }
                // 파일 탐색에서 예외가 발생한 경우, 관련 메시지를 출력한다.
                catch (Exception e)
                {
                    Debug.LogError($"{tryFullPath} 테이블을 생성하던 중 에러가 발생했습니다.\n{e.Message}\n{e.StackTrace}");
                }
            }
        }

        /// <summary>
        /// 해당 테이블에 들어갈 기본 값을 테이블 컬렉션에 레코드 인스턴스로 추가하는 메서드
        /// </summary>
        protected virtual async UniTask GenerateDefaultRecordSet()
        {
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// 해당 테이블에 등록된 레코드를 유니티 디버그 메서드를 통해 출력하는 메서드
        /// </summary>
        public void PrintTable()
        {
            var tryTable = GetTable();
            Debug.Log($"*****************************************************");
            Debug.Log($"Table Name : [{GetTableFileName(TableTool.TableNameType.Alter, false)}]");
            Debug.Log($"Table Record Count : [{GetTable().Count}]");
            foreach (var record in tryTable)
            {
                Debug.Log(record.Value.GetDescription());
            }
            Debug.Log($"*****************************************************");
        }
#endif
        
        #endregion

        #region <Table/Replace>
                
        /// <summary>
        /// 테이블을 비우고 리로드하는 메서드
        /// </summary>
        public async UniTask ReloadTable(string p_CustomName)
        {
            SetTableName(p_CustomName);
            await LoadTable(false);
        }
        
        /// <summary>
        /// 지정한 절대 경로로부터 테이블 파일을 탐색하여 읽는 메서드.
        /// </summary>
        protected async UniTask ReplaceTable(string p_TableAbsolutePath)
        {
            await ReplaceTable(await CreateTableFromAbsolutePath(p_TableAbsolutePath));
        }
        
        /// <summary>
        /// 입력받은 컬렉션으로 현재 테이블 싱글톤의 컬렉션을 교체하는 메서드
        /// </summary>
        public async UniTask ReplaceTable(Dictionary<K, T> p_TargetTable)
        {
            ClearTable();
            _Table = p_TargetTable;
            await OnTableLoaded(false);
        }

        #endregion
    }
}