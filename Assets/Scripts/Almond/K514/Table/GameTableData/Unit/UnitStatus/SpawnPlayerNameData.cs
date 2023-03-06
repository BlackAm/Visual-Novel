using System.Collections;
using System.Collections.Generic;
using k514;
using UnityEngine;

public class SpawnPlayerNameData : GameTable<SpawnPlayerNameData, int ,SpawnPlayerNameData.TableRecord>
{
    public class TableRecord : GameTableRecordBase
    {
        #region <Fields>

        public string Name { get; private set; }

        #endregion
    }

    protected override string GetDefaultTableFileName()
    {
        return "SpawnPlayerNameDataTable";
    }
    
    public override TableTool.TableFileType GetTableFileType()
    {
        return TableTool.TableFileType.Xml;
    }
}
