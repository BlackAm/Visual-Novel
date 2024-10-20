using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class SaveLoadManager
    {
        public async UniTask SaveGame(SaveLoad.SaveLoadType p_SaveLoadType, SaveData p_SaveData, string p_FilePath)
        {
            await SaveImageFileInDirectory(p_SaveLoadType, p_FilePath);

            SaveDataFileInDirectory(p_SaveData, p_FilePath);
        }
    }
}
