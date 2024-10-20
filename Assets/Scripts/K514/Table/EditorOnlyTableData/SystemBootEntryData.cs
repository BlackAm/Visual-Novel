#if UNITY_EDITOR
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    public class SystemBootEntryData : EditorModeOnlyGameData<SystemBootEntryData, SystemEntry.SystemBootEntryMode, SystemBootEntryData.TableRecord>
    {
        public class TableRecord : EditorModeOnlyTableRecord
        {
            public SystemEntry.SystemBootEntryPreset SystemEntryPreset { get; private set; }
            
            public override async UniTask SetRecord(SystemEntry.SystemBootEntryMode p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                SystemEntryPreset = (SystemEntry.SystemBootEntryPreset) p_RecordField[0];
            }
        }

        /// <summary>
        /// 해당 테이블에 들어갈 기본 값을 테이블 컬렉션에 레코드 인스턴스로 추가하는 메서드
        /// </summary>
        protected override async UniTask GenerateDefaultRecordSet()
        {
            await base.GenerateDefaultRecordSet();
            
            if (SystemTool.TryGetEnumEnumerator<SystemEntry.SystemBootEntryMode>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var modeType in o_Enumerator)
                {
                    switch (modeType)
                    {
                        case SystemEntry.SystemBootEntryMode.SingleNetwork : 
                            await AddRecord(modeType, new SystemEntry.SystemBootEntryPreset(SystemEntry.SystemBootEntryMode.SingleNetwork, SystemEntry.DEFAULT_ENTRY_INDEX));
                            break; 
                        case SystemEntry.SystemBootEntryMode.MultiNetwork : 
                            await AddRecord(modeType, new SystemEntry.SystemBootEntryPreset(SystemEntry.SystemBootEntryMode.MultiNetwork));
                            break; 
                        case SystemEntry.SystemBootEntryMode.MultiNetworkForTest : 
                            await AddRecord(modeType, new SystemEntry.SystemBootEntryPreset(SystemEntry.SystemBootEntryMode.MultiNetworkForTest, SystemEntry.DEFAULT_ENTRY_INDEX, SystemEntry.DEFAULT_ENTRY_ID, SystemEntry.DEFAULT_ENTRY_PW));
                            break; 
                    }
                }
            }
        }
        
        protected override async UniTask CheckMissedRecordSet()
        {
            await base.CheckMissedRecordSet();

            if (SystemTool.TryGetEnumEnumerator<SystemEntry.SystemBootEntryMode>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var modeType in o_Enumerator)
                {
                    if (!HasKey(modeType))
                    {
                        switch (modeType)
                        {
                            case SystemEntry.SystemBootEntryMode.SingleNetwork : 
                                await AddRecord(modeType, new SystemEntry.SystemBootEntryPreset(SystemEntry.SystemBootEntryMode.SingleNetwork, SystemEntry.DEFAULT_ENTRY_INDEX));
                                break; 
                            case SystemEntry.SystemBootEntryMode.MultiNetwork : 
                                await AddRecord(modeType, new SystemEntry.SystemBootEntryPreset(SystemEntry.SystemBootEntryMode.MultiNetwork));
                                break; 
                            case SystemEntry.SystemBootEntryMode.MultiNetworkForTest : 
                                await AddRecord(modeType, new SystemEntry.SystemBootEntryPreset(SystemEntry.SystemBootEntryMode.MultiNetworkForTest, SystemEntry.DEFAULT_ENTRY_INDEX, SystemEntry.DEFAULT_ENTRY_ID, SystemEntry.DEFAULT_ENTRY_PW));
                                break; 
                        }
                    }
                }
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "SystemBootEntryDataTable";

        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif