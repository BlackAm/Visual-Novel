using System;
using System.Collections.Generic;
using UnityEngine;
 
namespace k514
{
    public class ServerPopTeleportData : GameTable<ServerPopTeleportData, int, ServerPopTeleportData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int KEY { get; set; }
            /// <summary>
            /// 맵 번호
            /// </summary>
            public int Map { get; set; }

            /// <summary>
            /// 텔레포트 위치
            /// </summary>
            public Vector3 TeleportPosition { get; set; }

            /// <summary>
            /// 바라보는 방향
            /// </summary>
            public float TeleportRotation { get; set; }
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