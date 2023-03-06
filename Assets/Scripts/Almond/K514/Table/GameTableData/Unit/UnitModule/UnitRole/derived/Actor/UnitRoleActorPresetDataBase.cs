namespace k514
{
    public abstract class UnitRoleActorPresetDataBase<M, T> : UnitRolePresetDataBase<M, T>, IActorTableBridge
        where M : UnitRoleActorPresetDataBase<M, T>, new()
        where T : UnitRoleActorPresetDataBase<M, T>.ActorTableBaseRecord, new()
    {
        public abstract class ActorTableBaseRecord : RoleTableBaseRecord, IActorTableRecordBridge
        {
            public VolitionalTool.ActorModuleProgressFlag ActorModuleProgressFlag { get; protected set; }
            public VolitionalTool.ActorTimeOverEventType ActorTimeOverEventType { get; protected set; }
            public float LifeSpan { get; protected set; }
        }
    }
}