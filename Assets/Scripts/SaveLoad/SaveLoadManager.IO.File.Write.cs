using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class SaveLoadManager
    {
        public void SaveDataFileInDirectory(SaveData p_SaveData, string p_FilePath)
        {
            p_SaveData.SerializeObject($"{SystemMaintenance.SaveDataFileDirectory}{p_FilePath}");
        }
        
        public void SaveReadDialogueFile()
        {
            SaveReadDialogueFileInDirectory();
        }

        public void SaveReadDialogueFileInDirectory()
        {
            DialogueDataFile.SerializeObject(SystemMaintenance.SaveReadDialogueFile);
        }

        public void SaveGalleryInfoFile()
        {
            SaveGalleryInfoFileInDirectory();
        }

        public void SaveGalleryInfoFileInDirectory()
        {
            gallerySaveData.SerializeObject(SystemMaintenance.SaveGalleryFile);
        }
    }
}
