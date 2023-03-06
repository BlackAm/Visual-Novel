namespace k514
{
    public abstract class UnitRenderPresetDataBase<M, T> : MultiTableBase<M, int, T, UnitRenderDataRoot.UnitRenderType, IRenderableTableRecordBridge>, IRenderableTableBridge
        where M : UnitRenderPresetDataBase<M, T>, new()
        where T : UnitRenderPresetDataBase<M, T>.RenderableTableRecordBase, new()
    {
        public abstract class RenderableTableRecordBase : GameTableRecordBase, IRenderableTableRecordBridge
        {
            /// <summary>
            /// 해당 셰이더 모듈이 전이할 수 없는 셰이더 타입
            /// </summary>
            public RenderableTool.ShaderControlType ShaderTypeMask { get; protected set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}