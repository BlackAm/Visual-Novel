#if !SERVER_DRIVE

using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace k514
{
    public class UnitHitUISkinData : GameTable<UnitHitUISkinData, int, UnitHitUISkinData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Dictionary<UnitHitTool.HitResultType, int> HitSkinMap { get; private set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                var enumerator = UnitHitTool._HitResultTypeEnumerator;
                if (ReferenceEquals(null, HitSkinMap))
                {
                    HitSkinMap = new Dictionary<UnitHitTool.HitResultType, int>();
                    foreach (var hitResultType in enumerator)
                    {
                        HitSkinMap.Add(hitResultType, default);
                    }
                }
                else
                {
                    foreach (var hitResultType in enumerator)
                    {
                        if (!HitSkinMap.ContainsKey(hitResultType))
                        {
                            HitSkinMap.Add(hitResultType, default);
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
            return "UnitHitUISkinTable";
        }
    }
}
#endif
