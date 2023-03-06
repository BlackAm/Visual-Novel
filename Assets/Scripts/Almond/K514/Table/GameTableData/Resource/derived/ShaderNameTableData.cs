#if !SERVER_DRIVE
using UnityEngine;

namespace k514
{
    public class ShaderNameTableData : BaseResourceNameTable<ShaderNameTableData, RenderableTool.ShaderControlType, ShaderNameTableData.TableRecord, Shader>
    {
        public class TableRecord : BaseTableRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "ShaderNameTable";
        }
    }
}
#endif