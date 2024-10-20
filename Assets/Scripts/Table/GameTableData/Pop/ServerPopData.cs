using UnityEngine;
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 서버에서 사용하는 팝관련 테이블
    /// </summary>
    public class ServerPopData : GameTable<ServerPopData, int, ServerPopData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int KEY { get; set; }
            /// <summary>
            /// 맵인덱스
            /// </summary>
            public int Map { get; set; }

            /// <summary>
            /// 몬스터 테이블 키값
            /// </summary>
            public int MonsterKey { get; set; }

            /// <summary>
            /// 적용될 레벨
            /// </summary>
            public int Level { get; set; }

            /// <summary>
            /// 리젠 시간(최소)
            /// </summary>
            public int RegenTime { get; set; }

            /// <summary>
            /// 리젠 시간(최대)
            /// </summary>
            public int MaxRegenTime { get; set; }

            /// <summary>
            /// 팝 위치
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// 바라보는 방향
            /// </summary>
            public float Rotation { get; set; }

            /// <summary>
            ///범위 내에 무작위 스폰
            /// </summary>
            public float SpawnRange { get; set; }

            /// <summary>
            /// 유닛 타이프(0 : 몬스터 / 1 : Npc)
            /// </summary>
            public TargetType ObjectType { get; set; }
        }

        protected override async UniTask OnCreated()
        {
            // 게임 테이블 타입을 설정한다.
            TableType = TableTool.TableType.WholeGameTable;
            CheckTable();
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return string.Empty;
        }
    }
}