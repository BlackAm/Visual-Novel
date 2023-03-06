using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using k514;
using UnityEngine;

public class LoginSceneEnvironmentResourceData : GameTable<LoginSceneEnvironmentResourceData, string, LoginSceneEnvironmentResourceData.TableRecord>
{
    private Dictionary<string, object> _loadedResources;

    public class TableRecord : GameTableRecordBase
    {
        public string type;
    }

    protected override async UniTask OnCreated()
    {
        await base.OnCreated();
        
        _loadedResources = new Dictionary<string, object>();
    }

    public T GetResource<T>(string resourceName)
    {
        return (T)_loadedResources[resourceName];
    }

    public Sprite GetSprite(string resourceName)
    {
        return _loadedResources[resourceName] as Sprite;
    }
    
    public void ResourcePreLoad()
    {
        foreach (var tableRecord in _Table.Values)
        {
            switch (tableRecord.type)
            {
                case "sprite":
                    var loadResult = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(
                        ResourceType.Image, ResourceLifeCycleType.Scene,
                        tableRecord.KEY);
            
                    if(_loadedResources.ContainsKey(tableRecord.KEY)) return;
                    _loadedResources.Add(tableRecord.KEY, loadResult.Item1.Asset);
                    break;
            }

        }
    }

    public override TableTool.TableFileType GetTableFileType()
    {
        return TableTool.TableFileType.Xml;
    }

    protected override string GetDefaultTableFileName()
    {
        return "LoginSceneEnvironmentResource";
    }
}