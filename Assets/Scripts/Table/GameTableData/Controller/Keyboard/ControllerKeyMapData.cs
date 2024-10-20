using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// [트리거 버튼, 인풋 식별자] 컬렉션
    /// </summary>
    public class ControllerKeyMapData : GameTable<ControllerKeyMapData, KeyCode, ControllerKeyMapData.ControllerGameDataInstance>
    {
        /// <summary>
        ///  방향키는 1~4의 인덱스를 지님
        /// </summary>
        public const int ARROW_KEY_UPPERBOUND = 5;
        
        /// <summary>
        /// 커맨드 키는 방향키를 포함하여 1 ~ 9의 인덱스를 지님
        /// </summary>
        public const int COMMAND_KEY_UPPERBOUND = 10;
        
        public class ControllerGameDataInstance : GameTableRecordBase
        {
            public int Value { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ControlKeyMap";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}