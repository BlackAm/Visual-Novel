#if !SERVER_DRIVE
using System.Collections;
using System.Collections.Generic;
using k514;
using UI2020;
using UnityEngine;

public class InformationData : GameTable<InformationData, int, InformationData.TableData>
{
    public class TableData : GameTableRecordBase
    {
        // npc, 타겟UI, 대화내용.
        public string NPC { get; protected set; }
        public string Target { get; protected set; }
        public string Script { get; protected set; }
    }

    public override TableTool.TableFileType GetTableFileType()
    {
        return TableTool.TableFileType.Xml;
    }

    protected override string GetDefaultTableFileName()
    {
        return "InformationTable";
    }
}

#endif