#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using UnityEngine;
 
namespace k514
{
    public class ServerPopSoundData : GameTable<ServerPopSoundData, int, ServerPopSoundData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int KEY { get; set; }

            /// <summary>
            /// 맵 번호
            /// </summary>
            public int Map { get; set; }

            /// <summary>
            /// 사운드 키
            /// </summary>
            public int SoundIndex { get; set; }

            /// <summary>
            /// 범위(최소)
            /// </summary>
            public float RangeMin { get; set; }

            /// <summary>
            /// 범위(최대)
            /// </summary>
            public float RangeMax { get; set; }

            /// <summary>
            /// 환경음 위치
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// 바라보는 방향
            /// </summary>
            public float Rotation { get; set; }

            /// <summary>
            /// 사운드 유형
            /// </summary>
            public MapEffectSoundManager.MapEffectSound MapEffectSound { get; set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif