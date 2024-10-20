using UnityEngine;

namespace BlackAm
{
    public partial class CommandSystem
    {
        #region <Fields>

        public bool DashFlag;

        #endregion

        #region <Callbacks>

        private void OnUpdateDashFlag(bool p_Flag)
        {
            DashFlag = p_Flag;
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 현재 이동중인 방향에서 교착 상태가 되지 않는 방향, 예를 들어 위쪽 방향이 현재 방향이라면 좌/우 방향,
        /// 으로 이중 입력이 발생했는지 체크하는 논리 메서드
        /// 만약 특정 방향으로 입력이 발생했다면, 그 입력 정보를 부분만 가져와서(예를 들어, 상이동 + 우측 입력 이었다면 입력된 부분 정보는 (우)이다.)
        /// outmode로 리턴한다.
        /// </summary>
        private (bool, ArrowType) HasPerspectiveKeyDoubleTriggered(ArrowType p_CompareTo, bool p_IsSoleArrowType)
        {
            // 지정한 방향키 타입에서 마지막으로 입력된 방향키 타입을 제한 나머지 성분을 LastTransitedArrowTypeDelta에 세트한다.
            // 만약, "LastTransitedArrowType ⊂ 파라미터 타입"을 만족하지 않으면 None을 리턴한다.
            //
            // 여기에서 Controller Tracker의 PrevArrow타입과 현재 타입을 비교하는데, 그 이유는 이전 터치 혹은 키보드 입력 선정과정에서 p_CompareTo값과 ControllerTracker.CurrentArrowType
            // 값이 동기화되기 때문에 이전 값을 저장해놨다가 비교하는 방식을 사용하고 있기 때문이다.
            var _LastTransitedArrowTypeDelta = p_IsSoleArrowType ? ArrowType.None : ArrowType.None + Mathf.Max(0, p_CompareTo - ControllerTracker.PrevArrowType);;

            // 키보드 입력이 변경됬던 경우
            if (_LastTransitedArrowTypeDelta != ArrowType.None)
            {
                // LastTransitedArrowTypeDeltaCompare 는 이전에 변경된 값으로 두 값이 일치한다는 것은
                // 특정 방향으로의 입력이 두 번 입력되었음을 나타낸다.
                if (_LastTransitedArrowTypeDelta == LastTransitedArrowTypeDeltaCompare)
                {
                    LastTransitedArrowTypeDeltaCompare = _LastTransitedArrowTypeDelta;
                    return (true, _LastTransitedArrowTypeDelta);
                }
                // 일치하지 않는 경우
                else
                {
                    PerspectiveArrowInputExpireReq(_LastTransitedArrowTypeDelta);
                    return default;
                }
            }
            // 키보드 입력이 변화가 없었던 경우
            else
            {
                // 변화가 없었던 경우에는 LastTransitedArrowTypeDeltaCompare를 갱신해주지 않는다.
                // 대쉬입력의 작은 시간 차에 LastTransitedArrowTypeDeltaCompare가 None으로 갱신되기 때문이다.
                return default;
            }
        }

        #endregion
    }
}