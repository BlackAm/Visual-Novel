#if UNITY_EDITOR
using Cysharp.Threading.Tasks;

namespace k514
{
    public class GlobalDefineTable : EditorModeOnlyGameData<GlobalDefineTable, GlobalDefineTable.GlobalDefineType, GlobalDefineTable.TableRecord>
    {
        #region <Enums>

        public enum GlobalDefineType
        {
            /// <summary>
            /// 타입 기본값
            /// </summary>
            None,
            
            /// <summary>
            /// 리소스 테이블을 시스템 테이블이 아닌 게임 테이블로서 다룸
            /// 게임 테이블로서 다룰시, 리소스 테이블은 에셋번들에 포함됨
            /// 시스템 테이블로서 다룰시, 리소스 테이블은 어플에 포함됨
            /// </summary>
            RESOURCE_LIST_TABLE_INTO_GAMETABLE,
            
            /// <summary>
            /// 방향키 입력이 교착상태가 되었을 때,(즉 왼쪽 방향키를 누르다가 오른쪽도 동시에 눌렀을 때)
            /// 해당 전처리기가 활성화된 경우 늦게 입력된쪽을 우선 처리한다.(즉, 오른쪽 방향키가 적용된다.)
            /// </summary>
            OVERRAP_DEADLOCK_ARROWTYPE,
            
            /// <summary>
            /// 테스트 등에 필요한 Gizmos 기능을 활성화한다.
            /// </summary>
            ON_GUI,
            
            /// <summary>
            /// 카메라 드래그 입력이 지속되는 동안 카메라 회전을 유지시킨다.
            /// </summary>
            KEEP_VIEW_DRAG_DIRECTION,
            
            /// <summary>
            /// 자동으로 리소스의 씬 폴더를 검색하여 씬 인덱스 테이블을 작성한다.
            /// </summary>
            AUTO_ASSEMBLE_SCENE_INDEX_TABLE,

            /// <summary>
            /// 해당 클라이언트를 서버 노드로 사용한다.
            /// </summary>
            SERVER_DRIVE,
            
#if SERVER_DRIVE
#else
            /// <summary>
            /// 포스트 프로세스 스택을 적용한다.
            /// </summary>
            APPLY_PPS,
#endif
        }

        #endregion

        #region <Class>

        public class TableRecord : EditorModeOnlyTableRecord
        {
            public string Description { get; private set; }

            public override async UniTask SetRecord(GlobalDefineType p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                Description = (string)p_RecordField[0];
            }
        }

        #endregion

        #region <Methods>

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "GlobalDefineTable";
        }

        protected override async UniTask GenerateDefaultRecordSet()
        {
            await base.GenerateDefaultRecordSet();
            
            if (SystemTool.TryGetEnumEnumerator<GlobalDefineType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var globalDefineType in o_Enumerator)
                {
                    await AddRecord(globalDefineType, string.Empty);
                }
            }
        }

        protected override async UniTask CheckMissedRecordSet()
        {
            await base.CheckMissedRecordSet();
            
            if (SystemTool.TryGetEnumEnumerator<GlobalDefineType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var globalDefineType in o_Enumerator)
                {
                    if (!HasKey(globalDefineType))
                    {
                        await AddRecord(globalDefineType, string.Empty);
                    }
                }
            }
        }
        
        #endregion
    }
}

#endif