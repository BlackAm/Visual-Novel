using System;
using System.Collections.Generic;
using UnityEngine;
 
namespace k514
{
    public class ItemRecipeData : GameTable<ItemRecipeData, int, ItemRecipeData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int ProductionItemData { get; protected set; }
            public List<(int, int)> ItemRecipeList { get; protected set; }//(재료 키, 갯수)
            public int CostGold { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ItemRecipeDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}