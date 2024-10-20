using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class DialoguePlayModeAutoPlayPresetData : DialoguePlayModeDataBase<DialoguePlayModeAutoPlayPresetData, DialoguePlayModeAutoPlayPresetData.PlayModeTableRecord>
    {
        public class PlayModeTableRecord : PlayModeTableBaseRecord
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialoguePlayModeAutoPlayPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100;
            EndIndex = 200;
        }

        public override DialogueGameManager.DialoguePlayMode GetThisLabelType()
        {
            return DialogueGameManager.DialoguePlayMode.AutoPlay;
        }
    }
}
