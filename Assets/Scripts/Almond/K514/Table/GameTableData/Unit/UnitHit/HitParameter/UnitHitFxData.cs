#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public class UnitHitFxData : GameTable<UnitHitFxData, int, UnitHitFxData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Dictionary<UnitHitTool.HitResultType, (int t_VfxIndex, int t_SfxIndex)> HitFxMap { get; private set; }
            
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                var enumerator = UnitHitTool._HitResultTypeEnumerator;
                if (ReferenceEquals(null, HitFxMap))
                {
                    HitFxMap = new Dictionary<UnitHitTool.HitResultType, (int t_VfxIndex, int t_SfxIndex)>();
                    foreach (var hitResultType in enumerator)
                    {
                        HitFxMap.Add(hitResultType, default);
                    }
                }
                else
                {
                    foreach (var hitResultType in enumerator)
                    {
                        if (!HitFxMap.ContainsKey(hitResultType))
                        {
                            HitFxMap.Add(hitResultType, default);
                        }
                    }
                }
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitHitFxTable";
        }
    }
}
#endif
