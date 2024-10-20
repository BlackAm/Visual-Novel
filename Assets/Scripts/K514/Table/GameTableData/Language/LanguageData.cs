using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public abstract class BaseLanguageData<M, T> 
        : MultiTableBase<M, int, T, LanguageManager.LanguageDataType, IIndexableLanguageRecordBridge>, IIndexableLanguageTableBridge
        where M : BaseLanguageData<M, T>, new() 
        where T : BaseLanguageData<M, T>.BaseLanguageContent, new()
    {
        public abstract class BaseLanguageContent : GameTableRecordBase, IIndexableLanguageRecordBridge
        {
            public string content { get; protected set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                if (!string.IsNullOrEmpty(content) && content.Contains("\\n"))
                {
                    content = content.Replace("\\n", "\n");
                }
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
        
        public override MultiTableIndexer<int, LanguageManager.LanguageDataType, IIndexableLanguageRecordBridge> GetMultiGameIndex()
        {
            return LanguageManager.GetInstanceUnSafe.GameDataTableCluster;
        }
    }

    public class GameSystemLanguage : BaseLanguageData<GameSystemLanguage, GameSystemLanguage.LanguageContent>
    {
        public class LanguageContent : BaseLanguageContent
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "GameSystemLanguage";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 10000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.SystemLanguage;
        }
    }

    public class GameProperNameLanguage : BaseLanguageData<GameProperNameLanguage, GameProperNameLanguage.LanguageContent>
    {
        public class LanguageContent : BaseLanguageContent
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "GameProperNameLanguage";
        }
        
        public override void InitIntervalIndex()
        {
            StartIndex = 10000;
            EndIndex = 20000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.GamePropertyLanguage;
        }
    }
    
    
    
    public class UILanguage : BaseLanguageData<UILanguage, UILanguage.LanguageContent>
    {
        public class LanguageContent : BaseLanguageContent
        {
            public List<string> ContentSet;
            
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                if (string.IsNullOrEmpty(content))
                {
                    if (ContentSet.CheckGenericCollectionSafe())
                    {
                        content = ContentSet[0];
                    }
                    else
                    {
                        content = String.Empty;
                    }
                }
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UILanguage";
        } 
        
        public override void InitIntervalIndex()
        {
            StartIndex = 20000;
            EndIndex = 30000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.UILanguage;
        }
    }
        
    public class GameContentsLanguage : BaseLanguageData<GameContentsLanguage, GameContentsLanguage.LanguageContent>
    {
        public class LanguageContent : BaseLanguageContent
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "GameContentsLanguage";
        } 
        
        public override void InitIntervalIndex()
        {
            StartIndex = 30000;
            EndIndex = 40000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.GameContentLanguage;
        }
    }
        
    public class ScenarioLanguage : BaseLanguageData<ScenarioLanguage, ScenarioLanguage.LanguageContent>
    {
        public class LanguageContent : BaseLanguageContent
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "ScenarioLanguage";
        }
        
        public override void InitIntervalIndex()
        {
            StartIndex = 40000;
            EndIndex = 100000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.ScenarioLanguage;
        }
    }
}