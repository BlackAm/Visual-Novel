using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public partial class TableBase<M, K, T>
    {
        /// <summary>
        /// 테이블 타입에 따라 테이블 파일을 로드하고, 컬렉션에 초기화 시키는 메서드
        /// </summary>
        private async UniTask LoadTable(bool p_InvokeFromSingletonInitiate)
        {
            switch (TableType)
            {
                case TableTool.TableType.WholeGameTable:
                case TableTool.TableType.SceneGameTable:
                case TableTool.TableType.SystemTable:
#if UNITY_EDITOR
                    if (SystemFlag.IsTableByteImageMode())
                    {
                        switch (TableSerializeType)
                        {
                            case TableTool.TableSerializeType.SerializeObjects:
                                await LoadTableByte(p_InvokeFromSingletonInitiate);
                                break;
                            case TableTool.TableSerializeType.SerializeString:
                                await LoadStringByte(p_InvokeFromSingletonInitiate);
                                break;
                            case TableTool.TableSerializeType.NoneSerialize:
                                await LoadTableText(p_InvokeFromSingletonInitiate);
                                break;
                        }
                    }
                    else
                    {
                        await LoadTableText(p_InvokeFromSingletonInitiate);
                    }
#else
                    switch (TableSerializeType)
                    {
                        case TableTool.TableSerializeType.SerializeObjects:
                            await LoadTableByte(p_InvokeFromSingletonInitiate);
                            break;
                        case TableTool.TableSerializeType.SerializeString:
                            await LoadStringByte(p_InvokeFromSingletonInitiate);
                            break;
                        case TableTool.TableSerializeType.NoneSerialize:
                            await LoadTableText(p_InvokeFromSingletonInitiate);
                            break;
                    }
#endif
                    break;
                case TableTool.TableType.EditorOnlyTable:
                    await LoadTableText(p_InvokeFromSingletonInitiate);
                    break;
            }
        }
    }
}