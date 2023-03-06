namespace k514
{
    public class UnitEvolveTreeData : GameTable<UnitEvolveTreeData, int, UnitEvolveTreeData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int NeedLevelUp { get; private set; }
            public int EvolveTo { get; private set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitEvolveTreeTable";
        }
    }
}