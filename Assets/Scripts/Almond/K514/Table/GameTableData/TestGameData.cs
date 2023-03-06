using Cysharp.Threading.Tasks;

namespace k514
{
    public class TestGameData : GameTable<TestGameData, int, TestGameData.TestGameDataInstance>
    {
        public class TestGameDataInstance : GameTableRecordBase
        {
            private string value1;
            public int value2 { get; set; }
            private float value3;

            public override async UniTask SetRecord(int p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                value2 = (int) p_RecordField[0];
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "Test";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
