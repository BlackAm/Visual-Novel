using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public class PrefabModelData_Vfx : PrefabModelDataIntTable<PrefabModelData_Vfx, PrefabModelData_Vfx.TableRecord>, PrefabModelDataTableBridge
    {
        public class TableRecord : PrefabModelDataRecord
        {
            public List<string> VfxName { get; private set; }
            private bool HasMultiVfx;

            public override string GetPrefabName()
            {
                return HasMultiVfx ? VfxName[Random.Range(0, VfxName.Count)] : PrefabName;
            }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                if (VfxName != null)
                {
                    HasMultiVfx = VfxName.Count > 1;
                    if (!HasMultiVfx)
                    {
                        PrefabName = VfxName[0];
                    }
                }
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "VfxPrefabDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 30000;
            EndIndex = 100000;
        }

        public override PrefabModelDataRoot.PrefabModelDataType GetThisLabelType()
        {
            return PrefabModelDataRoot.PrefabModelDataType.VFX;
        }
    }
}