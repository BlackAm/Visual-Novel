using System;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 디버그 출력을 플래그로 제어하는 정적 클래스
    /// </summary>
    public static partial class CustomDebug
    {
        private static int DebugPivotCounter;

        private static string GetFormed(object p_Caller, string p_Message)
        {
            return $"[{(p_Caller == null ? "Null" : p_Caller.GetType().Name)}], {p_Message}";
        }

        public static void Log(object p_Caller, string p_Message)
        {
            var formed = GetFormed(p_Caller, p_Message);
#if UNITY_EDITOR
            Debug.Log(formed);
#else
            Console.WriteLine(formed);
#endif
        }
        
        public static void LogWarning(object p_Caller, string p_Message)
        {
            var formed = GetFormed(p_Caller, p_Message);
#if UNITY_EDITOR
            Debug.LogWarning(formed);
#else
            Console.WriteLine(formed);
#endif 
        }
        
        public static void LogError(object p_Caller, string p_Message)
        {
            var formed = GetFormed(p_Caller, p_Message);
#if UNITY_EDITOR
            Debug.LogError(formed);
#else
            Console.WriteLine(formed);
#endif
        }

        public static void WardHere(object p_Caller, string p_Message)
        {
            var formed = $"{GetFormed(p_Caller, p_Message)} ({DebugPivotCounter++})";
#if UNITY_EDITOR
            Debug.Log(formed);
#else
            Console.WriteLine(formed);
#endif
        }
    }
}