using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public class ObjectDeployPresetMapData : GameTable<ObjectDeployPresetMapData, int, ObjectDeployPresetMapData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public ObjectDeployEventPreset DeployEventPresetMap { get; private set; }
            public int MaxRecursiveCount { get; private set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                MaxRecursiveCount = Mathf.Max(1, MaxRecursiveCount);
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "ObjectDeployPresetMapDataTable";
        }
    }
}