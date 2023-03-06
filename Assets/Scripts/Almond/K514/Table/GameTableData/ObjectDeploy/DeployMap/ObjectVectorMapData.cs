using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class ObjectVectorMapData : GameTable<ObjectVectorMapData, int, ObjectVectorMapData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public List<(int t_DeployIndex, ObjectDeployTimePreset t_DeployTimePreset)> DeployEventPreset { get; private set; }
            public ObjectDeployEventPreset DeployEventPresetMap { get; private set; }
            public float EventDuration { get; private set; }
            
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                var _lastDeployTimePreset = default(ObjectDeployTimePreset);
                var eventMap =
                    new Dictionary<ObjectDeployTimePreset, List<(ObjectDeployIndexPreset t_DeployDataIndexPreset,
                        Dictionary<ObjectDeployTool.DeployableType, List<int>>)>>();

                var firstStamp = int.MaxValue;
                var lastStamp = 0;
                
                foreach (var tuple in DeployEventPreset)
                {
                    var eventList =
                        new List<(ObjectDeployIndexPreset t_DeployDataIndexPreset,
                            Dictionary<ObjectDeployTool.DeployableType, List<int>>)>
                        {
                            (
                                new ObjectDeployIndexPreset(tuple.t_DeployIndex),
                                new Dictionary<ObjectDeployTool.DeployableType, List<int>>
                                {
                                    { ObjectDeployTool.DeployableType.None, null }
                                }
                            )
                        };

                    var tryDeployTimePreset = tuple.t_DeployTimePreset;
                    tryDeployTimePreset.AddOffset(_lastDeployTimePreset.GetLastStamp());
                    firstStamp = Mathf.Min(firstStamp, tryDeployTimePreset.GetFirstStamp());
                    lastStamp = Mathf.Max(firstStamp, tryDeployTimePreset.GetLastStamp());
                    
                    eventMap.Add(tryDeployTimePreset, eventList);
                    _lastDeployTimePreset = tryDeployTimePreset;
                }
                
                DeployEventPresetMap = new ObjectDeployEventPreset(eventMap, default);
                EventDuration = 0.001f * (lastStamp - firstStamp);
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "ObjectVectorMapDataTable";
        }
    }
}