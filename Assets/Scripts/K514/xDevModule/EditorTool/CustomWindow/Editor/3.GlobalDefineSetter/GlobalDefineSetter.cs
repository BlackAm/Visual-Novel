
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace BlackAm
{
    /// <summary>
    /// PlayerSettings 의 예약 전처리기를 스크립트로 제어하도록 하는 싱글톤 매니저 클래스
    /// </summary>
    public class GlobalDefineSetter : Singleton<GlobalDefineSetter>
    {
        #region <Consts>

        private static readonly GlobalDefineTable.GlobalDefineType[] DefaultGlobalDefineType =
            {GlobalDefineTable.GlobalDefineType.ON_GUI};

        #endregion
        
        #region <Fields>

        public List<string> _CurrentGlobalDefineSet { get; private set; }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
        }

        public override void OnInitiate()
        {
            _CurrentGlobalDefineSet = new List<string>();
            InitGlobalDefineSet();  
        }

        #endregion

        #region <Methods>
        
        public void InitGlobalDefineSet()
        {
            _CurrentGlobalDefineSet.Clear();
            var defaultTargetBuildPlatform = SystemMaintenance.BuildTargetGroups.First();
            var defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(defaultTargetBuildPlatform);
            _CurrentGlobalDefineSet = defineSymbol == String.Empty ? new List<string>() : defineSymbol.Split(';').ToList();
        }
        
        public void AddDefine(string p_TargetDefineName)
        {
            _CurrentGlobalDefineSet.Add(p_TargetDefineName);
            var symbolDefine = string.Empty;
            var onceFlag = false;
            foreach (var define in _CurrentGlobalDefineSet)
            {
                if (!onceFlag)
                {
                    onceFlag = true;
                    symbolDefine += define;
                }
                else
                {
                    symbolDefine += $";{define}";
                }
            }
            var defaultTargetBuildPlatform = SystemMaintenance.BuildTargetGroups;
            foreach (var targetPlatform in defaultTargetBuildPlatform)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetPlatform, symbolDefine);
            }
        }
        
        public void RemoveDefine(string p_TargetDefineName)
        {
            _CurrentGlobalDefineSet.Remove(p_TargetDefineName);
            var symbolDefine = string.Empty;
            var onceFlag = false;
            foreach (var define in _CurrentGlobalDefineSet)
            {
                if (!onceFlag)
                {
                    onceFlag = true;
                    symbolDefine += define;
                }
                else
                {
                    symbolDefine += $";{define}";
                }
            }
            var defaultTargetBuildPlatform = SystemMaintenance.BuildTargetGroups;
            foreach (var targetPlatform in defaultTargetBuildPlatform)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetPlatform, symbolDefine);
            }
        }
        
        public void ClearDefine()
        {
            var defaultTargetBuildPlatform = SystemMaintenance.BuildTargetGroups;
            foreach (var targetPlatform in defaultTargetBuildPlatform)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetPlatform, string.Empty);
            }
            
            InitGlobalDefineSet();
            foreach (var defaultGlobalDefineType in DefaultGlobalDefineType)
            {
                AddDefine(defaultGlobalDefineType.ToString());
            }
        }

        #endregion
    }
}
#endif