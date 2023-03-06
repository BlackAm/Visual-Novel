using System;
using Cysharp.Threading.Tasks;

namespace k514
{
    public class SystemLanguage : SystemTable<SystemLanguage, SystemLanguage.SystemLanguageType, SystemLanguage.SystemLanguageRecord>
    {
        public class SystemLanguageRecord : SystemTableRecordBase, IIndexableLanguageRecordBridge
        {
            public string content { get; private set; }

            public override async UniTask SetRecord(SystemLanguageType p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                content = (string) p_RecordField[0];
            }
        }

        public enum SystemLanguageType
        {
            Patch_TryNetworkConnection,
            Patch_CompareVersion,
            Patch_DownloadFile,
            Patch_Terminate,
            
            Patch_Fail,
        }

#if UNITY_EDITOR
        /// <summary>
        /// 해당 테이블에 들어갈 기본 값을 테이블 컬렉션에 레코드 인스턴스로 추가하는 메서드
        /// </summary>
        protected override async UniTask GenerateDefaultRecordSet()
        {
            await base.GenerateDefaultRecordSet();
            
            var enumerator = SystemTool.GetEnumEnumerator<SystemLanguageType>(SystemTool.GetEnumeratorType.ExceptNone);
            foreach (var languageType in enumerator)
            {
                switch (languageType)
                {
                    case SystemLanguageType.Patch_TryNetworkConnection:
                        await AddRecord(languageType, "서버와 연결중입니다.");
                        break;
                    case SystemLanguageType.Patch_CompareVersion:
                        await AddRecord(languageType, "버전을 비교중입니다.");
                        break;
                    case SystemLanguageType.Patch_DownloadFile:
                        await AddRecord(languageType, "패치파일을 다운로드 중입니다.");
                        break;
                    case SystemLanguageType.Patch_Terminate:
                        await AddRecord(languageType, "패치모듈 종료중");
                        break;
                    case SystemLanguageType.Patch_Fail:
                        await AddRecord(languageType, "패치에 실패했습니다.");
                        break;
                }
            }
        }
#endif

        protected override string GetDefaultTableFileName()
        {
            return "SystemLanguage";

        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}