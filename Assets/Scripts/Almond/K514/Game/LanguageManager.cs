namespace k514
{
    public interface IIndexableLanguageTableBridge : ITableBase
    {
    }

    public interface IIndexableLanguageRecordBridge : ITableBaseRecord
    {
        string content { get; }
    }

    public class LanguageManager : MultiTableProxy<LanguageManager, int, LanguageManager.LanguageDataType, IIndexableLanguageTableBridge, IIndexableLanguageRecordBridge>
    {
        #region <Enums>

        public enum LanguageDataType
        {
            SystemLanguage,
            ItemLanguage,
            GamePropertyLanguage,
            ScenarioLanguage,
            GameContentLanguage,
            SkillPropertyLanguage,
            UILanguage
        }

        #endregion

        public static string GetContent(int id)
        {
            if (GetInstanceUnSafe.GameDataTableCluster.GetTableData(id, out var o_Content))
            {
                return o_Content.content;
            }
            else
            {
                return $"* {id} *";
            }
        }

        protected override MultiTableIndexer<int, LanguageDataType, IIndexableLanguageRecordBridge> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<LanguageDataType, IIndexableLanguageRecordBridge>();
        }
    }
}