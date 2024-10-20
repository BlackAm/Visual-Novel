using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    public abstract class MultiTableProxy<This, Key, Label, Table, Record> : AsyncSingleton<This> 
        where Label : struct 
        where Table : ITableBase
        where Record : ITableBaseRecord
        where This : MultiTableProxy<This, Key, Label, Table, Record>, new()
    {
        #region <Fields>

        public MultiTableIndexer<Key, Label, Record> GameDataTableCluster;

        #endregion

        #region <Indexer>

        public IMultiTable<Key, Label, Record> this[Label pt_LabelType] => GameDataTableCluster[pt_LabelType];
        public Record this[(Label, Key) pt_SuperKey] => GameDataTableCluster[pt_SuperKey];
        public Record this[Label p_LabelType, Key p_Key] => GameDataTableCluster[p_LabelType, p_Key];
        public Record this[Key p_Key] => GameDataTableCluster.GetTableData(p_Key);
        protected List<Table> TableList;
        
        #endregion
        
        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            GameDataTableCluster = SpawnGameDataTableCluster();
            TableList = new List<Table>();
            
            var indexableTableSet = typeof(Table).GetSubClassTypeSet();
            foreach (var tableType in indexableTableSet)
            {
                var elementTable = await SingletonTool.CreateAsyncSingleton(tableType);
                if (!ReferenceEquals(null, elementTable))
                {
                    TableList.Add((Table) elementTable);
#if UNITY_EDITOR
                    SingletonTool.OnSingletonAttached((ISingleton) elementTable);
#endif
                }
            }
        }

        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }

        #endregion

        #region <Methods>

        protected abstract MultiTableIndexer<Key, Label, Record> SpawnGameDataTableCluster();

        public bool HasKey(Key p_Key) => GameDataTableCluster.HasKey(p_Key);

        public (bool, Label) GetLabelType(Key p_Key)
        {
            return GameDataTableCluster.GetLabel(p_Key);
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            foreach (var table in TableList)
            {
                table.Dispose();
            }
            TableList = null;
            
            base.DisposeUnManaged();
        }

        #endregion
    }
}