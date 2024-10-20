using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class DialoguePlayModeSkipPlayPresetData : DialoguePlayModeDataBase<DialoguePlayModeSkipPlayPresetData, DialoguePlayModeSkipPlayPresetData.PlayModeTableRecord>
    {
        public class PlayModeTableRecord : PlayModeTableBaseRecord
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialoguePlayModeSkipPlayPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 200;
            EndIndex = 300;
        }

        public override DialogueGameManager.DialoguePlayMode GetThisLabelType()
        {
            return DialogueGameManager.DialoguePlayMode.SkipPlay;
        }
    }

}