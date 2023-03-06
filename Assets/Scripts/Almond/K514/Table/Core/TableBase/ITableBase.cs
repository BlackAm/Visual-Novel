using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace k514
{
    public interface ITableBase : ISingleton
    {
        TableTool.TableType TableType { get; }
        TableTool.TableSerializeType TableSerializeType { get; }
        TableTool.TableFileType GetTableFileType();
        string GetTableFileName(TableTool.TableNameType p_Type, bool p_AttachExt);
#if UNITY_EDITOR
        UniTask OnUpdateTableFile();
        UniTask TryWriteByteCode<K>(Dictionary<K, Dictionary<string, string>> p_Table);
#endif
        string GetTableFileFullPath(AssetLoadType p_AssetLoadType, PathType p_PathType,
            TableTool.TableNameType p_NameType, bool p_AttachExt);
        string GetByteTableFullPath();
        string GetByteTableRelativePath();
    }
    
    public interface ITableBaseRecord
    {
        UniTask OnRecordDecoded();
    }
}