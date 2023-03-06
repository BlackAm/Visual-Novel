using Cysharp.Threading.Tasks;

namespace k514
{
    /// <summary>
    /// 프리팹 모델 데이터 테이블 클래스의 추상 클래스
    /// </summary>
    public abstract class PrefabModelDataTable<M, K, T> : MultiTableBase<M, K, T, PrefabModelDataRoot.PrefabModelDataType, PrefabModelDataRecordBridge> 
        where M : PrefabModelDataTable<M, K, T>, new() 
        where T : PrefabModelDataTable<M, K, T>.PrefabModelDataRecord, new()
    {
        /// <summary>
        /// 프리팹 모델 데이터 테이블 레코드 클래스의 추상 클래스
        /// </summary>
        public abstract class PrefabModelDataRecord : GameTableRecordBase, PrefabModelDataRecordBridge
        {
            /// <summary>
            /// 해당 모델 데이터가 기술하는 프리팹 이름
            /// </summary>
            protected string PrefabName;
            
            /// <summary>
            /// 해당 프리팹 모델 스케일
            /// </summary>
            public float PrefabModelScale { get; protected set; }

            /// <summary>
            /// 프리팹 이름을 리턴하는 메서드
            /// </summary>
            public virtual string GetPrefabName() 
#if SERVER_DRIVE
                => "ServerDoppel";
#else
                => PrefabName;
#endif
            
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                if (PrefabModelScale < CustomMath.Epsilon)
                {
                    PrefabModelScale = 1f;
                }
            }
        }
    }
    
    /// <summary>
    /// 프리팹 모델 데이터 테이블 클래스의 정수 특정 추상 클래스
    /// </summary>
    public abstract class PrefabModelDataIntTable<M, T> : PrefabModelDataTable<M, int, T> where M : PrefabModelDataIntTable<M, T>, new() where T : PrefabModelDataTable<M, int, T>.PrefabModelDataRecord, new()
    {
        public override MultiTableIndexer<int, PrefabModelDataRoot.PrefabModelDataType, PrefabModelDataRecordBridge> GetMultiGameIndex()
        {
            return PrefabModelDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }
    }
}