using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlackAm
{
    public static partial class SystemTool
    {
        #region <Consts>

        public static readonly char[] SlashSymbolSet = {'/', '\\'};

        #endregion
        
        #region <Method/Bool>
        
        /// <summary>
        /// 문자열 리스트에 지정한 키워드를 포함한 원소가 있는지 검증하는 메서드
        /// </summary>
        public static bool SearchKeyWord(this string p_TargetString, List<string> p_KeywordSafeList)
        {
            foreach (var symbol in p_KeywordSafeList)
            {
                if (p_TargetString.Contains(symbol))
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 특정 문자열이 반각 숫자 0 ~ 9 로만 구성되어 있는지 검증하는 메서드
        /// </summary>
        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 지정한 문자열이 특정 문자를 포함하는지 검증하는 논리메서드
        /// </summary>
        public static bool ContainChar(this string p_TargetString, char p_Comparer)
        {
            return p_TargetString.IndexOf(p_Comparer) > -1;
        }
        
        /// <summary>
        /// 지정한 문자열의 시작과 끝이 두 심볼과 일치하는지 검증하는 메서드
        /// </summary>
        public static bool CheckPairFirstLast(this string p_TargetString, char p_LeftSymbol, char p_RightSymbol)
        {
            return p_TargetString.First() == p_LeftSymbol && p_TargetString.Last() == p_RightSymbol;
        }

        /// <summary>
        /// 문자열을 순회하여 지정한 두 심볼이 짝으로 포함되어 있는지 검증하는 메서드
        /// [] [] [] 를 참으로 리턴한다.
        /// </summary>
        public static bool CheckPairPaired(this string p_TargetString, char p_LeftSymbol, char p_RightSymbol)
        {
            var cnt = 0;
            foreach (var tryChar in p_TargetString)
            {
                if (tryChar == p_LeftSymbol)
                {
                    cnt++;
                }
                else if (tryChar == p_RightSymbol)
                {
                    if (cnt > 0)
                    {
                        cnt--;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return cnt == 0;
        }
        
        /// <summary>
        /// 문자열을 순회하여 지정한 두 심볼의 개폐 여부를 검증하는 메서드
        /// [] [] [] 는 거짓으로 리턴한다. [ [] [] ]는 참으로 리턴한다.
        /// </summary>
        public static bool CheckPairClosed(this string p_TargetString, char p_LeftSymbol, char p_RightSymbol)
        {
            var cnt = 0;
            var isClosed = false;
            foreach (var tryChar in p_TargetString)
            {
                if (tryChar == p_LeftSymbol)
                {
                    if (isClosed)
                    {
                        return false;
                    }
                    else
                    {
                        cnt++;
                    }
                }
                else if (tryChar == p_RightSymbol)
                {
                    if (isClosed)
                    {
                        return false;
                    }
                    else
                    {
                        if (cnt > 0)
                        {
                            cnt--;
                            if (cnt == 0)
                            {
                                isClosed = true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return cnt == 0;
        }
                
        /// <summary>
        /// 지정한 문자열의 시작과 끝이 두 심볼과 일치하는지 검증하는 메서드
        /// </summary>
        public static bool CheckPairFirstLastPaired(this string p_TargetString, char p_LeftSymbol, char p_RightSymbol)
        {
            return p_TargetString.First() == p_LeftSymbol && p_TargetString.Last() == p_RightSymbol;
        }
        
        /// <summary>
        /// 문자열로부터 지정한 두 심볼 사이 밖에서 특정 문자를 찾아 그 인덱스를 리턴하는 메서드
        /// </summary>
        public static int FindCharIndexBetweenBracket(this string p_TargetString, char p_LeftSymbol, char p_RightSymbol, char p_FindSymbol)
        {
            var cnt = 0;
            var length = p_TargetString.Length;
            for (int i = 0; i < length; i++)
            {
                var tryChar = p_TargetString[i];

                if (tryChar == p_LeftSymbol)
                {
                    if (cnt < 0)
                    {
                    }
                    else
                    {
                        cnt++;
                    }
                }
                else if (tryChar == p_RightSymbol)
                {
                    if (cnt > 0)
                    {
                        cnt--;
                    }
                    else
                    {
                    }
                }
                else if(tryChar == p_FindSymbol)
                {
                    if (cnt == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        
        /// <summary>
        /// 문자열로부터 컬렉션 파싱 심볼에 둘러싸이지 않은 특정 문자를 찾아 그 인덱스를 리턴하는 메서드
        /// </summary>
        public static int FindCharIndexBetweenCollectionParsingBracket(this string p_TargetString, char p_FindSymbol)
        {
            return p_TargetString.FindCharIndexBetweenBracket(MultiElementValueLeftBracket, MultiElementValueRightBracket, p_FindSymbol);
        }
        
        #endregion

        #region <Method/Trim>

        /// <summary>
        /// 제로 패딩을 수행하는 메서드
        /// </summary>
        public static string ZeroPadding(this string p_Target, int p_TotalCount) => p_Target.PadLeft(p_TotalCount, '0');

        /// <summary>
        /// 지정한 문자열에서 공백을 전부 제거한다.
        /// </summary>
        public static string RemoveSpace(this string p_TargetString)
        {
            return String.Concat(p_TargetString.Where(c => !Char.IsWhiteSpace(c)));
        }

        /// <summary>
        /// 지정한 경로의 '../' 와 같은 심볼을 적용시킨 경로를 리턴하는 메서드
        /// 해당 메서드를 사용하는 경우 경로의 파서는 슬래시로 변환된다.
        public static string TrimPathSymbol(this string p_TargetPath)
        {
            p_TargetPath = p_TargetPath.TurnToSlash();
            var parsingSymbol = "../";
            while (p_TargetPath.Contains(parsingSymbol))
            {
                var header = p_TargetPath.CutString(parsingSymbol, true, true).CutLastSlashGetForward().CutStringWithPivot("/",true, false);
                var rear = p_TargetPath.CutString(parsingSymbol, false, true);
                p_TargetPath = header + rear;
            }
            return p_TargetPath;
        }

        /// <summary>
        /// 지정한 문자열을 Trim한 이후에, 시작과 끝이 [, ] 인 경우 해당 문자도 제거해주는 메서드
        /// </summary>
        public static string TrimAndOffBracket(this string p_TargetString)
        {
            p_TargetString = p_TargetString.Trim();
            if (p_TargetString.CheckPairFirstLast(MultiElementValueLeftBracket, MultiElementValueRightBracket) 
                && p_TargetString.CheckPairClosed(MultiElementValueLeftBracket, MultiElementValueRightBracket))
            {
                return p_TargetString.SubstringFromTo(1, p_TargetString.Length - 2);
            }
            else
            {
                return p_TargetString;
            }
        }
                
        /// <summary>
        /// 지정한 문자열의 맨 앞, 뒤에 있는 특정 문자를 제거하는 메서드. 만약 해당 문자열이 심볼와 공백만으로 이루어져 있는 경우에는 공백을 리턴한다.
        /// </summary>
        public static string TrimFrontRearSymbol(this string p_TargetString, char p_Symbol)
        {
            return p_TargetString.TrimStart(' ', p_Symbol).TrimEnd(' ', p_Symbol);
        }

        /// <summary>
        /// 지정한 문자열의 맨 앞, 뒤에 있는 공백을 제거하는 메서드. 만약 해당 문자열이 심볼와 공백만으로 이루어져 있는 경우에는 공백을 리턴한다.
        /// </summary>
        public static string TrimFrontRearSpace(this string p_TargetString)
        {
            return p_TargetString.TrimStart(' ').TrimEnd(' ');
        }
        
        /// <summary>
        /// 지정한 문자열 리스트에 TrimFrontRearSymbol를 적용하여, 공백을 제거한 리스트를 리턴하는 메서드
        /// </summary>
        public static List<string> TrimSymbol(this List<string> p_TargetList, char p_Symbol)
        {
            var result = new List<string>();
            foreach (var str in p_TargetList)
            {
                var trimmedStr = str.TrimFrontRearSymbol(p_Symbol);
                if (trimmedStr != string.Empty)
                {
                    result.Add(trimmedStr);
                }
            }

            return result;
        }

        /// <summary>
        /// 지정한 문자열 배열에 TrimFrontRearSymbol를 적용하여, 공백을 제거한 리스트를 리턴하는 메서드
        /// </summary>
        public static List<string> TrimSymbol(this string[] p_TargetArray, char p_Symbol)
        {
            return p_TargetArray.ToList().TrimSymbol(p_Symbol);
        }
        
        /// <summary>
        /// 지정한 문자열 리스트 내부에 공백 문자열을 제거한 리스트를 리턴한다.
        /// </summary>
        public static List<string> TrimEmpty(this List<string> p_TargetList)
        {
            return p_TargetList.Where(element => element != string.Empty).ToList();
        }
        
        /// <summary>
        /// 지정한 문자열 배열 내부에 공백 문자열을 제거한 리스트를 리턴한다.
        /// </summary>
        public static List<string> TrimEmpty(this string[] p_TargetArray)
        {
            return p_TargetArray.ToList().TrimEmpty();
        }
        
        #endregion

        #region <Method/Parsing>

        /// <summary>
        /// 표준 SubString은 지정한 pivot으로부터 일정 length만큼의 문자열을 자르는 메서드.
        /// 해당 메서드는 지정한 두 pivot 사이에 있는 문자열을 잘라 리턴하는 메서드
        /// </summary>
        public static string SubstringFromTo(this string p_TargetString, int p_StartIndex, int p_EndIndex)
        {
            return p_TargetString.Substring(p_StartIndex, p_EndIndex - p_StartIndex + 1);
        }

        /// <summary>
        /// 지정한 문자열로부터 특정한 문자세트 [ ] 사이에 잇는 문자열을 잘라내는 메서드.
        /// 만약 해당 문자세트 [ ] 사이에 또 하나의 문자세트 [ ]가 있다고 하더라도, 항상 최외곽에 있는 세트를 기준으로만 잘라낸다.
        ///
        /// p_CutBracket 파라미터를 설정하는 것으로 파싱심볼을 제거할지 정할 수 있다.
        /// 
        /// p_ExtraCutCompare 파라미터를 설정하면, [ ] 외에도 , 문자를 기준으로도 문자열을 자른다.
        /// 해당 경우에도 항상 최외곽에 있는 [ ] 의 바깥쪽만 자른다. 해당 파라미터는 p_CutBracket의 영향을 받지 않는다.
        /// 또한, p_ApplyExtraCutComparer가 설정된 경우 p_ApplyExtraCutComparer가 먼저 탐색되기 전까지 Bracket은 무시된다.
        /// </summary>
        public static List<string> SplitBetweenBracket(this string p_TargetString, bool p_CutBracket, char p_ExtraCutCompare = ' ')
        {
            // 인덱스를 세어서 문자열을 자르므로, 공백에 민감하다. Trim이 아닌 공백 제거를 수행한다.
            p_TargetString = p_TargetString.RemoveSpace();
            
            var strLength = p_TargetString.Length;
            var strLastIndex = strLength - 1;
            var parseFlag = new bool[strLength];
            var pivot = 0;
            var bracketOpenFlag = false;
            var bracketStack = 0;
            var lastOpenedPivot = -1;
            var bracketParsingValidFlag = p_ExtraCutCompare == ' ';
            
            while (pivot < strLength)
            {
                var tryChar = p_TargetString[pivot];
                switch (tryChar)
                {
                    case MultiElementValueLeftBracket :
                        if (bracketParsingValidFlag)
                        {
                            if (bracketOpenFlag)
                            {
                                bracketStack++;
                            }
                            else
                            {
                                bracketOpenFlag = true;
                                lastOpenedPivot = pivot;
                                parseFlag[pivot] = p_CutBracket;
                            }
                        }
                        break;
                    case MultiElementValueRightBracket :
                        if (bracketParsingValidFlag)
                        {
                            if (bracketOpenFlag)
                            {
                                if (bracketStack > 0)
                                {
                                    bracketStack--;
                                }
                                else
                                {
                                    bracketOpenFlag = false;
                                    parseFlag[pivot] = p_CutBracket;
                                }
                            }
                        }
                        break;
                    default :
                        if (p_ExtraCutCompare == tryChar)
                        {
                            if (bracketOpenFlag)
                            {
                            }
                            else
                            {
                                bracketParsingValidFlag = true;
                                parseFlag[pivot] = true;
                            }
                        }
                        break;
                }

                pivot++;
            }

            if (bracketOpenFlag)
            {
                bracketOpenFlag = false;
                if (lastOpenedPivot > -1)
                {
                    parseFlag[lastOpenedPivot] = false;
                }
#if UNITY_EDITOR
                Debug.LogError($"'{p_TargetString}' : unfair left bracket set");
#endif
            }

            var result = new List<string>();
            var cutLeftPivot = 0;
            var cutRightPivot = strLastIndex;
            for (int i = 0; i < strLength; i++)
            {
                if (parseFlag[i])
                {
                    if (cutLeftPivot < i)
                    {
                        cutRightPivot = i;
                        result.Add(p_TargetString.SubstringFromTo(cutLeftPivot, cutRightPivot - 1));
                        cutLeftPivot = i + 1;
                    }
                    else
                    {
                        cutLeftPivot++;
                    }
                }
            }

            if (cutLeftPivot < strLength)
            {
                result.Add(p_TargetString.SubstringFromTo(cutLeftPivot, strLastIndex));
            }

            return result;
        }

        /// <summary>
        /// 문자열을 지정한 파싱 심볼 기준으로 자르는 메서드.
        /// 이 때, 파라미터로 받은 Left, Right Bracket안에 있는 심볼은 무시한다.
        /// </summary>
        public static List<string> SplitAvoidBracket(this string p_TargetString, char p_LeftBracketSymbol, char p_RightBracketSymbol, char p_FindSymbol)
        {
            var strLength = p_TargetString.Length;
            var strLastIndex = strLength - 1;
            var parseFlag = new bool[strLength];
            var isBracketOpen = 0;

            for (int i = 0; i < strLength; i++)
            {
                var tryChar = p_TargetString[i];

                if (tryChar == p_LeftBracketSymbol)
                {
                    isBracketOpen++;
                }
                else if (tryChar == p_RightBracketSymbol)
                {
                    isBracketOpen--;
                }
                else
                {
                    if (isBracketOpen < 1 && tryChar == p_FindSymbol)
                    {
                        parseFlag[i] = true;
                    }
                }
            }

            var result = new List<string>();
            var cutLeftPivot = 0;
            var cutRightPivot = strLastIndex;
            for (int i = 0; i < strLength; i++)
            {
                if (parseFlag[i])
                {
                    if (cutLeftPivot < i)
                    {
                        cutRightPivot = i;
                        result.Add(p_TargetString.SubstringFromTo(cutLeftPivot, cutRightPivot - 1));
                        cutLeftPivot = i + 1;
                    }
                    else
                    {
                        cutLeftPivot++;
                    }
                }
            }

            if (cutLeftPivot < strLength)
            {
                result.Add(p_TargetString.SubstringFromTo(cutLeftPivot, strLastIndex));
            }

            return result;
        }
        
        /// <summary>
        /// 문자열을 지정한 심볼 기준으로 잘라서 리턴하는 메서드. 단 [] 심볼이 있다면 해당 내부는 자르지 않는다.
        /// </summary>
        public static List<string> SplitAvoidCollectionParsingBracket(this string p_TargetString, char p_FindSymbol)
        {
            return p_TargetString.SplitAvoidBracket(MultiElementValueLeftBracket, MultiElementValueRightBracket, p_FindSymbol);
        }

        #endregion
    }
}