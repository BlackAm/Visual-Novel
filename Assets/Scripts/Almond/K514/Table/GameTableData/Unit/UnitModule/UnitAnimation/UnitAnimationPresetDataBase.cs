namespace k514
{
    public abstract class UnitAnimationPresetDataBase<M, T> : MultiTableBase<M, int, T, UnitAnimationDataRoot.AnimatableType, IAnimatableTableRecordBridge>, IAnimatableTableBridge
        where M : UnitAnimationPresetDataBase<M, T>, new()
        where T : UnitAnimationPresetDataBase<M, T>.AnimatableTableRecordBase, new()
    {
        public abstract class AnimatableTableRecordBase : GameTableRecordBase, IAnimatableTableRecordBridge
        {
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}