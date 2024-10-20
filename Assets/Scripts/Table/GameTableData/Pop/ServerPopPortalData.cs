using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class ServerPopPortalData : GameTable<ServerPopPortalData, int, ServerPopPortalData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int KEY { get; set; }
            /// <summary>
            /// 맵 번호
            /// </summary>
            public int Map { get; set; }

            /// <summary>
            /// 포탈 위치
            /// </summary>
            public Vector3 PortalPosition { get; set; }

            /// <summary>
            /// 바라보는 방향
            /// </summary>
            public float PortalRotation { get; set; }
            
            /// <summary>
            /// 포탈 범위
            /// </summary>
            public float Range { get; set; }

            /// <summary>
            /// 텔레포트 키
            /// </summary>
            public int TeleportKey { get; set; }
        }

        public Dictionary<int, PrefabInstance> SpawnPortals(int mapIndex)
        {
            int a = mapIndex * 100;
            int b = 0;

            Dictionary<int, PrefabInstance> portals = new Dictionary<int, PrefabInstance>();


            for (int i = a; i < (a + 100); i++)
            {
                if (GetTable().ContainsKey(i))
                {
                    var data = GetTableData(i);

                    var portal = PrefabPoolingManager.GetInstance.PoolInstance("Portal", ResourceLifeCycleType.Scene, ResourceType.GameObjectPrefab, data.PortalPosition).Item1;

                    portals.Add(b, portal);
                    b++;
                }
                else
                {
                    break;
                }
            }

            return portals;
        }

        protected override string GetDefaultTableFileName()
        {
            return "PortalPlacementData_401";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}