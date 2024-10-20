using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 시스템 기능을 포함하고 있는 클래스
    /// </summary>
    public static partial class SystemTool
    {
        #region <Constructor>

        static SystemTool()
        {
            EnumeratorCache = new Dictionary<GetEnumeratorType, Dictionary<Type, Array>>();
            EnumeratorCache.Add(GetEnumeratorType.GetAll, new Dictionary<Type, Array>());
            EnumeratorCache.Add(GetEnumeratorType.ExceptNone, new Dictionary<Type, Array>());
            EnumeratorCache.Add(GetEnumeratorType.ExceptMask, new Dictionary<Type, Array>());
            EnumeratorCache.Add(GetEnumeratorType.ExceptMaskNone, new Dictionary<Type, Array>());
              
            _ResourceTypeEnumerator = GetEnumEnumerator<ResourceType>(GetEnumeratorType.ExceptNone);
            SystemLateHandleEventTypeEnumerator = GetEnumEnumerator<SystemOnceFrameEventType>(GetEnumeratorType.ExceptNone);
        }

        #endregion

        #region <Resources>
                
        private static readonly ResourceType[] _ResourceTypeEnumerator;

        /// <summary>
        /// 지정한 파일 패스가 리소스 패스인지 검증하며, 리소스 패스라면 해당 리소스 타입을 리턴하는 메서드
        /// 2번째 파라미터를 통해서, 시스템 테이블 같은 경로가 항상 일정한 리소스 타입을 검증에서 제외시킬 수 있다.
        /// </summary>
        public static ResourceType TryGetUnityResourcePath(this string p_TargetPath, bool p_ExceptFixedLoadType)
        {
            foreach (var resourceType in _ResourceTypeEnumerator)
            {
                if (p_ExceptFixedLoadType && !resourceType.IsFixedLoadTypeResource())
                {
                    var resourcePath = SystemMaintenance.GetSystemResourcePath(AssetLoadType.FromUnityResource, resourceType,
                        PathType.SystemGenerate_AbsolutePath).CutStringWithPivot(SystemMaintenance.UnityResourceDirectory, false, true);
                    
                    if (p_TargetPath.Contains(resourcePath))
                    {
                        return resourceType;
                    }
                }
            }
            return ResourceType.None;
        }

        #endregion
        
        #region <EnumArray>

        private static readonly string NoneSymbol = "NONE";

        /// <summary>
        /// Enumerator Array[] Cache
        /// </summary>
        private static Dictionary<GetEnumeratorType, Dictionary<Type, Array>> EnumeratorCache;

        public enum GetEnumeratorType
        {
            GetAll,
            ExceptNone,
            ExceptMask,
            ExceptMaskNone,
        }

        /// <summary>
        /// 특정 Enum 타입을 기술하는 배열을 리턴하는 메서드
        /// </summary>
        public static T[] GetEnumEnumerator<T>(GetEnumeratorType p_GetEnumeratorType)
        {
            var genericType = typeof(T);
            if (genericType.BaseType == typeof(Enum))
            {
                if (EnumeratorCache[p_GetEnumeratorType].ContainsKey(genericType))
                {
                    return (T[]) EnumeratorCache[p_GetEnumeratorType][genericType];
                }
                else
                {
                    var valueSet = Enum.GetValues(genericType);
                    var nameSet = Enum.GetNames(genericType);
                    var list = new List<T>();

                    for (int i = 0; i < nameSet.Length; i++)
                    {
                        switch (p_GetEnumeratorType)
                        {
                            case GetEnumeratorType.GetAll:
                                list.Add((T)valueSet.GetValue(i));
                                break;
                            case GetEnumeratorType.ExceptNone:
                            {
                                var formedName = nameSet[i].ToUpper();
                                if (!string.Equals(NoneSymbol, formedName))
                                {
                                    list.Add((T) valueSet.GetValue(i));
                                }
                            }
                                break;
                            case GetEnumeratorType.ExceptMask:
                            {
                                var formedNumber = (int)valueSet.GetValue(i);
                                if (formedNumber.IsPowerOfTwo())
                                {
                                    list.Add((T) valueSet.GetValue(i));
                                }
                            }
                                break;
                            case GetEnumeratorType.ExceptMaskNone:
                            {
                                var formedNumber = (int)valueSet.GetValue(i);
                                var formedName = nameSet[i].ToUpper();
                                if (formedNumber.IsPowerOfTwo() && !string.Equals(NoneSymbol, formedName))
                                {
                                    list.Add((T) valueSet.GetValue(i));
                                }
                            }
                                break;
                        }
                    }

                    var result = list.ToArray();
                    EnumeratorCache[p_GetEnumeratorType].Add(genericType, result);
                    return result;
                }
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 특정 Enum 타입을 기술하는 배열을 리턴하는 메서드, out모드 버전
        /// </summary>
        public static bool TryGetEnumEnumerator<T>(GetEnumeratorType p_GetEnumeratorType, out T[] o_Enumerator)
        {
            o_Enumerator = GetEnumEnumerator<T>(p_GetEnumeratorType);
            return !ReferenceEquals(null, o_Enumerator);
        }
        
        /// <summary>
        /// 특정 Enum 타입을 기술하는 배열을 문자열 배열로 리턴하는 메서드
        /// </summary>
        public static string[] GetEnumStringEnumerator<T>(GetEnumeratorType p_GetEnumeratorType)
        {
            if (TryGetEnumEnumerator<T>(p_GetEnumeratorType, out var o_Enumerator))
            {
                return o_Enumerator.Select(type => type.ToString()).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 특정 enum의 첫번째 타입을 리턴하는 메서드
        /// </summary>
        public static T GetFirstElementOf<T>(GetEnumeratorType p_GetEnumeratorType)
        {
            var targetArray = GetEnumEnumerator<T>(p_GetEnumeratorType);
            return targetArray[0];
        }
        
        /// <summary>
        /// 특정 enum의 마지막 타입을 리턴하는 메서드
        /// </summary>
        public static T GetLastElementOf<T>(GetEnumeratorType p_GetEnumeratorType)
        {
            var targetArray = GetEnumEnumerator<T>(p_GetEnumeratorType);
            return targetArray[targetArray.Length - 1];
        }
        
        #endregion
        
        #region <Editor>

#if UNITY_EDITOR
        public static void PlayEditorMode(bool p_Flag)
        {
            EditorApplication.isPlaying = p_Flag;
        }

        public static void PauseEditorMode(bool p_Flag)
        {
            EditorApplication.isPaused = p_Flag;
        }

        public static void TogglePauseEditorMode()
        {
            EditorApplication.isPaused = !EditorApplication.isPaused;
        }
#endif

        #endregion
    }
}