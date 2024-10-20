using System.Collections.Generic;
using System.Linq;

namespace BlackAm
{
    /*public interface ICommonUnitPartModelData : ITableBase
    {
        void InitPartModelToolList(
            Dictionary<(Vocation, UnitPartModelTool.LamiereUnitPart), List<IIndexableUnitPartModelDataRecordBridge>>
                p_PartTypeIndexedTable);
    }

    public interface IIndexableUnitPartModelDataRecordBridge : ITableBaseRecord
    {
        UnitPartModelTool.LamiereUnitPart TargetPartType { get; }
        string PrefabName { get; }
    }

    public class UnitPartModelData : GameTable<UnitPartModelData, int, UnitPartModelData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public UnitPartModelTool.LamiereUnitPart TargetPartType { get; private set; }
            public string PrefabName { get; private set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "EquipPartModelTable";
        }
    }*/

    /*public abstract class UnitPartModelData<M, T> :
        MultiTableBase<M, int, T, Vocation, IIndexableUnitPartModelDataRecordBridge>, ICommonUnitPartModelData
        where M : UnitPartModelData<M, T>, new()
        where T : UnitPartModelData<M, T>.UnitPartModelRecordBase, new()
    {
        public abstract class UnitPartModelRecordBase : GameTableRecordBase, IIndexableUnitPartModelDataRecordBridge
        {
            public UnitPartModelTool.LamiereUnitPart TargetPartType { get; protected set; }
            public string PrefabName { get; protected set; }
        }

        public override MultiTableIndexer<int, Vocation, IIndexableUnitPartModelDataRecordBridge> GetMultiGameIndex()
        {
            return UnitPartModelTool.GetInstanceUnSafe.GameDataTableCluster;
        }

        public void InitPartModelToolList(Dictionary<(Vocation, UnitPartModelTool.LamiereUnitPart), List<IIndexableUnitPartModelDataRecordBridge>> p_PartTypeIndexedTable)
        {
            foreach (var record in GetTable())
            {
                p_PartTypeIndexedTable[(GetThisLabelType(), record.Value.TargetPartType)].Add(record.Value);
            }       
        }
    }

    public class KnightUnitPartModelTableData : UnitPartModelData<KnightUnitPartModelTableData,
        KnightUnitPartModelTableData.UnitPartModelRecord>
    {
        public class UnitPartModelRecord : UnitPartModelRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "KnightPartModelTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 22000;
            EndIndex = 23000;
        }

        public override Vocation GetThisLabelType()
        {
            return Vocation.KNIGHT;
        }
    }
    
    public class ArcherUnitPartModelTableData : UnitPartModelData<ArcherUnitPartModelTableData,
        ArcherUnitPartModelTableData.UnitPartModelRecord>
    {
        public class UnitPartModelRecord : UnitPartModelRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "ArcherPartModelTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 23000;
            EndIndex = 24000;
        }

        public override Vocation GetThisLabelType()
        {
            return Vocation.ARCHER;
        }
    }
        
    public class MageUnitPartModelTableData : UnitPartModelData<MageUnitPartModelTableData,
        MageUnitPartModelTableData.UnitPartModelRecord>
    {
        public class UnitPartModelRecord : UnitPartModelRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "MagePartModelTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 24000;
            EndIndex = 25000;
        }

        public override Vocation GetThisLabelType()
        {
            return Vocation.MAGICIAN;
        }
    }

    public class AnithingPartModelTableData : UnitPartModelData<AnithingPartModelTableData,
        AnithingPartModelTableData.UnitPartModelRecord>
    {
        public class UnitPartModelRecord : UnitPartModelRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "AnythingPartModelTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 25000;
            EndIndex = 26000;
        }

        public override Vocation GetThisLabelType()
        {
            return Vocation.ANYTHING;
        }
    }*/
}