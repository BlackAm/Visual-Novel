using Cysharp.Threading.Tasks;

namespace k514
{
    public class SystemValue : SystemTable<SystemValue, int, SystemValue.SystemValueRecord>
    {
        public static SystemValueRecord GetSystemValueRecord()
        {
            return GetInstanceUnSafe.GetTableData(0);
        }

        public class SystemValueRecord : SystemTableRecordBase
        {
            /// <summary>
            /// 동일한 커맨드 발생시 커맨드 추가 입력을 기다리는 시간
            /// </summary>
            public int CommandCooldown { get; private set; }

            /// <summary>
            /// 입력된 커맨드 유지 시간
            /// </summary>
            public int CommandExpireMsec { get; private set; }
            
            /// <summary>
            /// 컨트롤러의 방향키 입력시, 최대 방향키 기억 갯수
            /// </summary>
            public int CommandMaxCapacity { get; private set; }
            
            public override async UniTask SetRecord(int p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                CommandCooldown = (int) p_RecordField[0];
                CommandExpireMsec = (int) p_RecordField[1];
                CommandMaxCapacity = (int) p_RecordField[2];
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 해당 테이블에 들어갈 기본 값을 테이블 컬렉션에 레코드 인스턴스로 추가하는 메서드
        /// </summary>
        protected override async UniTask GenerateDefaultRecordSet()
        {
            await base.GenerateDefaultRecordSet();
            
            var targetTable = GetTable();
            var defaultKey = 0;
            if (!targetTable.ContainsKey(defaultKey))
            {
                await AddRecord(defaultKey, 100, 500, 4);
            }
        }
#endif

        protected override string GetDefaultTableFileName()
        {
            return "SystemValue";

        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}