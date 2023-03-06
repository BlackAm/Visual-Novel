namespace k514
{
    public class UnitHitStuckData : GameTable<UnitHitStuckData, int, UnitHitStuckData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            /// <summary>
            /// * 스턱 플래그가 존재하는 경우에만 동작하는 레코드 필드
            /// 경직 타입
            /// </summary>
            public UnitHitTool.HitStuckType StuckType;
        
            /// <summary>
            /// * 스턱 플래그가 존재하는 경우에만 동작하는 레코드 필드
            /// 경직길이
            /// </summary>
            public uint StuckDurationCount;
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitHitStuckTable";
        }
    }
}