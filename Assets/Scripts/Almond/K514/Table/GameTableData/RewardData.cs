using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace k514
{
    public class RewardData : GameTable<RewardData, int, RewardData.Data>
    {
        public class Data : GameTableRecordBase
        {
            public List<(int itemIndex, int amount)> Item { get; private set; } //변경 예정
            //public List<int> itemIndex { get; private set; }
            //public List<int> amount { get; private set; }
            public int Exp { get; private set; }
            public int Coin { get; private set; }
            public int Diamond { get; private set; }
        }
        
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "RewardTable";
        }
    }
}