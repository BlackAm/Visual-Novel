using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// SceneSettingData의 [Key, VariableIndex] 슈퍼키를 제공하여 레코드에 접근하도록 돕는 테이블 
    /// </summary>
    public class SceneEntryData : GameTable<SceneEntryData, int, SceneEntryData.TableRecord>
    {
        #region <Callbacks>

        #endregion
        
        public class TableRecord : GameTableRecordBase
        {
            /// <summary>
            /// SceneDescription
            /// </summary>
            public string SceneDescription { get; private set; }
            
            /// <summary>
            /// 각 씬에 대응하는 유니크 키 값
            /// </summary>
            public int SceneSettingIndex { get; private set; }
            
            /// <summary>
            /// 각 씬 전용으로 적용가능한 Variable 리스트에서, 어떤 Variable을 적용할지에 대한 List Index 값
            /// </summary>
            public int SceneVariableListIndex { get; private set; }

            /// <summary>
            /// 해당 설정으로 인한 진입씬이 멀티플레이를 지원하는지 검증하는 논리메서드
            /// </summary>
            public bool IsSupportMultiPlay()
            {
                var sceneSettingRecord = SceneSettingData.GetInstanceUnSafe[SceneSettingIndex][SceneVariableListIndex];
                if (ReferenceEquals(null, sceneSettingRecord))
                {
#if UNITY_EDITOR
                    Debug.LogError($"[SceneEntry] => {KEY}, SceneSettingTable {SceneSettingIndex}레코드.SceneVariableList 의 {SceneVariableListIndex}번 인덱스가 유효하지 않음.");
#endif
                    return false;
                }
                else
                {
                    return sceneSettingRecord.SceneVariableFlagMask.HasAnyFlagExceptNone(SceneDataTool.SceneVariablePropertyType.SupportMultiPlay);
                }
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneEntryIndex";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}