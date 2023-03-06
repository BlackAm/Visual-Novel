using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace k514
{
    /// <summary>
    /// 특정한 위치의 테이블 파일을 읽고 컬렉션으로 파싱하는 기능을 가지는 클래스
    /// </summary>
    public static class TableLoader
    {
        #region <Consts>
        
        /// <summary>
        /// 지정한 테이블 클래스로부터 테이블 파일 정보를 참조하여 테이블을 읽고 문자열컬렉션으로 디코딩하는 메서드
        /// </summary>
        public static async UniTask<(bool, TextAsset)> ReadTableFile<KeyType>(ITableBase p_Table)
        {
            var tableType = p_Table.TableType;
            
            var textAsset = default(TextAsset);
            switch (tableType)
            {
#if UNITY_EDITOR
                case TableTool.TableType.EditorOnlyTable:
                {
                    var targetFileName = p_Table.GetTableFileFullPath(AssetLoadType.FromUnityResource, PathType.SystemGenerate_RelativePath, TableTool.TableNameType.Alter, true);
                    await UniTask.SwitchToMainThread();
                    textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(targetFileName);
                    break;
                }
#endif
                case TableTool.TableType.SystemTable:
                {
                    var targetFileName = p_Table.GetTableFileFullPath(AssetLoadType.FromUnityResource, PathType.SystemGenerate_RelativePath, TableTool.TableNameType.Alter, false);
                    textAsset = await SystemTool.LoadAsync<TextAsset>(targetFileName);
                    break;
                }
                case TableTool.TableType.WholeGameTable:
                case TableTool.TableType.SceneGameTable:
                {
                    var targetFileName = p_Table.GetTableFileName(TableTool.TableNameType.Alter, true);
                    // ResourceListData 가 게임 데이터로 다루어지고 있는 경우,
                    // 에셋 로드 매니저를 거치지 않고 로드 해야 한다.
                    // 에셋 로드 매니저는 ResourceListData 없이 동작하지 않기 때문에, ResourceListData를 로드하기 위해
                    // 사용할 수 없기 때문이다.
#if RESOURCE_LIST_TABLE_INTO_GAMETABLE
                    if (typeof(M) == typeof(ResourceListData))
                    {
                        loadedAsset = ResourceListData.GetInstance.GetResourceListTableTextAsset();
                    }
                    else
#endif
                    {
                        await UniTask.SwitchToMainThread();
                        textAsset = (await LoadAssetManager.GetInstance())
                            .LoadAsset<TextAsset>(ResourceType.Table, ResourceLifeCycleType.Scene, targetFileName).Item2;
                    }
                    break;
                }
            }

            return (!ReferenceEquals(null, textAsset), textAsset);
        }

        /// <summary>
        /// 지정한 xml 원본 텍스트 파일을 SecurityParser를 통해 트리구조로 디코딩하고, 디코딩된 값을 컬렉션으로 리턴하는 메서드
        /// 원본 텍스트 파일을 가져오려면 LoadAssetManager를 이용하고, 텍스트 파일을 가져오는 과정까지 포함한 메서드는
        /// TryLoadXmlTable로 따로 구현되어 있다.
        /// </summary>
        public static async UniTask<Dictionary<KeyType, Dictionary<String, String>>> ParsingTableFile<KeyType>(ITableBase p_Table, string p_RawXmlText)
        {
#if UNITY_EDITOR
            try
            {
                var result = await ParsingTableFile<KeyType>(await p_RawXmlText.TryParsingXmlAsync());
                p_Table.TryWriteByteCode(result);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"Table not valid : Please check the name or record KV at class script of Table [{p_Table.GetTableFileName(TableTool.TableNameType.Alter, false)}]\n[{e.Message}]\n[{e.StackTrace}]");
                return null;
            }
#else
            return await ParsingTableFile<KeyType>(await p_RawXmlText.TryParsingXmlAsync());
#endif
        }

        /// <summary>
        /// SecurityElement 타입은 내부에 xml 엘리먼트를 문자열 key로 하는 컬렉션을 가지고 있으며, 해당 컬렉션의 value는
        /// 재차 SecurityElement로 트리 구조를 구성하고 있다.
        /// </summary>
        public static async UniTask<Dictionary<KeyType, Dictionary<String, String>>> ParsingTableFile<KeyType>(SecurityElement p_ParsedXml)
        {
            await UniTask.SwitchToThreadPool();
          
            var result = new Dictionary<KeyType, Dictionary<String, String>>();
            if (p_ParsedXml != null && p_ParsedXml.Children != null)
            {
                int count = p_ParsedXml.Children.Count;
                for (int index = 0; index < count; index++ )
                {
                    var subNode = p_ParsedXml.Children[index] as SecurityElement;
                    
                    /* Case 1 : 특정 노드의 자손이 없는 경우 */
                    var trySet = subNode.Children;
                    if (!trySet.CheckCollectionSafe())
                    {
                        continue;
                    }
                    
                    /* Case 2 : 특정 행의 0번 Index가 곧 id = key 값이 된다. key 중복을 체크한다. */
                    KeyType key = (KeyType) SystemTool.DecodeValue((trySet[0] as SecurityElement).Text, typeof(KeyType));
                    if (result.ContainsKey(key))
                    {
                        continue;
                    }

                    /* 특정 행의 1번 부터 ~ 끝 까지는 <tag, tagValue> = <프로퍼티명, 값>, 즉 2개의 문자열을 포함하는 SecurityElement 인스턴스 */
                    var children = new Dictionary<String, String>();
                    result.Add(key, children);
                    for (int i = 1; i < trySet.Count; i++)
                    {
                        var tagWrapper = trySet[i] as SecurityElement;
                        /* 현재 해당 프로젝트는 tag만 기술하고 있고, attribute는 기술되어 있지 않음 */
                        string tag = tagWrapper.Tag.Trim();
                        string tagValue = tagWrapper.Text == null ? string.Empty : tagWrapper.Text.Trim();
                        if (!children.ContainsKey(tag))
                        {
                            children.Add(tag, tagValue);
                        }
                    }
                }
            }
            
            return result;
        }

        /// <summary>
        /// 지정한 디렉터리에 있는 파일을 읽고 파일 타입에 따라 파싱하여 컬렉션으로 리턴하는 메서드
        /// </summary>
        public static async UniTask DecodeTable<KeyType, RecordType>(ITableBase p_Table, Dictionary<KeyType, Dictionary<String, String>> p_ParsedTable, Dictionary<KeyType, RecordType> r_Collection) where RecordType : ITableBaseRecord, new()
        {
            var tableRecordType = typeof(RecordType);
            
            /* 정적 멤버 외의 모든 필드를 가져온다. */
            var fieldInfoGroup = tableRecordType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            /* 정적 멤버 외의 모든 프로퍼티를 가져온다. */
            var propertyInfoGroup = tableRecordType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (var tryCollectionPair in p_ParsedTable)
            {
                /* 테이블 데이터 인스턴스 생성 */
                var tableDataInstance = new RecordType();
                var tryTableKey = tryCollectionPair.Key;
                var tryTableValue = tryCollectionPair.Value;

                /* 테이블 데이터로부터 테이블 데이터 인스턴스에 값을 담는다. */
                /* 필드 서치 */
                for (int i = 0; i < fieldInfoGroup.Length; i++)
                {
                    var tryFieldInfo = fieldInfoGroup[i];
                    var tryFieldName = tryFieldInfo.Name;
                    var tryFieldType = tryFieldInfo.FieldType;
                    
                    /* 필드명이 KEY인 경우, 키값을 세트한다. */
                    if (tryFieldName == TableTool.TableKeyFieldName)
                    {
                        tryFieldInfo.SetValue(tableDataInstance, tryCollectionPair.Key);
                    }
                    
                    /* 일치하는 필드명이 있다면 해당 필드에 값을 세트한다. */
                    if (tryTableValue.ContainsKey(tryFieldName))
                    {
                        var tryValue = tryTableValue[tryFieldName];
                        tryFieldInfo.SetValue(tableDataInstance, tryValue.DecodeValue(tryFieldType));
                    }
                }

                /* 프로퍼티 서치 */
                for (int i = 0; i < propertyInfoGroup.Length; i++)
                {
                    var tryPropertyInfo = propertyInfoGroup[i];
                    var tryPropertyName = tryPropertyInfo.Name;
                    var tryPropertyType = tryPropertyInfo.PropertyType;
                    
                    /* 필드명이 KEY인 경우, 키값을 세트한다. */
                    if (tryPropertyName == TableTool.TableKeyFieldName)
                    {
                        tryPropertyInfo.SetValue(tableDataInstance, tryCollectionPair.Key);
                    }
                    
                    /* 일치하는 프로퍼티명이 있다면 해당 프로퍼티에 값을 세트한다. */
                    if (tryTableValue.ContainsKey(tryPropertyName))
                    {
#if UNITY_EDITOR
                        try
                        {
                            var tryValue = tryTableValue[tryPropertyName];
                            tryPropertyInfo.SetValue(tableDataInstance, tryValue.DecodeValue(tryPropertyType));
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[{p_Table.GetTableFileName(TableTool.TableNameType.Alter, false)}] 테이블의 [{tryPropertyName}({tryPropertyType})] 필드의 접근 지정자를 확인해주십시오. : {tryTableValue[tryPropertyName]} {e.Message}");
                            throw;
                        }
#else
                        var tryValue = tryTableValue[tryPropertyName];
                        tryPropertyInfo.SetValue(tableDataInstance, tryValue.DecodeValue(tryPropertyType));
#endif
                    }
                }

                /* 테이블로부터 레코드가 디코드된 경우 */
                await tableDataInstance.OnRecordDecoded();
   
                /* 테이블 인스턴스를 컬렉션에 테이블 컬렉션에 담는다. */
                if (!r_Collection.ContainsKey(tryTableKey))
                {
                    r_Collection.Add(tryTableKey, tableDataInstance);
                }
            }
        }

        #endregion
    }
}