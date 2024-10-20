using System;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
namespace BlackAm
{
    /// <summary>
    /// 마을 씬
    /// </summary>
    public class MainGameZone_SceneEnvironment : LamierePlayerDeploySceneEnvironment
    {
        public override void OnSceneStarted()
        {
            base.OnSceneStarted();
#if !SERVER_DRIVE
            DefaultUIManagerSet.GetInstanceUnSafe._MainLoadingBar.Set_UI_Hide(true);
            DefaultUIManagerSet.GetInstanceUnSafe._TouchPanel.Set_UI_Hide(false);
            DefaultUIManagerSet.GetInstanceUnSafe._MainGameUi.Set_UI_Hide(false);
            //DefaultUIManagerSet.GetInstanceUnSafe._MainGameUi.mainUI.SetMiniMap();
#endif
        }
    }
}