namespace k514
{
    public class AnimationControllerData : GameTable<AnimationControllerData, int, AnimationControllerData.AnimationControllerGameDataRecord>
    {
        public class AnimationControllerGameDataRecord : GameTableRecordBase
        {
            public string AnimationControllerName { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "AnimationControllerTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}