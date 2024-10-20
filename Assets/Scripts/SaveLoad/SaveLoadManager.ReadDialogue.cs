using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BlackAm
{
    public partial class SaveLoadManager
    {
        #region <Fields>

        public ReadDialogueKey DialogueDataFile;

        #endregion

        #region <Callbacks>

        public void OnReadDialogueCreated()
        {
            var readDialogueData = GetDataFileInDirectory<ReadDialogueKey>(SystemMaintenance.SaveReadDialogueFile);
            if (readDialogueData.Item1)
            {
                DialogueDataFile = readDialogueData.Item2;
            }
            else
            {
                MakeReadDialogueFile();
            }

            var isDialogueAdded = CheckReadDialogueFile();
            if (!isDialogueAdded)
            {
                UpdateReadDialogueFile();
            }
        }

        #endregion

        #region <Methods>

        public void MakeReadDialogueFile()
        {
            DialogueDataFile = new ReadDialogueKey();

            var scenarioKeys = ScenarioLanguage.GetInstanceUnSafe.GetValidKeyEnumerator();
            foreach (var key in scenarioKeys)
            {
                DialogueDataFile.ReadDialogueInfo.Add(key, false);
            }
        }

        public bool CheckReadDialogueFile()
        {
            return DialogueDataFile.ReadDialogueInfo.Count !=
                   ScenarioLanguage.GetInstanceUnSafe.GetValidKeyEnumerator().Count;
        }

        public void UpdateReadDialogueFile()
        {
            var scenarioKeys = ScenarioLanguage.GetInstanceUnSafe.GetValidKeyEnumerator();
            foreach (var key in scenarioKeys)
            {
                if (!DialogueDataFile.ReadDialogueInfo.ContainsKey(key))
                {
                    DialogueDataFile.ReadDialogueInfo.Add(key, false);
                }
            }
        }

        #endregion
    }
}