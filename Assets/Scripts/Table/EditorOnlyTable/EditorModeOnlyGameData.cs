#if UNITY_EDITOR
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 실제 런타임에는 포함되지 않는 테이블
    /// </summary>
    public abstract class EditorModeOnlyGameData<M, K, T> : TableBase<M, K, T> where T : EditorModeOnlyGameData<M, K, T>.EditorModeOnlyTableRecord, new() where M : EditorModeOnlyGameData<M, K, T>, new()
    {
        public abstract class EditorModeOnlyTableRecord : TableRecordBase
        {
        }

        protected override async UniTask OnCreated()
        {
            // 시스템 테이블임을 표시한다.
            TableType = TableTool.TableType.EditorOnlyTable;
            TableSerializeType = TableTool.TableSerializeType.NoneSerialize;
     
            // 관련 디렉터리, 시스템 테이블이 없다면 기본값으로 생성한다.
            await CreateDefaultTable(false);

            // 테이블을 읽고 컬렉션을 초기화 시킨다.
            await base.OnCreated();
        }
    }
}

#endif