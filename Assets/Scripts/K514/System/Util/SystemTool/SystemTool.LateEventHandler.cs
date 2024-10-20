using System;

namespace BlackAm
{
    public static partial class SystemTool
    {
        /// <summary>
        /// 시스템 업데이트 함수 이후에 처리할 이벤트 타입
        /// </summary>
        [Flags]
        public enum SystemOnceFrameEventType
        {
            None = 0,
             
            /// <summary>
            /// 화면 플래시
            /// </summary>
            FlashScreen = 1 << 0,
             
            /// <summary>
            /// 시스템이 특정 사운드를 출력해야하는 경우
            /// </summary>
            Beep = 1 << 1,
        }
         
        public static SystemOnceFrameEventType[] SystemLateHandleEventTypeEnumerator;
    }
}