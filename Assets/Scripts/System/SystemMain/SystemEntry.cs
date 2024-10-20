using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public static class SystemEntry
    {
        #region <Consts>

        public const SystemBootEntryMode DEFAULT_ENTRY_MODE = SystemBootEntryMode.MultiNetwork;
        public const int DEFAULT_ENTRY_INDEX = 0;
        public const string DEFAULT_ENTRY_ID = "DefaultName";
        public const string DEFAULT_ENTRY_PW = "DefaultPassword";

        #endregion

        #region <Fields>

        public static SystemBootEntryPreset SystemEntryPreset;

        #endregion

        #region <Enums>

        public enum SystemBootEntryMode
        {
            SingleNetwork,
            MultiNetwork,
            MultiNetworkForTest,
        }

        #endregion

        #region <Callbacks>

#if UNITY_EDITOR
        public static async UniTask OnUpdateSystemBootEntry()
        {
            var tryMode = SystemEntryPreset.EntryMode;
            await SystemBootEntryData.GetInstanceUnSafe.ReplaceRecord(tryMode, SystemEntryPreset);
            await SystemBootEntryData.GetInstanceUnSafe.UpdateTableFile(ExportDataTool.WriteType.Overlap);
            
            CustomDebug.LogWarning("SystemEntry", $"SceneEntryPreset이 갱신되었습니다.\n{SystemEntryPreset}");
        }
#endif
        public static void OnEntryScene()
        {
            SceneControllerManager.GetInstance.TurnSceneTo(SceneControllerTool.SceneControllerShortCutType.MainHomeScene, (SceneControllerTool.LoadingSceneType.Black, SceneControllerTool.SceneControlFlag.SystemBootInvoke));
/*#if UNITY_EDITOR
            CustomDebug.LogWarning("SystemEntry", SystemEntryPreset.ToString());
#endif
            switch (SystemEntryPreset.EntryMode)
            {
                case SystemBootEntryMode.SingleNetwork:
                    LamiereGameManager.GetInstanceUnSafe.SelectPlayer(Vocation.KNIGHT);
                    SceneControllerManager.GetInstance.TurnSceneTo(901, (SceneControllerTool.LoadingSceneType.Black, SceneControllerTool.SceneControlFlag.SystemBootInvoke));
                    break;
                case SystemBootEntryMode.MultiNetwork:
                    SceneControllerManager.GetInstance.TurnSceneTo(SceneControllerTool.SceneControllerShortCutType.LoginScene, (SceneControllerTool.LoadingSceneType.Black, SceneControllerTool.SceneControlFlag.SystemBootInvoke));
                    break;
                case SystemBootEntryMode.MultiNetworkForTest:
                    SceneControllerManager.GetInstance.TurnSceneTo(SceneControllerTool.SceneControllerShortCutType.LoginScene, (SceneControllerTool.LoadingSceneType.Black, SceneControllerTool.SceneControlFlag.SystemBootInvoke));
                    break;
            }*/
        }
        
        #endregion

        #region <Methods>

        public static async UniTask<SystemBootEntryPreset> TryGetEntryPreset()
        {
            if (ReferenceEquals(null, SystemEntryPreset))
            {
#if UNITY_EDITOR
                var entryTable = await SystemBootEntryData.GetInstance();
                if (ReferenceEquals(null, SystemEntryPreset))
                {
                    SystemEntryPreset = entryTable[DEFAULT_ENTRY_MODE].SystemEntryPreset;
                    CustomDebug.LogWarning("SystemEntry", $"SceneEntryPreset이 초기화되었습니다.\n{SystemEntryPreset}");
                }
#else
                SystemEntryPreset = new SystemBootEntryPreset(DEFAULT_ENTRY_MODE);
#endif
            }

            return SystemEntryPreset;
        }

        #endregion
        
        #region <Class>

        public class SystemBootEntryPreset
        {
            #region <Fields>

            public SystemBootEntryMode EntryMode;
            public int EntryIndex;
            public string EntryKey;
            public string EntryKey2;

            #endregion

            #region <Constructor>

            public SystemBootEntryPreset(SystemBootEntryMode p_EntryMode) : this(p_EntryMode, 0, string.Empty, string.Empty)
            {
            }
        
            public SystemBootEntryPreset(SystemBootEntryMode p_EntryMode, int p_EntryIndex) : this(p_EntryMode, p_EntryIndex, string.Empty, string.Empty)
            {
            }
        
            public SystemBootEntryPreset(SystemBootEntryMode p_EntryMode, int p_EntryIndex, string p_EntryKey, string p_EntryKey2)
            {
                EntryMode = p_EntryMode;
                EntryIndex = p_EntryIndex;
                EntryKey = p_EntryKey;
                EntryKey2 = p_EntryKey2;
            }

            #endregion

            #region <Operator>

            public override string ToString()
            {
                switch (EntryMode)
                {
                    default:
                    case SystemBootEntryMode.SingleNetwork:
                        return $"[{EntryMode}](Entry Scene Index : [{EntryIndex}])";
                    case SystemBootEntryMode.MultiNetwork:
                        return $"[{EntryMode}](ID : {EntryKey}, PW : {EntryKey2}, Entry Scene Index : [{EntryIndex}])";
                    case SystemBootEntryMode.MultiNetworkForTest:
                        return $"[{EntryMode}](ID : {EntryKey}, PW : {EntryKey2}, Entry Scene Index : [{EntryIndex}])";
                }
            }

            #endregion
        }

        #endregion
    }
}