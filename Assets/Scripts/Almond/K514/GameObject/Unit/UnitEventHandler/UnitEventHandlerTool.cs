using System;

namespace k514
{
    public static class UnitEventHandlerTool
    {
        #region <Enums>

        [Flags]
        public enum UnitEventType
        {
            /// <summary>
            /// 이벤트 없음
            /// </summary>
            None = 0,

            /// <summary>
            /// 해당 유닛이 이동한 경우
            /// </summary>
            PositionChanged = 1 << 0,

            /// <summary>
            /// 해당 유닛이 사망한 경우
            /// </summary>
            UnitDead = 1 << 1,

            /// <summary>
            /// 해당 유닛이 카메라 뷰포트 밖으로 벗어난 경우
            /// </summary>
            OutOfScreen = 1 << 2,

            /// <summary>
            /// 해당 유닛이 카메라와의 거리에 따라 컬링된 경우
            /// </summary>
            DistanceCulling = 1 << 3,

            /// <summary>
            /// 유닛이 랜더링되는 도중에 카메라가 움직인 경우
            /// </summary>
            CameraMovedWhenThisUnitRendering = 1 << 4,

            /// <summary>
            /// 유닛의 이름이 변경된 경우
            /// </summary>
            UnitNameChanged = 1 << 5,

            /// <summary>
            /// 유닛이 풀에 회수된 경우
            /// </summary>
            UnitRetrieved = 1 << 6,

            /// <summary>
            /// 유닛이 비활성화된 경우
            /// </summary>
            UnitDisabled = 1 << 7,

            /// <summary>
            /// 유닛의 이름표 UI를 활성화 여부를 수동으로 제어할 때 사용하는 이벤트 타입
            /// </summary>
            SwitchHideUINameLabel = 1 << 8,

            /// <summary>
            /// 유닛의 이름표 UI 폰트 색상을 제어할 때 사용하는 이벤트 타입
            /// </summary>
            SwitchColorUINameLabel = 1 << 9,

            /// <summary>
            /// 파티원일경우 이름 색상변경
            /// </summary>
            SwitchColorUIPartyColor = 1 << 10,

            UnitImageChanged = 1 << 19,
            SwitchHideUIImageLabel = 1 << 20,
            SwitchHideUIImageMiniMapMaker = 1 << 21,
            SwitchHideUIImageWorldMapMaker = 1 << 22,

            UINameLabelEventFlag = PositionChanged | UnitDead | OutOfScreen | DistanceCulling
                                    | CameraMovedWhenThisUnitRendering | UnitNameChanged | UnitRetrieved | UnitDisabled | SwitchHideUINameLabel | SwitchColorUINameLabel
                                    | SwitchColorUIPartyColor,
            UIImageMakerEventFlag = PositionChanged | UnitDead | OutOfScreen | DistanceCulling 
                                    | CameraMovedWhenThisUnitRendering | UnitImageChanged | UnitRetrieved | UnitDisabled | SwitchHideUIImageLabel | SwitchHideUIImageMiniMapMaker | SwitchHideUIImageWorldMapMaker
                                    | SwitchColorUIPartyColor
        }
        
        public static UnitEventType[] _UnitEventType_Enumerator;

        #endregion

        #region <Constructor>

        static UnitEventHandlerTool()
        {
            _UnitEventType_Enumerator = SystemTool.GetEnumEnumerator<UnitEventType>(SystemTool.GetEnumeratorType.ExceptMaskNone);
        }

        #endregion
    }
}