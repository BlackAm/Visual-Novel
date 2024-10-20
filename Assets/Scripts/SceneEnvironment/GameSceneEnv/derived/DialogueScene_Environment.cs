using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class DialogueScene_Environment : GamePlaySceneEnvironmentBase
    {
        public override void OnSceneStarted()
        {
            base.OnSceneStarted();
        
            DefaultUIManagerSet.GetInstanceUnSafe._MainLoadingBar.Set_UI_Hide(true);
            DefaultUIManagerSet.GetInstanceUnSafe._TouchPanel.Set_UI_Hide(false);
            DefaultUIManagerSet.GetInstanceUnSafe._MainGameUi.Set_UI_Hide(false);
            DefaultUIManagerSet.GetInstanceUnSafe._TopMenu.Set_UI_Hide(false);
        
            switch (SaveLoadManager.GetInstanceUnSafe.loadType)
            {
                case SaveLoadManager.LoadType.TitleLoad:
                    DialogueGameManager.GetInstance.SetDialogue(DialogueGameManager.GetInstance.CurrentDialogueKey);
                    SaveLoadManager.GetInstanceUnSafe.loadType = SaveLoadManager.LoadType.None;
                    break;
                default:
                    DialogueGameManager.GetInstance.StartScene();
                    break;
            }
        }
    }
}