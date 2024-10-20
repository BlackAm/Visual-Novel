#if UNITY_EDITOR

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class TableBase<M, K, T>
    {
        #region <Callbacks>

        private async UniTask TryWriteByteCode()
        {
            if (TableSerializeType == TableTool.TableSerializeType.SerializeObjects)
            {
                switch (TableType)
                {
                    case TableTool.TableType.WholeGameTable:
                    case TableTool.TableType.SceneGameTable:
                        await UniTask.SwitchToMainThread();
                        if (Application.isPlaying)
                        {
                            goto case TableTool.TableType.SystemTable;
                        }
                        break;
                    case TableTool.TableType.SystemTable:
                        GetTable().SerializeObject(GetByteTableFullPath());
                        await ResourceTracker.GetInstanceUnSafe.ReplaceRecord(GetByteTableRelativePath(), typeof(TextAsset), false);
                        break;
                    case TableTool.TableType.EditorOnlyTable:
                        break;
                }
            }
        }

        public async UniTask TryWriteByteCode<KeyType>(Dictionary<KeyType, Dictionary<string, string>> p_Table)
        {
            if (TableSerializeType == TableTool.TableSerializeType.SerializeString)
            {
                switch (TableType)
                {
                    case TableTool.TableType.WholeGameTable:
                    case TableTool.TableType.SceneGameTable:
                        await UniTask.SwitchToMainThread();
                        if (Application.isPlaying)
                        {
                            goto case TableTool.TableType.SystemTable;
                        }
                        break;
                    case TableTool.TableType.SystemTable:
                        p_Table.SerializeObject(GetByteTableFullPath());
                        await ResourceTracker.GetInstanceUnSafe.ReplaceRecord(GetByteTableRelativePath(), typeof(TextAsset), false);
                        break;
                    case TableTool.TableType.EditorOnlyTable:
                        break;
                }
            }
        }

        private async UniTask TryWriteByteCodeWhenUpdateMonoTable()
        {
            switch (TableSerializeType)
            {
                case TableTool.TableSerializeType.SerializeObjects:
                    await TryWriteByteCode();
                    await ResourceTracker.GetInstanceUnSafe.UpdateTableFile(ExportDataTool.WriteType.Overlap);
                    break;
                case TableTool.TableSerializeType.SerializeString:
                    var (valid, textAsset) = await TableLoader.ReadTableFile<K>(this);
                    if (valid)
                    {
                        var rawText = textAsset.text;
                        var parsedText = await TableLoader.ParsingTableFile<K>(GetInstanceUnSafe, rawText);
                        await TryWriteByteCode(parsedText);
                        await ResourceTracker.GetInstanceUnSafe.UpdateTableFile(ExportDataTool.WriteType.Overlap);
                    }
                    break;
                case TableTool.TableSerializeType.NoneSerialize:
                    break;
            }
        }
        
        #endregion
        
        /// <summary>
        /// 현재 테이블을 파일로 저장하는 메서드
        /// </summary>
        public async UniTask UpdateTableFile(ExportDataTool.WriteType p_WriteType)
        {
            var tableFullBranch = GetTableFileRootPath(this.GetAssetLoadType(), PathType.SystemGenerate_AbsolutePath);
            await UpdateTableFile(p_WriteType, tableFullBranch);
        }

        /// <summary>
        /// 지정한 절대 경로에 현재 테이블을 파일로 저장하는 메서드
        /// </summary>
        public async UniTask UpdateTableFile(ExportDataTool.WriteType p_WriteType, string p_TableAbsolutePath)
        {
            await TableModifier.UpdateTable<K, T>(this, GetTable(), p_WriteType, p_TableAbsolutePath);
            await TryWriteByteCodeWhenUpdateMonoTable();
        }
    }
}
#endif
