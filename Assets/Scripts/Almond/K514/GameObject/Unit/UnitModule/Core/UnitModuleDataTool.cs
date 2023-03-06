using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace k514
{
    public interface IIncarnateUnitTable<ModuleType> where ModuleType : struct
    {
        (bool, ModuleType) GetModuleType(int p_TableIndex);
        (ModuleType, IIncarnateUnit) SpawnModule(Unit p_Target, int p_TableIndex);
        Dictionary<ModuleType, int> DefaultFallbackIndexTable { get; }
        (bool, int) GetDefaultFallbackIndex(ModuleType p_Type);
    }
    
    public abstract class UnitModuleDataRootBase<This, Label, Table, Record> : MultiTableProxy<This, int, Label, Table, Record>, IIncarnateUnitTable<Label> 
        where This : UnitModuleDataRootBase<This, Label, Table, Record>, new() 
        where Label : struct
        where Table : ITableBase
        where Record : ITableBaseRecord
    {
        #region <Fields>

        public Dictionary<Label, int> DefaultFallbackIndexTable { get; private set; }
        public UnitModuleDataTool.UnitModuleType UnitModuleType { get; protected set; }
        
        #endregion
        
        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            await base.OnCreated();
                        
            DefaultFallbackIndexTable = new Dictionary<Label, int>();
            var enumerator = SystemTool.GetEnumEnumerator<Label>(SystemTool.GetEnumeratorType.ExceptNone);
     
            foreach (var labelType in enumerator)
            {
                var partialTable = this[labelType];
                if (!ReferenceEquals(null, partialTable))
                {
                    DefaultFallbackIndexTable.Add(labelType, partialTable.GetValidKeyEnumerator().First());
                }
            }
        }

        #endregion
        
        #region <Methods>

        protected abstract UnitModuleDataTool.UnitModuleType GetUnitModuleType();
        
        protected override MultiTableIndexer<int, Label, Record> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<Label, Record>();
        }

        public (bool, Label) GetModuleType(int p_TableIndex)
        {
            return GetLabelType(p_TableIndex);
        }

        public abstract (Label, IIncarnateUnit) SpawnModule(Unit p_Target, int p_TableIndex);
        
        public (bool, int) GetDefaultFallbackIndex(Label p_Type)
        {
            if (DefaultFallbackIndexTable.TryGetValue(p_Type, out var o_Key))
            {
                return (true, o_Key);
            }
            else
            {
                return default;
            }
        }
        
        #endregion
    }

    public static class UnitModuleDataTool
    {
        #region <Enums>

        public enum UnitModuleType
        {
            Action,
            AI,
            Animation,
            Physics,
            Render,
            Role
        }

        #endregion

        #region <Methods>

        public static IIncarnateUnitTable<Label> GetIncarnateUnitTable<Label>(UnitModuleType p_Type) where Label : struct
        {
            switch (p_Type)
            {
                case UnitModuleType.Action:
                    return (IIncarnateUnitTable<Label>) UnitActionDataRoot.GetInstanceUnSafe;
                case UnitModuleType.AI:
                    return (IIncarnateUnitTable<Label>) UnitAIDataRoot.GetInstanceUnSafe;
                case UnitModuleType.Animation:
                    return (IIncarnateUnitTable<Label>) UnitAnimationDataRoot.GetInstanceUnSafe;
                case UnitModuleType.Physics:
                    return (IIncarnateUnitTable<Label>) UnitPhysicsDataRoot.GetInstanceUnSafe;
                case UnitModuleType.Render:
                    return (IIncarnateUnitTable<Label>) UnitRenderDataRoot.GetInstanceUnSafe;
                case UnitModuleType.Role:
                    return (IIncarnateUnitTable<Label>) UnitRoleDataRoot.GetInstanceUnSafe;
            }

            return default;
        }

        #endregion
    }
}