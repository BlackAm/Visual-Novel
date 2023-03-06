using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace k514
{
    public partial class SystemTool
    {
        #region <Fields>

        /// <summary>    
        /// [데이터 타입, 데이터 타입 인코드 익명 함수 대리자]
        /// 특정 타입의 값을 인코드하여 각 타입에 맞는 특별한 형식의 문자열로 리턴하는 대리자를 가지는 컬렉션
        /// </summary>
        private static Dictionary<Type, Func<Type, object, string>> EncodeByCaseCollection =
            new Dictionary<Type, Func<Type, object, string>>
            {
                {typeof(Type), (Type, Value) => Value.ToString()},
                {typeof(string), (Type, Value) => Value.ToString()},
                {typeof(byte), (Type, Value) => Value.ToString()},
                {typeof(sbyte), (Type, Value) => Value.ToString()},
                {typeof(Int16), (Type, Value) => Value.ToString()},
                {typeof(Int32), (Type, Value) => Value.ToString()},
                {typeof(Int64), (Type, Value) => Value.ToString()},
                {typeof(UInt16), (Type, Value) => Value.ToString()},
                {typeof(UInt32), (Type, Value) => Value.ToString()},
                {typeof(UInt64), (Type, Value) => Value.ToString()},
                {typeof(float), (Type, Value) => Value.ToString()},
                {typeof(double), (Type, Value) => Value.ToString()},
                {typeof(bool), (Type, Value) => Value.ToString()},
            };

        #endregion
        
        #region <Encode_Instance_To_FormatString>
        
        /// <summary>
        /// 데이터 타입을 다수 보유하는 자료형을 문자열로 인코딩하는 메서드
        /// </summary>
        /// <param name="p_Value">변환할 벨류 오브젝트</param>
        /// <param name="p_ParameterNumber">변환할 오브젝트를 구성하는 타입의 필드 개수</param>
        /// <typeparam name="K">변환할 벨류 오브젝트 타입</typeparam>
        /// <typeparam name="T">변환할 벨류 오브젝트를 구성하는 파라미터 타입</typeparam>
        private static string EncodeMultiValueType(Type p_Type, object p_Value)
        {
            var result = string.Empty;
            
            // backing field 까지 긁어오므로, 프로퍼티를 따로 가져올 필요는 없다.
            var fieldInfoGroup = p_Type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var fieldNumber = fieldInfoGroup.Length;
            for (int i = 0; i < fieldNumber; i++)
            {
                var targetType = fieldInfoGroup[i].FieldType;
                if (EncodeByCaseCollection.ContainsKey(targetType))
                {
                    result += fieldInfoGroup[i].GetValue(p_Value).EncodeValue(targetType);
                }
                else
                {
                    result += $"{MultiElementValueLeftBracket}{fieldInfoGroup[i].GetValue(p_Value).EncodeValue(targetType)}{MultiElementValueRightBracket}";
                }
                
                if (i != fieldNumber - 1)
                {
                    result += MultiElementValueParser;
                }
            }

            return result;
        }
        
        /// <summary>
        /// 리스트 자료형을 문자열로 인코딩하는 메서드
        /// </summary>
        private static string EncodeListType(Type p_ListType, object p_Value)
        {
            var result = string.Empty;
            if (p_Value == null) return result;
            
            var genericType = p_ListType.GetGenericArguments()[0];
            var targetEnumerator = p_ListType.GetMethod("GetEnumerator").Invoke(p_Value, null) as IEnumerator;

            if (targetEnumerator != null)
            {
                var onceFlag = true;
                while (targetEnumerator.MoveNext())
                {
                    if (onceFlag)
                    {
                        onceFlag = false;
                    }
                    else
                    {
                        result += ListRecordParser;
                    }

                    result += targetEnumerator.Current.EncodeValue(genericType);
                }
            }

            return result;
        }

        /// <summary>
        /// 컬렉션 자료형을 문자열로 인코딩하는 메서드
        /// </summary>
        private static string EncodeCollectionType(Type p_CollectionType, object p_Value)
        {
            var result = string.Empty;
            if (p_Value == null) return result;

            var genericTypeGroup = p_CollectionType.GetGenericArguments();
            var genericKeyType = genericTypeGroup[0];
            var genericValueType = genericTypeGroup[1];
            var targetEnumerator = p_CollectionType.GetMethod("GetEnumerator").Invoke(p_Value, null) as IEnumerator;

            if (targetEnumerator != null)
            {
                var onceFlag = true;
                while (targetEnumerator.MoveNext())
                {
                    if (onceFlag)
                    {
                        onceFlag = false;
                    }
                    else
                    {
                        result += ListRecordParser;
                    }

                    var tryPair = targetEnumerator.Current;
                    var tryPairType = tryPair.GetType();
                    var tryKey = tryPairType.GetField("key", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                        .GetValue(tryPair);
                    var tryValue = tryPairType.GetField("value", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                        .GetValue(tryPair);
                    result += tryKey.EncodeValue(genericKeyType) + KVParser + tryValue.EncodeValue(genericValueType);
                }
            }

            return result;
        }

        /// <summary>
        /// 지정한 데이터를 문자열로 인코딩하는 메서드
        /// </summary>
        public static string EncodeValue(this object value, Type p_Type)
        {
            if (value != null)
            {
                /* 기본 자료형과 같이 추상화 할 수 있는 타입인 경우 */
                if (EncodeByCaseCollection.ContainsKey(p_Type))
                {
                    return EncodeByCaseCollection[p_Type](p_Type, value);
                }
                /* 제네릭스나 열거형 상수처럼 추상화 할 수 없는 타입의 경우 */
                // BaseType이 System.Enum이면 열거형 상수
                else if (p_Type.BaseType == typeof(Enum))
                {
                    var enumOriginType = Enum.GetUnderlyingType(p_Type);
                    return EncodeByCaseCollection[enumOriginType](p_Type, value);
                }
                // 그 외의 경우는 컬렉션 타입
                else if (p_Type.IsGenericType)
                {
                    if(p_Type.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        return EncodeListType(p_Type, value);
                    }
                    
                    if(p_Type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        return EncodeCollectionType(p_Type, value);
                    }
                }
                else
                {
                    return EncodeMultiValueType(p_Type, value);
                }
            }

            return default;
        }
        
        #endregion
    }
}