using System.Collections;
using System.Reflection;
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    public partial class TableBase<M, K, T>
    {
        /// <summary>
        /// 게임 데이터 테이블의 레코드를 기술하는 추상 클래스.
        /// </summary>
        public abstract class TableRecordBase : ITableBaseRecord
        {
            /// <summary>
            /// 해당 레코드 인스턴스의 키 값
            /// </summary>
            public K KEY { get; protected set; }

            /// <summary>
            /// 해당 레코드가 초기화된 이후 호출되는 콜백
            /// </summary>
            public virtual async UniTask OnRecordDecoded()
            {
                await UniTask.CompletedTask;
            }
            
            /// <summary>
            /// 해당 레코드가 테이블에 등록된 경우 호출되는 콜백
            /// </summary>
            public virtual async UniTask OnRecordAdded()
            {
                await UniTask.CompletedTask;
            }
            
            public virtual async UniTask SetRecord(K p_Key, object[] p_RecordField)
            {
                KEY = p_Key;
                
                await UniTask.CompletedTask;
            }

#if UNITY_EDITOR
            public string GetDescription()
            {
                string result = $"Key Index : [{KEY}]\n\n";
                
                var fieldSet = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var fieldInfo in fieldSet)
                {
                    // key field는 상단에서 이미 출력하므로 스킵한다.
                    var targetFieldName = fieldInfo.Name;
                    if(targetFieldName == TableTool.TableKeyFieldName || targetFieldName.Contains(TableTool.BackingFieldRear)) continue;
                    
                    // 필드가 컬렉션 타입인 경우. 내부 레코드들을 출력한다.
                    if (fieldInfo.GetValue(this) is ICollection)
                    {
                        var tryCollection = fieldInfo.GetValue(this) as ICollection;
                        result += $"Field Name : [{fieldInfo.Name}] / Type : [{fieldInfo.FieldType}] / Value : collection <";
                        var tryEnumerator = tryCollection.GetEnumerator();
                        var hasCheckIterateFirseElementFlag = false;
                        while (tryEnumerator.MoveNext())
                        {
                            if (!hasCheckIterateFirseElementFlag)
                            {
                                hasCheckIterateFirseElementFlag = true;
                            }
                            else
                            {
                                result += ", ";

                            }
                            result += tryEnumerator.Current;
                        }
                        result += ">\n";
                    }
                    else
                    {
                        result += $"Field Name : [{fieldInfo.Name}] / Type : [{fieldInfo.FieldType}] / Value : [{fieldInfo.GetValue(this)}]\n";
                    }
                }
                
                var propertySet = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var propertyInfo in propertySet)
                {
                    // key field는 상단에서 이미 출력하므로 스킵한다.
                    if(propertyInfo.Name == TableTool.TableKeyFieldName) continue;
                    
                    // 프로퍼티가 컬렉션 타입인 경우. 내부 레코드들을 출력한다.
                    if (propertyInfo.GetValue(this) is ICollection)
                    {
                        var tryCollection = propertyInfo.GetValue(this) as ICollection;
                        result += $"Property Name : [{propertyInfo.Name}] / Type : [{propertyInfo.PropertyType}] / Value : collection <";
                        var tryEnumerator = tryCollection.GetEnumerator();
                        var hasCheckIterateFirseElementFlag = false;
                        while (tryEnumerator.MoveNext())
                        {
                            if (!hasCheckIterateFirseElementFlag)
                            {
                                hasCheckIterateFirseElementFlag = true;
                            }
                            else
                            {
                                result += ", ";

                            }
                            result += tryEnumerator.Current;
                        }
                        result += ">\n";
                    }
                    else
                    {
                        result += $"Property Name : [{propertyInfo.Name}] / Type : [{propertyInfo.PropertyType}] / Value : [{propertyInfo.GetValue(this)}]\n";
                    }
                }

                return result;
            }
#endif
        }
    }
}