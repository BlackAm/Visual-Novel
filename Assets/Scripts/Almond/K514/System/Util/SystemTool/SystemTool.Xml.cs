using System;
using System.Security;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public static partial class SystemTool
    {
        #region <Consts>

        /// <summary>
        /// 구조체나 클래스 등의 생성자를 Parsing하는 단위 심볼
        /// </summary>
        private const char MultiElementValueParser = ',';
        
        /// <summary>
        /// 구조체나 클래스 등의 생성자를 Parsing하는 단위 심볼
        /// </summary>
        private const char MultiElementValueLeftBracket = '[';
        
        /// <summary>
        /// 구조체나 클래스 등의 생성자를 Parsing하는 단위 심볼
        /// </summary>
        private const char MultiElementValueRightBracket = ']';

        /// <summary>
        /// 리스트의 Element를 Parsing하는 단위 심볼
        /// </summary>
        private const char ListRecordParser = '/';
        
        /// <summary>
        /// 컬렉션의 Element를 Parsing하는 단위 심볼
        /// </summary>
        private const char CollectionRecordParser = '\\';

        /// <summary>
        /// Enum Flag를 파싱하는 단위 심볼
        /// </summary>
        private const char EnumFlagParser = '|';
        
        /// <summary>
        /// 컬렉션의 KeyPair를 Parsing하는 단위 심볼
        /// </summary>
        private const char KVParser = '#';
        
        /// <summary>
        /// 논리 변수 타입의 참 값 alias 심볼
        /// </summary>
        private const string BoolTrueSymbol = "1";
        
        /// 논리 변수 타입의 거짓 값 alias 심볼
        private const string BoolFalseSymbol = "0";

        /// <summary>
        /// xml 테이블 파서 인스턴스
        /// </summary>
        public static SecurityParser securityParser = new SecurityParser();
        
        #endregion

        #region <Methods>

        /// <summary>
        /// 지정한 xml 직렬화 데이터를 SecurityElement 인스턴스로 디코드하여 리턴하는 메서드 
        /// </summary>
        public static SecurityElement TryParsingXml(this string p_XmlData)
        {
            securityParser.LoadXml(p_XmlData);
            return securityParser.ToXml();
        }
        
        /// <summary>
        /// 지정한 xml 직렬화 데이터를 SecurityElement 인스턴스로 디코드하여 리턴하는 메서드 
        /// </summary>
        public static async UniTask<SecurityElement> TryParsingXmlAsync(this string p_XmlData)
        {
            await UniTask.SwitchToThreadPool();
            securityParser.LoadXml(p_XmlData);
            return securityParser.ToXml();
        }

        #endregion
    }
}