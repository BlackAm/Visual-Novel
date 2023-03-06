using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace k514
{
    public class UIParticleEffectData : GameTable<UIParticleEffectData, int, UIParticleEffectData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            /// <summary>
            /// VfxSpawnDataTable
            /// </summary>
            public int VfxEffect { get; private set; }
            
            /// <summary>
            /// 스킬 쿨타임
            /// </summary>
            public Vector3 Position {get; private set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UIParticleEffectDataTable";
        }
    }
}