using System;
using System.Collections.Generic;
using UnityEngine;
 
namespace k514
{
    public class ItemDecomData : GameTable<ItemDecomData, int, ItemDecomData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public List<(int, int)> GetItemList { get; protected set; }//(재료 키, 갯수)
        }

        protected override string GetDefaultTableFileName()
        {
            return "ItemDecomDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}