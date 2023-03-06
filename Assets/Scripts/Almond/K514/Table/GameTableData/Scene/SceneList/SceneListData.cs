using System.IO;
using Cysharp.Threading.Tasks;

namespace k514
{
    public abstract class CommonSceneListData<M, T> : MultiTableBase<M, int, T, SceneDataRoot.SceneDataType, IIndexableSceneDataRecordBridge>, ICommonSceneListData
        where M : CommonSceneListData<M, T>, new() 
        where T : CommonSceneListData<M, T>.SceneRecord, new()
    {
        public IIndexableSceneDataRecordBridge FirstRecord { get; private set; }

        public abstract class SceneRecord : GameTableRecordBase, IIndexableSceneDataRecordBridge
        {
            /// <summary>
            /// 확장자(*.unity)를 포함한 씬 이름
            /// </summary>
            public string SceneName { get; protected set; }

            public override async UniTask SetRecord(int p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                SceneName = (string)p_RecordField[0];
            }
        }

        protected override async UniTask OnCreated()
        {
             await base.OnCreated();
            
            // 배포모드가 아니라면, 각 씬 테이블을 업데이트 한다.
            if (!SystemMaintenance.IsPlayOnTargetPlatform())
            {
#if AUTO_ASSEMBLE_SCENE_INDEX_TABLE
                UpdateSceneTable();
#endif
            }

            for (int i = StartIndex; i < EndIndex; i++)
            {
                if (HasKey(i))
                {
                    FirstRecord = GetTableData(i);
                    break;
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 지정한 디렉터리의 씬 목록을 테이블에 업데이트하는 메서드
        /// </summary>
        protected async UniTask UpdateSceneTable()
        {
            ClearTable();
            
            var tableOriginIndex = StartIndex;
            var targetSceneDirectory = string.Concat(SystemMaintenance.GetSystemResourcePath(AssetLoadType.FromUnityResource,
                ResourceType.Scene, PathType.SystemGenerate_AbsolutePath), GetThisLabelType());
            var targetTableDirectory = string.Concat(TableTool.GetTablePath(AssetLoadType.FromUnityResource, PathType.SystemGenerate_AbsolutePath, 
                TableTool.TableType.WholeGameTable, TableTool.TableFileType.Xml), "Scene/SceneList/");
            
            if (!Directory.Exists(targetSceneDirectory))
            {
                Directory.CreateDirectory(targetSceneDirectory);
            }
            
            var directoryInfo = Directory.GetFiles(targetSceneDirectory);
            foreach (var fileName in directoryInfo)
            {
                if (!fileName.Contains(".meta") && !fileName.Contains(".lighting"))
                {
                    var SceneName = fileName.CutString("\\", false, false);
                    await AddRecord(tableOriginIndex++, SceneName);
                }
            }
            await UpdateTableFile(ExportDataTool.WriteType.Overlap, targetTableDirectory);
        }
#endif
        
        public override MultiTableIndexer<int, SceneDataRoot.SceneDataType, IIndexableSceneDataRecordBridge> GetMultiGameIndex()
        {
            return SceneDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }

        public int GetSceneIndex(string p_SceneName)
        {
            var targetTable = GetTable();
            var keyEnumerator = GetMultiGameIndex()[GetThisLabelType()].GetValidKeyEnumerator();
            foreach (var key in keyEnumerator)
            {
                var compareRecord = targetTable[key];
                if (!ReferenceEquals(null, compareRecord) && compareRecord.SceneName == p_SceneName)
                {
                    return key;
                }
            }

            return StartIndex;
        }
    }

    public class LoadingSceneListTableData : CommonSceneListData<LoadingSceneListTableData, LoadingSceneListTableData.SceneTableRecord>
    {
        public class SceneTableRecord : SceneRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneList100";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100;
            EndIndex = 200;
        }

        public override SceneDataRoot.SceneDataType GetThisLabelType()
        {
            return SceneDataRoot.SceneDataType.SystemHiddenScene;
        }
    }

    public class SceneList200TableData : CommonSceneListData<SceneList200TableData, SceneList200TableData.SceneTableRecord>
    {
        public class SceneTableRecord : SceneRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneList200";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 200;
            EndIndex = 300;
        }

        public override SceneDataRoot.SceneDataType GetThisLabelType()
        {
            return SceneDataRoot.SceneDataType.DungeonScene;
        }
    }
    
    public class BetaSceneListTableData : CommonSceneListData<BetaSceneListTableData, BetaSceneListTableData.SceneTableRecord>
    {
        public class SceneTableRecord : SceneRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneList300";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 300;
            EndIndex = 400;
        }

        public override SceneDataRoot.SceneDataType GetThisLabelType()
        {
            return SceneDataRoot.SceneDataType.SystemScene;
        }
    }
    
    public class SceneList400TableData : CommonSceneListData<SceneList400TableData, SceneList400TableData.SceneTableRecord>
    {
        public class SceneTableRecord : SceneRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneList400";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 400;
            EndIndex = 500;
        }

        public override SceneDataRoot.SceneDataType GetThisLabelType()
        {
            return SceneDataRoot.SceneDataType.MainScene;
        }
    }
    
    public class SceneListTestTableData : CommonSceneListData<SceneListTestTableData, SceneListTestTableData.SceneTableRecord>
    {
        public class SceneTableRecord : SceneRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneListTest";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 900;
            EndIndex = 1000;
        }

        public override SceneDataRoot.SceneDataType GetThisLabelType()
        {
            return SceneDataRoot.SceneDataType.TestScene;
        }
    }
}