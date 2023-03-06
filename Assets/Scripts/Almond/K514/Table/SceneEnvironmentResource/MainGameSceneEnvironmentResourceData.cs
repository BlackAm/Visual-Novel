using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using k514;
using UnityEngine;

public class MainGameSceneEnvironmentResourceData : GameTable<MainGameSceneEnvironmentResourceData, string, MainGameSceneEnvironmentResourceData.TableRecord>
{
    private Dictionary<string, object> _loadedResources;
    private Dictionary<string, bool> _loadedCheck;

    public class TableRecord : GameTableRecordBase
    {
        public string type;
    }

    protected override async UniTask OnCreated()
    {
        await base.OnCreated();
        _loadedResources = new Dictionary<string, object>();
        _loadedCheck = new Dictionary<string, bool>();
    }

    public async Task<T> GetResource<T>(string resourceName)
    {
        if (_loadedCheck[resourceName])
            return (T) _loadedResources[resourceName];

        await Task.Run(WaitResourceLoad);
        
        return (T) _loadedResources[resourceName];
    }
    
    public void ResourcePreLoad()
    {
        foreach (var tableRecord in _Table.Values)
        {
            _loadedCheck[tableRecord.KEY] = false;
            switch (tableRecord.type)
            {
                case "sprite":
                    var loadResult = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(
                        ResourceType.Image, ResourceLifeCycleType.Scene,
                        tableRecord.KEY);
            
                    _loadedResources[tableRecord.KEY] = loadResult.Item1.Asset;
                    _loadedCheck[tableRecord.KEY] = true;   
                    break;
            }
        }
    }

    public async void WaitResourceLoad()
    {
        bool loaded = false;
        while (!loaded)
        {
            loaded = true;
            foreach (var isLoaded in _loadedCheck.Values)
            {
                if (!isLoaded) loaded = false;
            }
        }
    }

    public override TableTool.TableFileType GetTableFileType()
    {
        return TableTool.TableFileType.Xml;
    }

    protected override string GetDefaultTableFileName()
    {
        return "MainGameSceneEnvironmentResource";
    }
}