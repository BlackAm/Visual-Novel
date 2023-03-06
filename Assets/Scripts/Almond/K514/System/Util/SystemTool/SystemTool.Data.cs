using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;
    
namespace k514
{
    public static partial class SystemTool
    {
        #region <Enum>

        public enum DataAccessType
        {
            Sequence,
            Random,
            RandomNotSame,
        }

        #endregion
        
        #region <Method/Data>

        public static int FindFirstEmptyIndex(this IList p_Array)
        {
            for (int i = 0; i < p_Array.Count; i++)
            {
                if (p_Array[i] == default)
                {
                    return i;
                }
            }

            return -1;
        }

        public static int FindFirstValidIndex(this IList p_Array)
        {
            for (int i = 0; i < p_Array.Count; i++)
            {
                if (p_Array[i] != default)
                {
                    return i;
                }
            }

            return -1;
        }
        
        public static int FindCeilingIndex(this List<float> p_TargetList, float p_Value)
        {
            var count = p_TargetList.Count - 1;
            switch (count)
            {
                case var _ when count > 0:
                    for (int i = 0; i < count; i++)
                    {
                        if (p_TargetList[i] <= p_Value && p_Value < p_TargetList[i + 1])
                        {
                            return i;
                        }
                    }

                    return count;
                default:
                    return 0;
            }
        }
        
        public static void Swap(this IList p_Array, int p_LeftIndex, int p_RightIndex)
        {
            var left = p_Array[p_LeftIndex];
            var right = p_Array[p_RightIndex];
            p_Array[p_LeftIndex] = right;
            p_Array[p_RightIndex] = left;
        }
        
        public static void Swap<T>(ref T p_Left, ref T p_Right) 
        {
            var tmp = p_Left;
            p_Left = p_Right;
            p_Right = tmp;
        }
        
        public static void SortIndexAscendant(ref int p_Left, ref int p_Right) 
        {
            if (p_Left > p_Right)
            {
                Swap(ref p_Left, ref p_Right);
            }
        }

        public static (int t_Lower, int t_Upper) SortIndexAscendant(int p_Left, int p_Right)
        {
            if (p_Left > p_Right)
            {
                return (p_Right, p_Left);
            }
            else
            {
                return (p_Left, p_Right);
            }
        }

        /// <summary>
        /// 지정한 실수와 0 사이의 랜덤한 값을 리턴하는 메서드
        /// </summary>
        public static int GetRandom(this ICollection p_List)
        {
            return Random.Range(0, p_List.Count);
        }
        
        /// <summary>
        /// 지정한 실수와 0 사이의 랜덤한 값을 리턴하는 메서드
        /// </summary>
        public static int GetRandomGeneric<T>(this ICollection<T> p_List)
        {
            return Random.Range(0, p_List.Count);
        }
        
        /// <summary>
        /// 지정한 실수와 0 사이의 랜덤한 값을 리턴하는 메서드
        /// </summary>
        public static int GetRandomExcept(this ICollection p_List, int p_ExceptIndex)
        {
            var listCount = p_List.Count;
            switch (p_ExceptIndex)
            {
                case var _ when listCount == 1 :
                case var idx when idx < 0 || listCount < idx + 1 :
                    return Random.Range(0, p_List.Count);
                default :
                    var rand = Random.Range(1, listCount);
                    if (rand == p_ExceptIndex)
                    {
                        rand = 0;
                    }

                    return rand;
            }
        }
        
        /// <summary>
        /// 지정한 실수와 0 사이의 랜덤한 값을 리턴하는 메서드
        /// </summary>
        public static int GetRandomExceptGeneric<T>(this ICollection<T> p_List, int p_ExceptIndex)
        {
            var listCount = p_List.Count;
            switch (p_ExceptIndex)
            {
                case var _ when listCount == 1 :
                case var idx when idx < 0 || listCount < idx + 1 :
                    return Random.Range(0, p_List.Count);
                default :
                    var rand = Random.Range(1, listCount);
                    if (rand == p_ExceptIndex)
                    {
                        rand = 0;
                    }

                    return rand;
            }
        }

        public static bool CheckCollectionSafe(this ICollection p_TargetList, int p_Index = 0)
        {
            return !ReferenceEquals(null, p_TargetList) && p_TargetList.Count > p_Index;
        }

        public static bool CheckGenericCollectionSafe<T>(this ICollection<T> p_TargetList, int p_Index = 0)
        {
            return !ReferenceEquals(null, p_TargetList) && p_TargetList.Count > p_Index;
        }
        
        public static T GetElementSafe<T>(this IList<T> p_TargetArray, int p_Index)
        {
            if (p_TargetArray.CheckGenericCollectionSafe(p_Index))
            {
                return p_TargetArray[p_Index];
            }
            else
            {
                return default;
            }
        }
        
        public static T GetElementRandomSafe<T>(this IList<T> p_TargetArray)
        {
            var randIndex = p_TargetArray.GetRandomGeneric();
            if (p_TargetArray.CheckGenericCollectionSafe(randIndex))
            {
                return p_TargetArray[randIndex];
            }
            else
            {
                return default;
            }
        }
        
        public static T GetElementRandomExceptSafe<T>(this IList<T> p_TargetArray, int p_ExceptIndex)
        {
            var randIndex = p_TargetArray.GetRandomExceptGeneric(p_ExceptIndex);
            if (p_TargetArray.CheckGenericCollectionSafe(randIndex))
            {
                return p_TargetArray[randIndex];
            }
            else
            {
                return default;
            }
        }
        
        public static void ClearArray<T>(this IList<T> p_TargetArray)
        {
            if (!ReferenceEquals(null, p_TargetArray))
            {
                var cnt = p_TargetArray.Count;
                for (int i = 0; i < cnt; i++)
                {
                    p_TargetArray[i] = default;
                }
            }
        }
        
        #endregion

        #region <Method/Convert>

        /// <summary>
        /// 타입을 변경하는 제네릭스 메서드
        /// </summary>
        public static K ChangeType<K>(object obj)
        {
            return (K) Convert.ChangeType(obj, typeof(K));
        }

        /// <summary>
        /// 지정한 정수 값을 long타입으로 캐스팅한 값을 리턴한다.
        /// </summary>
        public static long CastIntToLong(this int p_Value)
        {
            // 단순 캐스팅을 하면 정수가 음수일 때, msb 값이 소실되기 때문에
            // 아래와 같이 비트연산을 수행하여 캐스팅한다.
            return p_Value & long.MaxValue;
        }

        public static Vector2 DecodeToVector2(this (float,float) p_Tuple)
        {
            return new Vector2(p_Tuple.Item1, p_Tuple.Item2);
        }
        
        public static Vector3 DecodeToVector3(this (float, float, float) p_Tuple)
        {
            return new Vector3(p_Tuple.Item1, p_Tuple.Item2, p_Tuple.Item3);
        }

        #endregion
        
        #region <Method/Type>

        /// <summary>
        /// 어떤 타입이 인터페이스를 구현했는지 검증하는 메서드
        /// </summary>
        public static bool IsSubInterfaceOf(this Type p_Type, string p_InterfaceName) =>
            p_Type.GetInterface(p_InterfaceName, false) != null;

        /// <summary>
        /// 어떤 타입이 튜플인지 검증하는 메서드
        /// </summary>
        public static bool IsValueTuple(this Type p_Type) =>
            p_Type.IsSubInterfaceOf(typeof(System.Runtime.CompilerServices.ITuple).Name);
        
        /// <summary>
        /// 어떤 타입이 타겟 제네릭 타입의 파생 타입인지 검증하는 메서드
        /// </summary>
        public static bool IsSubclassOfRawGeneric(this Type p_Type, Type p_TargetType) {
            while (p_Type != null && p_Type != typeof(object)) 
            {
                var cur = p_Type.IsGenericType ? p_Type.GetGenericTypeDefinition() : p_Type;
                if (p_TargetType == cur) 
                {
                    return true;
                }
                p_Type = p_Type.BaseType;
            }
            return false;
        }

        /// <summary>
        /// 지정한 타입을 구현하거나 상속받는 추상 클래스 외의 클래스 타입을 배열로 리턴하는 메서드
        /// </summary>
        public static Type[] GetSubClassTypeSet(this Type p_Type)
        {
            if (p_Type.IsInterface)
            {
                return Assembly.GetAssembly(p_Type).GetTypes().Where(type => !type.IsAbstract && type.IsSubInterfaceOf(p_Type.Name)).ToArray();
            }
            else
            {
                if (p_Type.IsGenericType)
                {
                    return Assembly.GetAssembly(p_Type).GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOfRawGeneric(p_Type)).ToArray();
                }
                else
                {
                    return Assembly.GetAssembly(p_Type).GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(p_Type)).ToArray();
                }
            }
        }
        
        #endregion
    }
}