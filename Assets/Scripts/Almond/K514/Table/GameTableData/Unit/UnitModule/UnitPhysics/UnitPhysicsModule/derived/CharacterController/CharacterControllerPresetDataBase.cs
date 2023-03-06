namespace k514
{
    public abstract class CharacterControllerPresetDataBase<M, T> : UnitPhysicsPresetDataBase<M, T>, ICharacterControllerTableBridge
        where M : CharacterControllerPresetDataBase<M, T>, new() 
        where T : CharacterControllerPresetDataBase<M, T>.CharacterControllerTableBaseRecord, new()
    {
        public abstract class CharacterControllerTableBaseRecord : PhysicsTableBaseRecord, ICharacterControllerTableRecordBridge
        {
        }
    }
}