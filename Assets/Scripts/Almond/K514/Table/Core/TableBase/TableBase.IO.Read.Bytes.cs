using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public partial class TableBase<M, K, T>
    {
        #region <Methods>
        
        public string GetByteTableFullPath()
        {
            return GetTableFileRootPath(AssetLoadType.FromUnityResource, PathType.SystemGenerate_AbsolutePath,
                       TableTool.TableFileType.Bytes) + GetTableFileName(TableTool.TableNameType.Alter, false) + TableTool.BYTES_EXT;
        }
        
        public string GetByteTableRelativePath()
        {
            return GetTableFileRootPath(AssetLoadType.FromUnityResource, PathType.SystemGenerate_RelativePath,
                       TableTool.TableFileType.Bytes) + GetTableFileName(TableTool.TableNameType.Alter, false);
        }
        
        /// <summary>
        /// LoadAssetManager 에 의해 정해진 경로로부터 테이블 바이트 파일을 탐색하여 읽는 메서드
        /// </summary>
        private async UniTask LoadStringByte(bool p_InvokeFromSingletonInitiate)
        {
            if (p_InvokeFromSingletonInitiate)
            {
                CheckTable();
            }
            else
            {
                ClearTable();
            }
            
            var tableFileType = GetTableFileType();
            switch (tableFileType)
            {
                case TableTool.TableFileType.Xml:
                {
                    await UniTask.SwitchToMainThread();

                    var tryTextAsset =
                        TableType == TableTool.TableType.SystemTable ?
                            await SystemTool.LoadAsync<TextAsset>(GetByteTableRelativePath())
                            : (
                                await LoadAssetManager.GetInstanceUnSafe
                                .LoadAssetAsync<TextAsset>(ResourceType.Table, ResourceLifeCycleType.Scene, GetTableFileName(TableTool.TableNameType.Alter, false) + TableTool.BYTES_EXT)
                            )
                            .Item2;
                    
                    if (ReferenceEquals(null, tryTextAsset))
                    {
                        await LoadTableText(p_InvokeFromSingletonInitiate);
                    }
                    else
                    {
                        var byteImage = tryTextAsset.bytes.DeserializeObject<Dictionary<K,Dictionary<string, string>>>();
                        await TableLoader.DecodeTable(this, byteImage, GetTable()).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                        await OnTableLoaded(p_InvokeFromSingletonInitiate);
                    }
                }
                    break;
                default :
                    break;
            }
        }
        
        /// <summary>
        /// LoadAssetManager 에 의해 정해진 경로로부터 테이블 바이트 파일을 탐색하여 읽는 메서드
        /// </summary>
        private async UniTask LoadTableByte(bool p_InvokeFromSingletonInitiate)
        {
            if (p_InvokeFromSingletonInitiate)
            {
                CheckTable();
            }
            else
            {
                ClearTable();
            }
            
            var tableFileType = GetTableFileType();
            switch (tableFileType)
            {
                case TableTool.TableFileType.Xml:
                {
                    await UniTask.SwitchToMainThread();
                    
                    var tryTextAsset =
                        TableType == TableTool.TableType.SystemTable ?
                            await SystemTool.LoadAsync<TextAsset>(GetByteTableRelativePath())
                            : (
                                await LoadAssetManager.GetInstanceUnSafe
                                .LoadAssetAsync<TextAsset>(ResourceType.Table, ResourceLifeCycleType.Scene, GetTableFileName(TableTool.TableNameType.Alter, false) + TableTool.BYTES_EXT)
                            )
                            .Item2;
                    
                    if (ReferenceEquals(null, tryTextAsset))
                    {
                        await LoadTableText(p_InvokeFromSingletonInitiate);
                    }
                    else
                    {
                        var byteImage = tryTextAsset.bytes.DeserializeObject<Dictionary<K, T>>();
                        await ReplaceTable(byteImage);
                    }
                }
                    break;
                default :
                    break;
            }
        }
        
        #endregion
    }
}
