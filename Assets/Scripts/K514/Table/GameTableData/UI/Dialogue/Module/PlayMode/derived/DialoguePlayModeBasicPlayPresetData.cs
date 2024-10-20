using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class DialoguePlayModeBasicPlayPresetData : DialoguePlayModeDataBase<DialoguePlayModeBasicPlayPresetData, DialoguePlayModeBasicPlayPresetData.PlayModeTableRecord>
    {
        public class PlayModeTableRecord : PlayModeTableBaseRecord
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialoguePlayModeBasicPlayPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 100;
        }

        public override DialogueGameManager.DialoguePlayMode GetThisLabelType()
        {
            return DialogueGameManager.DialoguePlayMode.BasicPlay;
        }
    }
}
