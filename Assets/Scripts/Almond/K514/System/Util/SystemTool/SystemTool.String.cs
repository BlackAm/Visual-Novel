using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace k514
{
    public static partial class SystemTool
    {
        public static bool ContainSymbol(this string p_Target, List<string> p_FilterSymbolSet)
        {
            foreach (var filterSymbol in p_FilterSymbolSet)
            {
                if (p_Target.Contains(filterSymbol))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 유니티 라이브러리는 슬레시를 사용하므로 역슬래시 기반의 경로 문자열을 변환하는데 사용하는 메서드
        /// </summary>
        public static string TurnToSlash(this string p_Target) => p_Target.Replace('\\', '/');

        /// <summary>
        /// 표준 라이브러리의 경로는 역슬레시와 슬래시를 전부 사용할 수 있고, 유니티 경로는 슬래시만 허용한다.
        /// 따라서, 역슬래시로의 경로 문자열 변환은 쓸 일이 별로 없다.
        /// </summary>
        public static string TurnToBackslash(this string p_Target) => p_Target.Replace('/', '\\');

        /// <summary>
        /// 지정한 파일 패스에서 /를 포함하는 디렉터리와 .이후의 확장자 문자열을 제거하고 파일명만 리턴하는 메서드
        /// </summary>
        public static string GetFileNameFromPath(this string p_Path, bool p_AddExt)
        {
            return p_AddExt ? Path.GetFileName(p_Path) : Path.GetFileNameWithoutExtension(p_Path);
        }

        /// <summary>
        /// 지정한 파일 패스의 상위 디렉터리를 리턴하는 메서드
        /// </summary>
        public static string GetUpperPath(this string p_Path)
        {
            var tryPath = p_Path;
            return tryPath.Last() == '/' ? tryPath.CutString("/", true, false).CutString("/", true, false) : tryPath.CutString("/", true, false);
        }
        
        /// <summary>
        /// 지정한 파일 패스에서 . 이후의 확장자를 제거한 문자열을 리턴하는 메서드
        /// </summary>
        public static string GetPathWithoutExtension(this string p_Target)
        {
            return p_Target.CutString(".", true, true);
        }

        /// <summary>
        /// 특정한 문자열이 p_CutPivot를 포함한다면 p_CutPivot를 제외한
        /// 앞부분 혹은 뒷부분을 리턴하는 메서드, p_CutPivot의를 포함하지 않는다면 원본을 리턴한다.
        /// </summary>
        public static string CutString(this string p_TargetString, string p_CutPivot, bool p_GetFowardPart, bool p_SearchFromFoward)
        {
            var pivotIndex = p_SearchFromFoward ? p_TargetString.IndexOf(p_CutPivot, StringComparison.Ordinal) : p_TargetString.LastIndexOf(p_CutPivot, StringComparison.Ordinal);
            if (pivotIndex < 0)
            {
                return p_TargetString;
            }
            else
            {
                return p_GetFowardPart ? p_TargetString.Substring(0, pivotIndex) 
                    : p_TargetString.Substring(pivotIndex + p_CutPivot.Length, p_TargetString.Length - pivotIndex - p_CutPivot.Length);
            }
        }
        
        /// <summary>
        /// 특정한 문자열이 p_CutPivot를 포함한다면 p_CutPivot를 포함한
        /// 앞부분 혹은 뒷부분을 리턴하는 메서드, p_CutPivot의를 포함하지 않는다면 원본을 리턴한다.
        /// </summary>
        public static string CutStringWithPivot(this string p_TargetString, string p_CutPivot, bool p_GetFowardPart, bool p_SearchFromFoward)
        {
            var pivotIndex = p_SearchFromFoward ? p_TargetString.IndexOf(p_CutPivot, StringComparison.Ordinal) : p_TargetString.LastIndexOf(p_CutPivot, StringComparison.Ordinal);
            if (pivotIndex < 0)
            {
                return p_TargetString;
            }
            else
            {
                return p_GetFowardPart ? 
                    p_TargetString.Substring(0, pivotIndex) + p_CutPivot
                    : p_CutPivot + p_TargetString.Substring(pivotIndex + p_CutPivot.Length, p_TargetString.Length - pivotIndex - p_CutPivot.Length);
            }
        }

        /// <summary>
        /// 특정한 문자열의 마지막 슬래쉬 혹은 역슬래쉬를 제거하고 리턴하는 메서드
        /// 단, 모든 역슬래쉬가 슬래쉬로 변환된다.
        /// 슬래쉬 혹은 역슬래쉬가 없다면 원본을 리턴한다.
        /// </summary>
        public static string CutLastSlashGetForward(this string p_TargetString)
        {
            return p_TargetString.TurnToSlash().CutString("/", true, false);
        }
        
        /// <summary>
        /// 특정한 문자열의 마지막 슬래쉬 혹은 역슬래쉬를 제거하고 리턴하는 메서드
        /// 단, 모든 역슬래쉬가 슬래쉬로 변환된다.
        /// 슬래쉬 혹은 역슬래쉬가 없다면 원본을 리턴한다.
        /// </summary>
        public static string CutLastSlashGetBack(this string p_TargetString)
        {
            return p_TargetString.TurnToSlash().CutString("/", false, false);
        }
    }
}