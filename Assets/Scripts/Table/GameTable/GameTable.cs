using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 실제 런타임에는 포함되지 않는 테이블
    /// </summary>
    public abstract class GameTable<M, K, T> : TableBase<M, K, T> where T : GameTable<M, K, T>.GameTableRecordBase, new() where M : GameTable<M, K, T>, new()
    {
        public abstract class GameTableRecordBase : TableRecordBase
        {
        }

        protected override async UniTask OnCreated()
        {
            // 게임 테이블 타입을 설정한다.
            TableType = TableTool.TableType.WholeGameTable;
            
            // 테이블을 읽고 컬렉션을 초기화 시킨다.
            await base.OnCreated();
        }
    }
}