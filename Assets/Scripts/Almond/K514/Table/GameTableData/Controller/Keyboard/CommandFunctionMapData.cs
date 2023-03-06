namespace k514
{
    /// <summary>
    /// [커맨드 코드, 인풋 기능] 컬렉션
    /// ControlKeyMap의 반대처럼 보이지만, 해당 테이블은 어떤 입력이 있었을 때 이러한 기능이 호출된다는 것을 표현함.
    /// 따라서 해당 테이블을 통해 커스텀 키를 구현할 수 있다.
    /// </summary>
    public class CommandFunctionMapData : GameTable<CommandFunctionMapData, int, CommandFunctionMapData.ControllerGameDataInstance>
    {
        public class ControllerGameDataInstance : GameTableRecordBase
        {
            public ControllerTool.CommandType Value { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "CommandMap";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}