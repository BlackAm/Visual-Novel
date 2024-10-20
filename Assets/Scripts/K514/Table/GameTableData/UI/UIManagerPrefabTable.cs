#if !SERVER_DRIVE
using System;

namespace BlackAm
{
    public class UIManagerPrefabTable : GameTable<UIManagerPrefabTable, UICustomRoot.UIManagerType, UIManagerPrefabTable.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public string ManagerPrefabName { get; private set; }
            public Type ManagerComponent { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UIManagerPrefabData";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif