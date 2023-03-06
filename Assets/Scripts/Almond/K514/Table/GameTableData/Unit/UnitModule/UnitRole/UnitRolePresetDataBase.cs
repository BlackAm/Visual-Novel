namespace k514
{
    public abstract class UnitRolePresetDataBase<M, T> : MultiTableBase<M, int, T, UnitRoleDataRoot.UnitRoleType, IVolitionalTableRecordBridge>, IVolitionalTableBridge
        where M : UnitRolePresetDataBase<M, T>, new()
        where T : UnitRolePresetDataBase<M, T>.RoleTableBaseRecord, new()
    {
        public abstract class RoleTableBaseRecord : GameTableRecordBase, IVolitionalTableRecordBridge
        {
            public float UnitScale { get; protected set; }
            public float BaseStatusAdditiveRate { get; protected set; }
            public int ExtraBaseStatusIndex { get; protected set; }
            public int ExtraBattleStatusIndex { get; protected set; }
        }

        public override MultiTableIndexer<int, UnitRoleDataRoot.UnitRoleType, IVolitionalTableRecordBridge> GetMultiGameIndex()
        {
            return UnitRoleDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }
        
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}