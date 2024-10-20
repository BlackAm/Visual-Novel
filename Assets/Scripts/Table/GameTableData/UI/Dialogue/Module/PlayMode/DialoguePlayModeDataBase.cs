using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public abstract class DialoguePlayModeDataBase<M, T> : MultiTableBase<M, int, T, DialogueGameManager.DialoguePlayMode, IPlayModeTableRecordBridge>, IPlayModeTableBridge
    where M : DialoguePlayModeDataBase<M, T>, new()
    where T : DialoguePlayModeDataBase<M, T>.PlayModeTableBaseRecord, new()
    {
        public abstract class PlayModeTableBaseRecord : GameTableRecordBase, IPlayModeTableRecordBridge
        {
            public uint PreDelay { get; protected set; }
        }

        public override MultiTableIndexer<int, DialogueGameManager.DialoguePlayMode, IPlayModeTableRecordBridge> GetMultiGameIndex()
        {
            return DialoguePlayModeDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }
        
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}