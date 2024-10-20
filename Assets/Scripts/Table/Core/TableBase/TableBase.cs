using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BlackAm
{
    /// <summary>
    /// 게임 데이터를 기술하는 추상 클래스
    /// 게임 데이터 값들을 테이블로 관리할 때 사용한다.
    /// 각 게임 데이터는 하나의 싱글톤으로 구성된다
    ///
    /// 기존에는 싱글톤을 상속받았으나, 테이블의 크기가 큰 경우에
    /// 테이블 로드 타임이 오래걸리게 되어 비동기 싱글톤을 상속받도록 변경되었다.
    /// 
    /// </summary>
    /// <typeparam name="K">테이블 키 타입</typeparam>
    /// <typeparam name="T">테이블 레코드 타입</typeparam>
    public abstract partial class TableBase<M, K, T> : AsyncSingleton<M>, ITableBase where T : TableBase<M, K, T>.TableRecordBase, new() where M : TableBase<M, K, T>, new()
    {
        #region <Fields>

        /// <summary>
        /// [키값, 레코드] 컬렉션
        /// </summary>
        protected Dictionary<K, T> _Table;

        /// <summary>
        /// 클래스에 정의된 테이블 이름 외의 테이블 이름을 사용하고자 할 때 사용되는 테이블 이름 필드
        /// </summary>
        private (bool t_HasAlterName, string t_AlterName) _CustomTableName;

        /// <summary>
        /// 테이블 생성등에 사용할 기본 브랜치
        /// </summary>
        private (bool t_HasBranch, string t_BranchName) _BranchNameTuple;

        /// <summary>
        /// 테이블 타입
        /// </summary>
        private TableTool.TableType _TableType;
        
        /// <summary>
        /// 테이블 타입 프로퍼티
        /// </summary>
        public TableTool.TableType TableType 
        {
            get => _TableType;
            protected set
            {
                _TableType = value;
                switch (_TableType)
                {
                    case TableTool.TableType.SceneGameTable:
                        TableManager.GetInstance.Subscribe_LifeCycleType_GameDataTable(this);
                        break;
                    case TableTool.TableType.WholeGameTable:
                        TableManager.GetInstance.OnDisposeScene_LifeCycleType_GameDataTable(this);
                        break;
                    case TableTool.TableType.EditorOnlyTable:
                        TableManager.GetInstance.OnDisposeScene_LifeCycleType_GameDataTable(this);
                        break;
                    case TableTool.TableType.SystemTable:
                        TableManager.GetInstance.OnDisposeScene_LifeCycleType_GameDataTable(this);
                        break;
                }
            }
        }

        /// <summary>
        /// 테이블 직렬화 타입
        /// </summary>
        public TableTool.TableSerializeType TableSerializeType { get; protected set; } =
            TableTool.TableSerializeType.SerializeString;
        
        /// <summary>
        /// 테이블 접근자
        /// </summary>
        public Dictionary<K, T> GetTable()
        {
            return _Table;
        }

        #endregion

        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            await LoadTable(true).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
            
#if UNITY_EDITOR
            if (CustomDebug.PrintSingletonLoading)
            {
                var tryTable = GetTable();
                Debug.Log(GetType().Name + " table : " + tryTable.Count);
            }
#endif
        }

        /// <summary>
        /// 게임 데이터 싱글톤 초기화 : 딱히 수행할 것 없음.
        /// 테이블 최초 생성 시, 테이블 리로딩 시 테이블이 완성된 상태에서
        /// 수행할 테이블 외 초기화 작업을 기술한다.
        /// </summary>
        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// 테이블 파일이 업데이트 된 경우 수행할 작업을 기술하는 콜백
        /// 현 시점에서는 빌드된 클라이언트에서 테이블을 수정할 필요성이 없기 때문에
        /// 에디터 모드에서만 동작하도록 정의 되어있다.
        /// </summary>
        public virtual async UniTask OnUpdateTableFile()
        {
            await UniTask.SwitchToMainThread();
            
            // 파일이 신규 생성되었을지 모르므로, 프로젝트의 에셋목록을 업데이트해준다.
            AssetDatabase.Refresh();
        }
#endif
        
        /// <summary>
        /// 테이블이 로드된 상태에서 데이터가 초기화되는 경우 호출되는 콜백
        /// </summary>
        protected virtual void OnTableBlowUp()
        {
        }

        /// <summary>
        /// 해당 클래스에 테이블이 로드되는 경우 호출되는 콜백
        /// </summary>
        protected async UniTask OnTableLoaded(bool p_InvokeFromSingletonInitiate)
        {
            await CheckMissedRecordSet();
            
            var table = GetTable();
            if (table != null)
            {
                foreach (var recordPair in table)
                {
                    var record = recordPair.Value;
                    await record.OnRecordAdded();
                }
            }

            if (!p_InvokeFromSingletonInitiate)
            {
                await OnInitiate();
            }

#if UNITY_EDITOR
            await TryWriteByteCode();
#endif
        }
        
        #endregion
      
        #region <Methods>

        /// <summary>
        /// 테이블 별명을 갱신하는 메서드
        /// 확장자를 제외한 이름만을 넘겨주면 되며, null이나 공백을 넘겨받는 경우
        /// 별명을 파기하고 원래 테이블을 읽도록 동작한다.
        /// </summary>
        private void SetTableName(string p_TableName)
        {
            _CustomTableName = string.IsNullOrEmpty(p_TableName) ? default : (true, p_TableName);
        }

        protected void SetBranchName(string p_Name)
        {
            _BranchNameTuple = string.IsNullOrEmpty(p_Name) ? default : (true, p_Name);
        }

        /// <summary>
        /// 해당 테이블 파일 타입
        /// </summary>
        public abstract TableTool.TableFileType GetTableFileType();
        
        /// <summary>
        /// 해당 테이블의 기본 파일명을 리턴하는 메서드
        /// </summary>
        protected abstract string GetDefaultTableFileName();

        /// <summary>
        /// 해당 테이블의 파일명을 리턴하는 메서드
        /// </summary>
        public string GetTableFileName(TableTool.TableNameType p_Type)
        {
            switch (p_Type)
            {
                default:
                case TableTool.TableNameType.Default:
                    return GetDefaultTableFileName();
                case TableTool.TableNameType.Alter:
                    return _CustomTableName.t_HasAlterName ? _CustomTableName.t_AlterName : GetDefaultTableFileName();
            }
        }

        /// <summary>
        /// 해당 테이블의 파일명을 리턴하는 메서드
        /// </summary>
        public string GetTableFileName(TableTool.TableNameType p_Type, bool p_AttachExt)
        {
            return p_AttachExt ? GetTableFileName(p_Type) + GetTableFileType().GetTableExtension() : GetTableFileName(p_Type);
        }

        #endregion
        
        #region <Disposable>

        /// <summary>
        /// 게임 데이터 싱글톤 파기 : 정적 변수 초기화
        ///
        /// xml 테이블 자체는 로드와 동시에 제거되고, 메모리에 GameData 인스턴스 형태로 올라온
        /// 해당 인스턴스를 제거하는 쪽이 Dispose 계열 메서드이다.
        /// 
        /// </summary>
        protected override void DisposeUnManaged()
        {
            switch (TableType)
            {
                case TableTool.TableType.SceneGameTable :
                    // 씬단위 테이블 싱글톤 인스턴스는 여기에서 제거되며, 테이블 파일 자체는 
                    // LoadAssetManager 클래스에 의해 관리된다.
                    TableManager.GetInstance.OnDisposeScene_LifeCycleType_GameDataTable(this);
                    break;
            }

            OnTableBlowUp();
            _Table = null;
            
            base.DisposeUnManaged();
        }
        
        #endregion
    }
}