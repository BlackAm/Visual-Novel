using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    public interface IDialogueTable<ModuleType> where ModuleType : struct
    {
        (bool, ModuleType) GetModuleType(int p_TableIndex);
        (ModuleType, IDialogue) SpawnModule(DialogueGameManager p_MasterNode, int p_TableIndex);
        Dictionary<ModuleType, int> DefaultFallbackIndexTable { get; }
        (bool, int) GetDefaultFallbackIndex(ModuleType p_Type);
    }

    public abstract class DialogueModuleDataRootBase<This, Label, Table, Record> : MultiTableProxy<This, int, Label, Table, Record>, IDialogueTable<Label>
        where This : DialogueModuleDataRootBase<This, Label, Table, Record>, new()
        where Label : struct
        where Table : ITableBase
        where Record : ITableBaseRecord
    {
        #region <Fields>

        public Dictionary<Label, int> DefaultFallbackIndexTable { get; private set; }
        
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

        protected abstract DialogueModuleDataTool.DialogueModuleType GetDialogueModuleType();
        
        protected override MultiTableIndexer<int, Label, Record> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<Label, Record>();
        }

        public (bool, Label) GetModuleType(int p_TableIndex)
        {
            return GetLabelType(p_TableIndex);
        }

        public abstract (Label, IDialogue) SpawnModule(DialogueGameManager _MasterNode, int p_TableIndex);
        
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

    public static class DialogueModuleDataTool
    {
        #region <Enums>

        public enum DialogueModuleType
        {
            KeyInput,
            DialoguePlayMode,
        }
        
        #endregion

        #region <Methods>

        public static IDialogueTable<Label> GetDialogueTable<Label>(DialogueModuleType p_Type) where Label : struct
        {
            switch (p_Type)
            {
                case DialogueModuleType.KeyInput:
                    return (IDialogueTable<Label>) DialogueKeyInputDataRoot.GetInstanceUnSafe;
                case DialogueModuleType.DialoguePlayMode:
                    return (IDialogueTable<Label>) DialoguePlayModeDataRoot.GetInstanceUnSafe;
            }
            
            return default;
        }

        #endregion
    }
}