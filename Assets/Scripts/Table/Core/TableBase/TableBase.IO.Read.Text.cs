using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    public partial class TableBase<M, K, T>
    {
        /// <summary>
        /// 유니티 리소스 경로로부터 테이블 파일을 탐색하여 읽는 메서드
        /// </summary>
        protected async UniTask LoadTableText(bool p_InvokeFromSingletonInitiate)
        {
            if (p_InvokeFromSingletonInitiate)
            {
                CheckTable();
            }
            else
            {
                ClearTable();
            }

            var (valid, textAsset) = await TableLoader.ReadTableFile<K>(this);
            if (valid)
            {
                await UniTask.SwitchToMainThread();
                var rawText = textAsset.text;
                var parsedText = await TableLoader.ParsingTableFile<K>(GetInstanceUnSafe, rawText);
                await TableLoader.DecodeTable(GetInstanceUnSafe, parsedText, GetTable()).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                await OnTableLoaded(p_InvokeFromSingletonInitiate);
            }
        }
        
        /// <summary>
        /// 지정한 텍스트로부터 테이블을 디코딩하는 메서드
        /// </summary>
        protected async UniTask LoadTableWithRawText(string p_Text, bool p_InvokeFromSingletonInitiate)
        {
            if (p_InvokeFromSingletonInitiate)
            {
                CheckTable();
            }
            else
            {
                ClearTable();
            }

            if (p_Text != null)
            {
                var parsedText = await TableLoader.ParsingTableFile<K>(GetInstanceUnSafe, p_Text);
                await TableLoader.DecodeTable(GetInstanceUnSafe, parsedText, GetTable()).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                await OnTableLoaded(p_InvokeFromSingletonInitiate);
            }
        }
                
        /// <summary>
        /// 지정한 절대 경로로부터 테이블 파일을 탐색하여 읽어 테이블 컬렉션을 리턴하는 메서드
        /// ** 해당 테이블에 로드하는게 아니라 그냥 컬렉션을 만들어 리턴하는 메서드이다.
        /// </summary>
        public async UniTask<Dictionary<K, T>> CreateTableFromAbsolutePath(string p_TableAbsolutePath)
        {
            var targetFileName = p_TableAbsolutePath + GetTableFileName(TableTool.TableNameType.Alter, true);

            if (!Directory.Exists(p_TableAbsolutePath))
            {
                Directory.CreateDirectory(p_TableAbsolutePath);
            }

            if (File.Exists(targetFileName))
            {
                var rawText = File.ReadAllText(targetFileName);
                var parsedText = await TableLoader.ParsingTableFile<K>(GetInstanceUnSafe, rawText).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                var tryTable = new Dictionary<K, T>();
                await TableLoader.DecodeTable(this as M, parsedText, tryTable);
                
                return tryTable;
            }
            else
            {
                return null;
            }
        }
    }
}