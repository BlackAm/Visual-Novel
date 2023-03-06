using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using System;

namespace k514
{
    /// <summary>
    /// LoadAssetManager에 요청된 UnityResource 에셋 로드의 타입 및 에셋 이름을 기록하는 테이블
    /// </summary>   
    public class ResourceTracker : EditorModeOnlyGameData<ResourceTracker, string, ResourceTracker.TableRecord>
    {
        public class TableRecord : EditorModeOnlyTableRecord
        {
            public string AssetTypeName { get; private set; }
            public bool IsMultiAsset { get; private set; }

            public override async UniTask SetRecord(string p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                AssetTypeName = ((Type)p_RecordField[0]).Name;
                IsMultiAsset = (bool)p_RecordField[1];
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ResourceTracker";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif