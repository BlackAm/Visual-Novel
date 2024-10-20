using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 실제 런타임에는 포함되지 않는 테이블
    /// </summary>
    public abstract class SystemTable<M, K, T> : TableBase<M, K, T> where T : SystemTable<M, K, T>.SystemTableRecordBase, new() where M : SystemTable<M, K, T>, new()
    {
        public abstract class SystemTableRecordBase : TableRecordBase
        {
        }
        
        protected override async UniTask OnCreated()
        {
            // 게임 테이블 타입을 설정한다.
            TableType = TableTool.TableType.SystemTable;
#if UNITY_EDITOR
            // 관련 디렉터리, 시스템 테이블이 없다면 기본값으로 생성한다.
            await CreateDefaultTable(false);
#endif
            // 테이블을 읽고 컬렉션을 초기화 시킨다.
            await base.OnCreated();
        }
    }
}