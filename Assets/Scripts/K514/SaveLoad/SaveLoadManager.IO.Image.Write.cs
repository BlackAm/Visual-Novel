using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class SaveLoadManager
    {
        public async UniTask SaveImageFileInDirectory(SaveLoad.SaveLoadType p_SaveLoadType, string p_FilePath)
        {
            switch (p_SaveLoadType)
            {
                case SaveLoad.SaveLoadType.Normal:
                    SaveLoad.Instance.SetActiveUI(false);
            
                    await CaptureScreen(p_FilePath);
            
                    SaveLoad.Instance.SetActiveUI(true);
                    break;
                case SaveLoad.SaveLoadType.Quick:
                    await CaptureScreen(p_FilePath);
                    break;
            }
        }
        
        public async UniTask CaptureScreen(string p_FilePath)
        {
            ScreenCapture.CaptureScreenshot($"{SystemMaintenance.SaveDataImageDirectory}{p_FilePath}.png");
            await UniTask.WaitUntil(() => File.Exists($"{SystemMaintenance.SaveDataImageDirectory}{p_FilePath}.png"));
        }
    }
}
