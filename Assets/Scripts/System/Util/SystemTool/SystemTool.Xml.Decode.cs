using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BlackAm
{
    public partial class SystemTool
    {
        #region <Fields>

        /// <summary>
        /// [데이터 타입, 데이터 타입 디코드 익명 함수 대리자]
        /// 문자열 벨류 코드로부터 각 벨류타입의 값을 디코드하여 리턴하는 대리자를 타입별로 가지는 컬렉션
        /// </summary>
        private static Dictionary<Type, Func<Type, string, (bool, object)>> _DecodeByCaseCollection =
            new Dictionary<Type, Func<Type, string, (bool, object)>>
            {
                {typeof(Type), (valueType, valueString) => DecodeTypeType(valueString)},
                {typeof(string), (valueType, valueString) => (true, valueString.TrimFrontRearSpace())},
                {typeof(bool), (valueType, valueString) => DecodeBooleanType(valueString)},
                {typeof(byte), DecodeSingleValueType<byte>},
                {typeof(sbyte), DecodeSingleValueType<sbyte>},
                {typeof(Int16), DecodeSingleValueType<Int16>},
                {typeof(Int32), DecodeSingleValueType<Int32>},
                {typeof(Int64), DecodeSingleValueType<Int64>},
                {typeof(UInt16), DecodeSingleValueType<UInt16>},
                {typeof(UInt32), DecodeSingleValueType<UInt32>},
                {typeof(UInt64), DecodeSingleValueType<UInt64>},
                {typeof(float), DecodeSingleValueType<float>},
                {typeof(double), DecodeSingleValueType<double>},
            };

        /// <summary>
        /// 데이터를 직렬화하는 과정에서 생성자에 의해 초기화되지 않는 필드값이 직렬화되어, 나중에 디코딩하는 과정에서
        /// 생성자 갯수가 맞지 않아 테이블을 읽지 못하는 경우가 있다.
        ///
        /// 그러한 경우, 해당 직렬화코드를 배열로 보았을 때 파라미터가 몇번째에 들어가면 되는지 수동으로 기술하는 컬렉션
        ///
        /// 예를 들어, 아래의 Color32 타입의 경우에는 인코딩되면 5개의 필드가 직렬화되고 그 중에 1~4번째 값이 생성자의 0~3번째 원소가
        /// 되므로 아래와 같이 파라미터 초기화 순서를 기술하고 있다.
        /// </summary>
        private static Dictionary<(Type, int), (int[], Type[])> DecodeConstructorPassiveParameterSequenceGroup =
            new Dictionary<(Type, int), (int[], Type[])>
            {
                {(typeof(Color32), 5), (new []{1, 2, 3, 4}, new []{typeof(byte), typeof(byte), typeof(byte), typeof(byte)})},
                {(typeof(Color32), 4), (new []{0, 1, 2, 3}, new []{typeof(byte), typeof(byte), typeof(byte), typeof(byte)})}
            };
        
        #endregion

        #region <Method/Decode/SingleType>
        
        /// <summary>
        /// Type 데이터 타입을 문자열로부터 디코딩하는 메서드
        /// </summary>
        private static (bool, Type) DecodeTypeType(string p_ValueString)
        {
            try
            {
                var tryString = p_ValueString.TrimAndOffBracket();
                var tryType = Type.GetType(tryString);
                var result = !ReferenceEquals(null, tryType);
                return (result, tryType);
            }
#if UNITY_EDITOR
            catch (Exception e)
            {
                if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                {
                    Debug.LogWarning($"[FAIL] : {p_ValueString} => TYPE\n{e.Message}");
                }
                return default;
            }
#else
            catch
            {
                return default;
            }
#endif   
        }
        
        /// <summary>
        /// 논리 데이터 타입을 문자열로부터 디코딩하는 메서드
        /// </summary>
        private static (bool, bool) DecodeBooleanType(string p_ValueString)
        {
            try
            {
                var tryString = p_ValueString.TrimAndOffBracket();
                if (tryString == BoolFalseSymbol)
                {
                    return (true, false);
                }
                else if (tryString == BoolTrueSymbol)
                {
                    return (true, true);
                }
                else
                {
                        /*
                        'true' parse as True
                        'True' parse as True
                        'TRUE' parse as True
                        'false' parse as False
                        'False' parse as False
                        'FALSE' parse as False
                        */
                        var tryResult = bool.Parse(tryString);
                        return (true, tryResult);
                }
            }
#if UNITY_EDITOR
            catch (Exception e)
            {
                if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                {
                    Debug.LogWarning($"[FAIL] : {p_ValueString} => BOOL\n{e.Message}");
                }
                return default;
            }
#else
            catch
            {
                return default;
            }
#endif 
        }

        /// <summary>
        /// 기본 자료형을 문자열로부터 디코딩하는 메서드
        /// </summary>
        private static (bool, object) DecodeSingleValueType<T>(Type p_SpawnType, string p_ValueString)
        {
            try
            {
                var tryString = p_ValueString.TrimAndOffBracket();
                var tryResult = ChangeType<T>(tryString);
                return (!ReferenceEquals(null, tryResult), tryResult);
            }
#if UNITY_EDITOR
            catch (Exception e)
            {
                if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                {
                    Debug.LogWarning($"[FAIL] : {p_ValueString} => SINGLE ({p_SpawnType})\n{e.Message}");
                }
                return default;
            }
#else
            catch
            {
                return default;
            }
#endif 
        }

        /// <summary>
        /// 리스트 자료형을 문자열로부터 디코딩하는 메서드
        /// </summary>
        private static (bool, object) DecodeEnumType(Type p_EnumType, string p_ValueString)
        {
            try
            {
                var tryString = p_ValueString.TrimAndOffBracket();
                if(IsDigitsOnly(tryString))
                {
                    var enumOriginType = Enum.GetUnderlyingType(p_EnumType);
                    // 기존에는 각 enum에 대응하는 정수를 입력해도 디코딩이 가능했지만,
                    // 생성자 함수에서 enum값과 정수값을 구분할 수 없는 문제가 있다.
                    return tryString._DecodeValue(enumOriginType);
                }
                // enum을 문자로 기술한 경우                    
                else
                {
                    var enumFlagSet = tryString.Split(EnumFlagParser);
                    // enum flag인 경우에
                    if (enumFlagSet.Length > 1)
                    {
                        int mask = 0;
                        foreach (var enumFlagValue in enumFlagSet)
                        {
                            mask += (int)Enum.Parse(p_EnumType, enumFlagValue, true);
                        } 
                                
                        return (true, mask);
                    }
                    // enum flag가 아닌 경우에
                    else
                    {
                        var tryResult = Enum.Parse(p_EnumType, tryString, true);
                        return (true, tryResult);
                    }
                }
            }
#if UNITY_EDITOR
            catch (Exception e)
            {
                if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                {
                    Debug.LogWarning($"[FAIL] : {p_ValueString} => ENUM ({p_EnumType})\n{e.Message}");
                }
                return default;
            }
#else
            catch
            {
                return default;
            }
#endif 
        }

        #endregion
        
        #region <Method/Decode/Collection>
        
        /// <summary>
        /// 리스트 자료형을 문자열로부터 디코딩하는 메서드
        /// </summary>
        private static (bool, object) DecodeListType(Type p_ListType, string p_ValueString)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintDecode)
            {
                Debug.Log($"[DECODE] : {p_ValueString} => LIST ({p_ListType})");
            }
#endif   
            var tryString = p_ValueString.TrimAndOffBracket();
            var parsedStringValue = tryString.SplitAvoidCollectionParsingBracket(ListRecordParser).TrimSymbol(ListRecordParser);
            var result = parsedStringValue.Count > 0 ? p_ListType.GetConstructor(Type.EmptyTypes).Invoke(null) : null;

            if (!ReferenceEquals(null, result))
            {
                var method = result.GetType().GetMethod("Add");
                var genericType = p_ListType.GetGenericArguments()[0];
                foreach (var parsedString in parsedStringValue)
                {
                    var tryResult = parsedString._DecodeValue(genericType);
                    if (tryResult.Item1)
                    {
#if UNITY_EDITOR
                        if (CustomDebug.PrintDecode)
                        {
                            Debug.Log($"[PARSE] : {p_ValueString} => ELEMENT {parsedString} ({genericType})");
                        }
#endif   
                        method.Invoke(result, new [] { tryResult.Item2 });
                    }
                    else
                    {
#if UNITY_EDITOR
                        if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                        {
                            Debug.LogWarning($"[FAIL] : {p_ValueString} => ELEMENT {parsedString} ({genericType})");
                        }
#endif   
                        return default;
                    }
                }

                return (true, result);
            }
            else
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                {
                    Debug.LogWarning($"[FAIL] : {p_ValueString} => LIST ({p_ListType})");
                }
#endif   
                return default;
            }
        }

        /// <summary>
        /// 데이터 타입을 다수 보유하는 자료형을 문자열로부터 디코딩하는 메서드
        /// </summary>
        private static (bool, object) DecodeCollectionType(Type p_CollectionType, string p_ValueString)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintDecode)
            {
                Debug.Log($"[DECODE] : {p_ValueString} => DICT ({p_CollectionType})");
            }
#endif   
            var tryString = p_ValueString.TrimAndOffBracket();
            var hasValidRecordParser = tryString.FindCharIndexBetweenCollectionParsingBracket(CollectionRecordParser);

            if (hasValidRecordParser > -1)
            {
                var parsedStringValue = tryString.SplitAvoidCollectionParsingBracket(CollectionRecordParser);
                var result = parsedStringValue.Count > 0
                    ? p_CollectionType.GetConstructor(Type.EmptyTypes).Invoke(null)
                    : null;

                if (!ReferenceEquals(null, result))
                {
                    var method = result.GetType().GetMethod("Add");
                    var genericTypeGroup = p_CollectionType.GetGenericArguments();
                    var genericKeyType = genericTypeGroup[0];
                    var genericValueType = genericTypeGroup[1];

                    foreach (var parsedString in parsedStringValue)
                    {
                        var parsedTupleStringValue = parsedString.SplitBetweenBracket(true, KVParser);
                        var tryKey = parsedTupleStringValue[0]._DecodeValue(genericKeyType);
                        if (tryKey.Item1)
                        {
#if UNITY_EDITOR
                            if (CustomDebug.PrintDecode)
                            {
                                Debug.Log($"[PARSE] : {p_ValueString} => KEY {parsedTupleStringValue[0]} ({genericKeyType})");
                            }
#endif   
                            var tryValue = parsedTupleStringValue[1]._DecodeValue(genericValueType);
                            if (tryValue.Item1)
                            {
#if UNITY_EDITOR
                                if (CustomDebug.PrintDecode)
                                {
                                    Debug.Log($"[PARSE] : {p_ValueString} => VALUE {parsedTupleStringValue[1]} ({genericValueType})");
                                }
#endif   
                                method.Invoke(result, new [] { tryKey.Item2, tryValue.Item2 });
                            }
                            else
                            {
#if UNITY_EDITOR
                                if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                                {
                                    Debug.LogWarning($"[FAIL] : {p_ValueString} => VALUE {parsedTupleStringValue[1]} ({genericValueType})");
                                }
#endif     
                                return default;
                            }
                        }
                        else
                        {
#if UNITY_EDITOR
                            if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                            {
                                Debug.LogWarning($"[FAIL] : {p_ValueString} => KEY {parsedTupleStringValue[0]} ({genericKeyType})");
                            }
#endif
                            return default;
                        }
                    }

                    return (true, result);
                }
                else
                {
#if UNITY_EDITOR
                    if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                    {
                        Debug.LogWarning($"[FAIL] : {p_ValueString} => DICT ({p_CollectionType})");
                    }
#endif
                    return default;
                }
            }
            // 자를 수 있는 레코드 파싱 심볼이 없었던 경우
            else
            {
                // KV파서가 있었던 경우
                var hasValidKVParser = tryString.FindCharIndexBetweenCollectionParsingBracket(KVParser);
                if (hasValidKVParser > -1)
                {
                    var result = p_CollectionType.GetConstructor(Type.EmptyTypes).Invoke(null);
                    var genericTypeGroup = p_CollectionType.GetGenericArguments();
                    var genericKeyType = genericTypeGroup[0];
                    var genericValueType = genericTypeGroup[1];
                    var parsedTupleStringValue = tryString.SplitAvoidCollectionParsingBracket(KVParser);
#if UNITY_EDITOR
                        if (CustomDebug.PrintDecode)
                        {
                            Debug.Log($"[PARSE] : {p_ValueString} => KEY {parsedTupleStringValue[0]} ({genericKeyType})");
                        }
#endif   
                    var tryKey = parsedTupleStringValue[0]._DecodeValue(genericKeyType);
                    if (tryKey.Item1)
                    {
#if UNITY_EDITOR
                            if (CustomDebug.PrintDecode)
                            {
                                Debug.Log($"[PARSE] : {p_ValueString} => VALUE {parsedTupleStringValue[1]} ({genericValueType})");
                            }
#endif   
                        var tryValue = parsedTupleStringValue[1]._DecodeValue(genericValueType);
                        if (tryValue.Item1)
                        {
                            result.GetType().GetMethod("Add").Invoke(result, new [] { tryKey.Item2, tryValue.Item2 });
                        }
                        else
                        {
#if UNITY_EDITOR
                            if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                            {
                                Debug.LogWarning($"[FAIL] : {p_ValueString} => VALUE {parsedTupleStringValue[1]} ({genericValueType})");
                            }
#endif     
                            return default;
                        }
                    }
                    else
                    {
#if UNITY_EDITOR
                        if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                        {
                            Debug.LogWarning($"[FAIL] : {p_ValueString} => KEY {parsedTupleStringValue[0]} ({genericKeyType})");
                        }
#endif
                        return default;
                    }
                    
                    return (true, result);
                }
                else
                {
#if UNITY_EDITOR
                    if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                    {
                        Debug.LogWarning($"[FAIL] : {p_ValueString} => DICT ({p_CollectionType})");
                    }
#endif
                    return default;
                }
            }
        }

        /// <summary>
        /// 데이터 타입을 다수 보유하는 자료형을 문자열로부터 디코딩하는 메서드
        /// </summary>
        private static (bool, object) DecodeConstructor(Type p_SpawnType, string p_ValueString)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintDecode)
            {
                Debug.Log($"[DECODE] : {p_ValueString} => OBJECT ({p_SpawnType})");
            }
#endif   
            var tryString = p_ValueString.TrimAndOffBracket();
            var parsedStringValue = tryString.SplitAvoidCollectionParsingBracket(MultiElementValueParser);
            var parameterNumber = parsedStringValue.Count;
            var targetConstructorTypeParameterSet = new List<Type>();
            var constructorParameterSet = new List<object>();
            var targetConstructorGroup = p_SpawnType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var decodeResultSpawnedFlag = false;

#if UNITY_EDITOR
            if (CustomDebug.PrintDecode)
            {
                Debug.Log($"* [SYMBOL] *");
                for (int i = 0; i < parameterNumber; i++)
                {
                    Debug.Log(parsedStringValue[i]);
                }
                Debug.Log($"* * * * *");
            }
#endif

            foreach (var constructorInfo in targetConstructorGroup)
            {
                var targetParams = constructorInfo.GetParameters();
                var constructorParamNumber = targetParams.Length;
                var iterateScale = Mathf.Max(constructorParamNumber, parameterNumber);
                
#if UNITY_EDITOR
                if (CustomDebug.PrintDecode)
                {
                    Debug.Log($"* * * * *");
                    Debug.Log($"* [CONSTUCTOR] *");
                    for (int i = 0; i < constructorParamNumber; i++)
                    {
                        Debug.Log(targetParams[i].Name);
                    }
                    Debug.Log($"* * * * *");
                }
#endif
                
                for (int i = 0; i < iterateScale; i++)
                {
                    // 현재 타겟 생성자의 파라미터 숫자를 초과해서 순회하는 경우
                    if (i >= constructorParamNumber)
                    {
                        // 해당 루프를 버린다.
                        goto SEG_INTERRUPT_NEXT_CONSTRUCTOR;
                    }

                    var targetParameter = targetParams[i];
                    var tryConstructorParamType = targetParameter.ParameterType;
                    targetConstructorTypeParameterSet.Add(tryConstructorParamType);

                    // 파싱된 문자열을 지정한 파라미터 타입으로 디코드한다.
                    if (i < parameterNumber)
                    {
                        var tryResult = parsedStringValue[i]._DecodeValue(tryConstructorParamType);
                        if (tryResult.Item1)
                        {
                            constructorParameterSet.Add(tryResult.Item2);
#if UNITY_EDITOR
                            if (CustomDebug.PrintDecode)
                            {
                                Debug.Log($"[CONSTRUCTOR => PARSE] : {parsedStringValue[i]} => PARAM ({tryResult.Item2})");
                            }
#endif
                        }
                        else
                        {
                            // 해당 루프를 버린다.
                            goto SEG_INTERRUPT_NEXT_CONSTRUCTOR;
                        }
                    }
                    // 파싱된 문자열 수를 초과한 경우
                    else
                    {
                        // 초과된 분의 타입이 옵셔널 파라미터였던 경우
                        if (targetParameter.IsOptional)
                        {
                            // 기본값을 삽입한다.
                            constructorParameterSet.Add(targetParameter.DefaultValue);
#if UNITY_EDITOR
                            if (CustomDebug.PrintDecode)
                            {
                                Debug.Log($"[CONSTRUCTOR => PARSE] => PARAM (OPTIONAL DEFAULT)");
                            }
#endif
                        }
                        else
                        {
                            // 해당 루프를 버린다.
                            goto SEG_INTERRUPT_NEXT_CONSTRUCTOR;
                        }
                    }
                }
                
                decodeResultSpawnedFlag = true;
                break;
                
                SEG_INTERRUPT_NEXT_CONSTRUCTOR : ;
                targetConstructorTypeParameterSet.Clear();
                constructorParameterSet.Clear();
                
#if UNITY_EDITOR
                if (CustomDebug.PrintDecode | CustomDebug.PrintDecodeFail)
                {
                    Debug.LogWarning($"[CONSTRUCTOR => PARSE] => FAIL");
                }
#endif
            }

            if (decodeResultSpawnedFlag)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintDecode)
                {
                    Debug.Log($"[SUCCESS] : {p_ValueString} => OBJECT ({p_SpawnType})");
                }
#endif
                return (true, p_SpawnType.GetConstructor(targetConstructorTypeParameterSet.ToArray()).Invoke(constructorParameterSet.ToArray()));
            }
            else
            {
                var tryKey = (p_SpawnType, parameterNumber);
                if (DecodeConstructorPassiveParameterSequenceGroup.ContainsKey(tryKey))
                {
                    var parameterSeqSet = DecodeConstructorPassiveParameterSequenceGroup[tryKey];
                    for (int i = 0; i < parameterSeqSet.Item1.Length; i++)
                    {
                        var permutatedParamIndex = parameterSeqSet.Item1[i];
                        var permutatedParamType = parameterSeqSet.Item2[i];
                        var tryResult = parsedStringValue[permutatedParamIndex]._DecodeValue(permutatedParamType);
                        if (tryResult.Item1)
                        {
                            targetConstructorTypeParameterSet.Add(permutatedParamType);
                            constructorParameterSet.Add(tryResult.Item2);
                        }
                        else
                        {
                            return default;
                        }
                    }
#if UNITY_EDITOR
                    if (CustomDebug.PrintDecode)
                    {
                        Debug.Log($"[FALLBACK => SUCCESS] : {p_ValueString} => OBJECT ({p_SpawnType})");
                    }
#endif
                    return (true, p_SpawnType.GetConstructor(targetConstructorTypeParameterSet.ToArray()).Invoke(constructorParameterSet.ToArray()));
                }
                else
                {
#if UNITY_EDITOR
                    if (CustomDebug.PrintDecode)
                    {
                        Debug.LogWarning($"[FAIL] : {p_ValueString} => OBJECT ({p_SpawnType})");
                    }
#endif
                    return default;
                }
            }
        }

        /// <summary>
        /// 문자열로 기술된 값을 지정한 타입으로 파싱하는 메서드
        /// </summary>
        private static (bool, object) _DecodeValue(this String p_Value, Type p_Type)
        {
            if (!string.IsNullOrEmpty(p_Value))
            {
                /* 기본 자료형과 같이 추상화 할 수 있는 타입인 경우 */
                if (_DecodeByCaseCollection.ContainsKey(p_Type))
                {
                    return _DecodeByCaseCollection[p_Type](p_Type, p_Value);
                }
                /* 열거형 상수인 타입의 경우 */
                // BaseType이 System.Enum이면 열거형 상수
                else if (p_Type.BaseType == typeof(Enum))
                {
                    return DecodeEnumType(p_Type, p_Value);
                }
                /* 제네릭스 컬렉션 타입의 경우 */
                else if (p_Type.IsGenericType)
                {
                    if(p_Type.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        return DecodeListType(p_Type, p_Value);
                    }
                    else if(p_Type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        return DecodeCollectionType(p_Type, p_Value);
                    }
                    else if (p_Type.IsValueTuple())
                    {
                        return DecodeConstructor(p_Type, p_Value);
                    }
                }
                /* 그 외 생성자 심볼('[' | ',' | ']')에 의해 생성자 디코딩을 수행해야 하는 경우 */
                else
                {
                    return DecodeConstructor(p_Type, p_Value);
                }
            }

            return default;
        }

        public static object DecodeValue(this String p_Value, Type p_Type)
        {
            var tryResult = p_Value._DecodeValue(p_Type);
#if UNITY_EDITOR
            if (CustomDebug.PrintDecode || CustomDebug.PrintDecodeError)
            {
                if (!tryResult.Item1 && !string.IsNullOrEmpty(p_Value))
                {
                    Debug.LogError($"[DECODE FAIL] : {p_Value} => ({p_Type})");
                }
            }
#endif   

            return tryResult.Item2;
        }

        #endregion
    }
}