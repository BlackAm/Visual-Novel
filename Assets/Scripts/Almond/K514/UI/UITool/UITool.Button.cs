#if !SERVER_DRIVE
using UnityEngine.Events;
using UnityEngine.UI;

namespace k514
{
    public partial class UITool
    {
        /// <summary>
        /// 지정한 버튼의 클릭이벤트에 이벤트 대리자를 등록시켜주는 메서드
        /// </summary>
        public static void SetButtonHandler(this Button p_TargetButton, UnityAction p_HandleEvent)
        {
            var tryHandler = new Button.ButtonClickedEvent();
            tryHandler.AddListener(p_HandleEvent);
            p_TargetButton.onClick = tryHandler;
        }
    }
}
#endif