using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UI2020;
using UnityEngine;

namespace k514
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
            StartIndex = 90000;
            EndIndex = 140000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.GamePropertyLanguage;
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
            StartIndex = 60000;
            EndIndex = 90000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.ScenarioLanguage;
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
            StartIndex = 140000;
            EndIndex = 190000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.GameContentLanguage;
        }
    }
    
    public class SkillPropertyLanguage : BaseLanguageData<SkillPropertyLanguage, SkillPropertyLanguage.LanguageContent>
    {
        public class LanguageContent : BaseLanguageContent
        {
            public string SkillDescription { get; set; }
            public string SkillFlavorText { get; set; }
            public int SkillIconSpriteIndex { get; set; }
            public string NeedLevel { get; set; }
            public string Ability { get; set; }
            public float Mana { get; set; }
            public float CoolTime { get; set; }
        }

#if !SERVER_DRIVE
        public (AssetPreset, Sprite) GetSkillIconSprite(int p_Key)
        {
            return ImageNameTableData.GetInstanceUnSafe.GetResource(GetTableData(p_Key).SkillIconSpriteIndex, ResourceType.Image, ResourceLifeCycleType.Scene);
        }
#endif

        protected override string GetDefaultTableFileName()
        {
            return "SkillPropertyLanguage";
        } 
        
        public override void InitIntervalIndex()
        {
            StartIndex = 190000;
            EndIndex = 200000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.SkillPropertyLanguage;
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
            StartIndex = 200000;
            EndIndex = 220000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.UILanguage;
        }
    }
    
    public class ItemPropertyLanguage : BaseLanguageData<ItemPropertyLanguage, ItemPropertyLanguage.LanguageContent>
    {
        public class LanguageContent : BaseLanguageContent
        {
            public string itemDescription { get; set; }
            public int inventoryIconSpriteIndex { get; set; }
            public int multySpriteNumber { get; set; }
        }

#if !SERVER_DRIVE
        public async UniTask<(AssetPreset, Sprite)> GetItemIconSprite(int p_Key)
        {
            return (await ImageNameTableData.GetInstance()).GetResource(GetTableData(p_Key).inventoryIconSpriteIndex, ResourceType.Image, ResourceLifeCycleType.Scene);
        }
#endif

        protected override string GetDefaultTableFileName()
        {
            return "ItemPropertyLanguage";
        } 
        
        public override void InitIntervalIndex()
        {
            StartIndex = 10000000;
            EndIndex = 50000000;
        }

        public override LanguageManager.LanguageDataType GetThisLabelType()
        {
            return LanguageManager.LanguageDataType.ItemLanguage;
        }
    }
}