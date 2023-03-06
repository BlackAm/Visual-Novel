using System.Collections.Generic;

namespace k514
{
    public class PhysicsCollisionLayerData : GameTable<PhysicsCollisionLayerData, GameManager.GameLayerType, PhysicsCollisionLayerData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public List<GameManager.GameLayerType> LayerBasedCollisionDetectionBlockTable { get; private set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "PhysicsCollisionLayerTable";
        }
    }
}