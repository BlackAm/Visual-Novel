using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// GameDataIndex를 사용하고자 하는 데이터 클래스는 해당 인터페이스를 상속해줘야 한다.
    /// </summary>
    public interface IMultiTable<Key, Label, Record> where Label : struct
    {
        Key StartIndex { get; }
        Key EndIndex { get; }
        MultiTableIndexer<Key, Label, Record> GetMultiGameIndex();
        void InitIntervalIndex();
        Label GetThisLabelType();
        void ConnectIndexer();
        bool HasKey(Key p_Key);
        Record GetTableData(Key p_Key);
        Key Convert_To_OrdinalKey(Key p_Key);
        Key Convert_To_IndexKey(Key p_Key);

        /// <summary>
        /// 호출 시, 힙발생
        /// </summary>
        List<Key> GetValidKeyEnumerator();
    }
    
    /// <summary>
    /// 다수의 테이블을 하나의 테이블처럼 관리하는 테이블 클래스 구현체
    /// </summary>
    public abstract class MultiTableBase<M, K, T, Label, Record> : GameTable<M, K, T>, IMultiTable<K, Label, Record> 
        where M : MultiTableBase<M, K, T, Label, Record>, new() 
        where T : GameTable<M, K, T>.GameTableRecordBase, Record, new() 
        where Label : struct
    {
        #region <IIndexableGameData>

        public K StartIndex { get; protected set; }
        public K EndIndex { get; protected set; }
        public abstract MultiTableIndexer<K, Label, Record> GetMultiGameIndex();
        public abstract void InitIntervalIndex();
        public abstract Label GetThisLabelType();
        public void ConnectIndexer()
        {
            GetMultiGameIndex().JoinTable(this, GetThisLabelType());
        }

        Record IMultiTable<K, Label, Record>.GetTableData(K p_Key)
        {
            return GetTable()[p_Key];
        }

        public K Convert_To_OrdinalKey(K p_Key)
        {
            return GetMultiGameIndex().Convert_To_OrdinalKey(GetThisLabelType(), p_Key);
        }
        
        public K Convert_To_IndexKey(K p_Key)
        {
            return GetMultiGameIndex().Convert_To_IndexKey(GetThisLabelType(), p_Key);
        }

        public List<K> GetValidKeyEnumerator()
        {
            return GetTable().Keys.ToList();
        }

        public void InitIndexInfo()
        {
            InitIntervalIndex();
            ConnectIndexer();
        }

        #endregion

        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            await base.OnCreated();
            
            InitIndexInfo();
        }

        #endregion
    }
}