using System;

namespace BlackAm
{
    public class SceneEnvironmentTypeMap : GameTable<SceneEnvironmentTypeMap, string, SceneEnvironmentTypeMap.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Type Type;
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneEnvironmentTypeTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
    
    public class SceneEnvironmentTypeIndexingMap : GameTable<SceneEnvironmentTypeIndexingMap, int, SceneEnvironmentTypeIndexingMap.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Type Type;
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneEnvironmentTypeIndexingTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}