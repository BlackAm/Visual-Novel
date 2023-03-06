#if !SERVER_DRIVE
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 카메라를 가속하는 속도로 흔드는 연출에 관한 테이블 데이터 클래스
    /// </summary>
    public class CameraShakeData : GameTable<CameraShakeData, int, CameraShakeData.CameraShakeDataRecord>
    {
        public class CameraShakeDataRecord : GameTableRecordBase
        {
            /// <summary>
            /// 최대 속도
            /// </summary>
            public Vector3 SwingBoundVector { get; private set; }
            
            /// <summary>
            /// 가속도
            /// </summary>
            public Vector3 Acceleration { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "CameraShake";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif