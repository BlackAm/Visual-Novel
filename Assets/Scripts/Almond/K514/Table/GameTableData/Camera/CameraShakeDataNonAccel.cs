
using Cysharp.Threading.Tasks;
#if !SERVER_DRIVE
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 카메라를 일정 속도로 흔드는 연출에 관한 테이블 데이터 클래스
    /// </summary>
    public class CameraShakeDataNonAccel : GameTable<CameraShakeDataNonAccel, int, CameraShakeDataNonAccel.CameraShakeDataRecord>
    {
        public class CameraShakeDataRecord : GameTableRecordBase
        {
            /// <summary>
            /// 흔들림 지속 시간
            /// </summary>
            public uint DurationMsec { get; private set; }
            
            /// <summary>
            /// 최대 흔들림 거리
            /// </summary>
            public float Distance { get; private set; }
            
            /// <summary>
            /// 흔들림 왕복 횟수
            /// </summary>
            public int CycleCount { get; private set; }
            
            /// <summary>
            /// 흔들림 왕복 횟수 역수값
            /// </summary>
            public float InvCycleCount { get; private set; }

            /// <summary>
            /// 흔들릴 방향
            /// </summary>
            public Vector3 Direction { get; private set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                if (CycleCount != 0)
                {
                    InvCycleCount = 1f / CycleCount;
                }
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "CameraShake_NonAccel";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif