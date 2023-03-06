using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace k514
{
    public class UnitHitParameterData : GameTable<UnitHitParameterData, int, UnitHitParameterData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public UnitHitTool.HitParameterType HitParameterType;
            public Dictionary<UnitHitTool.HitParameterType, int> HitParameterMap;

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                if (ReferenceEquals(null, HitParameterMap))
                {
                    HitParameterType = UnitHitTool.HitParameterType.None;
                }
                else
                {
                    HitParameterType = UnitHitTool.HitParameterType.None;
                    foreach (var _hitParameterType in UnitHitTool._HitParameterTypeEnumerator)
                    {
                        if (HitParameterMap.TryGetValue(_hitParameterType, out var o_Index))
                        {
                            if (o_Index != default)
                            {
                                HitParameterType.AddFlag(_hitParameterType);
                            }
                        }
                    }
                }
            }
            
            public (bool, int) GetHitParameter(UnitHitTool.HitParameterType p_Type)
            {
                if (HitParameterType.HasAnyFlagExceptNone(p_Type))
                {
                    return (true, HitParameterMap[p_Type]);
                }
                else
                {
                    return default;
                }
            }
            
            public (bool, UnitHitExtraData.TableRecord) GetHitExtraRecord()
            {
                var hitExtraTuple = GetHitParameter(UnitHitTool.HitParameterType.HitExtra);
                if (hitExtraTuple.Item1)
                {
                    return (true, UnitHitExtraData.GetInstanceUnSafe[hitExtraTuple.Item2]);
                }
                else
                {
                    return default;
                }
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitHitParameterTable";
        }
    }
}