using System.Collections.Generic;

namespace k514
{
    public class UnitAttachPointData : GameTable<UnitAttachPointData, int, UnitAttachPointData.TableRecord>
    {
        #region <Record>

        public class TableRecord : GameTableRecordBase
        {
            public Dictionary<Unit.UnitAttachPoint, List<string>> AttachPointNameMap { get; private set; }
        }

        #endregion

        #region <Methods>

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitAttachPointTable";
        }

        #endregion
    }
}