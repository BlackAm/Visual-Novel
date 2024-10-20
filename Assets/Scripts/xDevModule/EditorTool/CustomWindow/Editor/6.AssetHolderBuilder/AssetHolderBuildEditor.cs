using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    public class AssetHolderBuildEditor : CustomEditorWindowBase<AssetHolderBuildEditor>
    {
        #region <Fields>
        #endregion

        #region <Enums>
        #endregion
        
        #region <Callbacks>
        
        protected override void OnCreated()
        {
            SingletonTool.CreateSingleton(typeof(AssetHolderBuilder));
        }

        protected override void OnInitiate()
        {
        }

        #endregion
        
        #region <EditorWindow>

        [MenuItem(MenuHeader + "6. AssetHolderBuilder")]
        private static async void Init()
        {
            await InitWindow(0.17f, 1f);
        }

        protected override async void OnDrawEditor()
        {
            _DrawBlockFlag = true;

            if (EditorWindowTool.ConditionalButton("Asset Holder 갱신", true))
            {
                AssetHolderBuilder.GetInstance.UpdateAssetHolder();
            }
            
            if (EditorWindowTool.ConditionalButton("Asset Holder 초기화", true))
            {
                AssetHolderBuilder.GetInstance.InitAssetHolder();
            }
            
            if (EditorWindowTool.ConditionalButton("Resource Tracker 테이블 초기화", true))
            {
                ResourceTracker.GetInstanceUnSafe.ClearTable();
                await ResourceTracker.GetInstanceUnSafe.UpdateTableFile(ExportDataTool.WriteType.Overlap);
                await SystemFlag.GetInstanceUnSafe.UpdateSystemFlagReleaseMode(SystemFlag.SystemFlagType.UsingByteByteCode, false);
                AssetDatabase.Refresh();
                goto SEG_PAINT_END;
            }
            
            SEG_PAINT_END :
            _DrawBlockFlag = false;
        }

        
        #endregion
    }
}