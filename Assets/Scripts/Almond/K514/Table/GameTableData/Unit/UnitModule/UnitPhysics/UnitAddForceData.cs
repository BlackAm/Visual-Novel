using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class UnitAddForceData : GameTable<UnitAddForceData, int, UnitAddForceData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            /// <summary>
            /// 외력 타입
            /// </summary>
            public PhysicsTool.AccelerationType ForceType { get; private set; }

            /// <summary>
            /// 외력 적용방식에 대해 기술하는 열거형 상수 필드
            /// </summary>
            public UnitTool.UnitAddForceType ForceDirectionType { get; private set; }

            /// <summary>
            /// 외력 처리 플래그 마스크
            /// </summary>
            public UnitTool.UnitAddForceProcessType ForceProcessFlagMask;

            /// <summary>
            /// 외력 필터 이벤트 플래그 마스크
            /// </summary>
            public UnitTool.UnitAddForceFilterEventType ForceFilterEventFlagMask;
            
            /// <summary>
            /// 타격시 적을 날리는 벡터
            /// </summary>
            public Vector3 Force { get; private set; }
 
            /// <summary>
            /// 타격시 적 날리기 선딜레이
            /// </summary>
            public uint ForceDelay { get; private set; }

            /// <summary>
            /// 감쇄 카운트
            /// </summary>
            public int DampingBound { get; private set; }
            
            /// <summary>
            /// 힘을 가하는 주체 혹은 특정 좌표로부터 일정거리이상 벗어나서는 안되는 경우 그 거리를 기술하는 필드
            /// </summary>
            public float PivotDistanceBound { get; private set; }

            /// <summary>
            /// 충돌 검증 및 데미지 이벤트를 처리할 히트 프리셋 인덱스
            /// </summary>
            public int HitPresetIndex;
            
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                if (ForceFilterEventFlagMask != UnitTool.UnitAddForceFilterEventType.None)
                {
                    ForceProcessFlagMask.AddFlag(UnitTool.UnitAddForceProcessType.Filter);
                }
                else
                {
                    ForceProcessFlagMask.RemoveFlag(UnitTool.UnitAddForceProcessType.Filter);
                }
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitAddForceTable";
        }
    }
}