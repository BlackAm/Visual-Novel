using System;

namespace k514
{
    public interface IRenderableTableBridge : ITableBase
    {
    }    
    
    public interface IRenderableTableRecordBridge : ITableBaseRecord
    {
        int KEY { get; }
        RenderableTool.ShaderControlType ShaderTypeMask { get; }
    }

    public class UnitRenderDataRoot : UnitModuleDataRootBase<UnitRenderDataRoot, UnitRenderDataRoot.UnitRenderType, IRenderableTableBridge, IRenderableTableRecordBridge>
    {
        #region <Enums>

        public enum UnitRenderType
        {
            None,
            Default,
        }

        #endregion

        #region <Methods>

        protected override UnitModuleDataTool.UnitModuleType GetUnitModuleType()
        {
            return UnitModuleDataTool.UnitModuleType.Render;
        }

        public override (UnitRenderType, IIncarnateUnit) SpawnModule(Unit p_Target, int p_TableIndex)
        {
            /*var (valid, labelType) = GetLabelType(p_TableIndex);
            if (valid)
            {
                var record = this[p_TableIndex];
                switch (labelType)
                {
                    case UnitRenderType.Default:
                        return (labelType, new DefaultRenderable().OnInitializeRender(labelType, p_Target, record));
                }
            }*/

            return default;
        }
        #endregion
    }
}