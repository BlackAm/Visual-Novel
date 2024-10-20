using UnityEngine;

namespace BlackAm
{
    public partial class SystemMaintenance
    {
        /// <summary>
        /// 게임 윈도우 기본너비
        /// </summary>
        public const int DEFAULT_SCREEN_WIDTH = 1920;
        
        /// <summary>
        /// 게임 윈도우 기본높이
        /// </summary>
        public const int DEFAULT_SCREEN_HEIGHT = 1080;
        
        /// <summary>
        /// 게임 윈도우 기본너비 역수
        /// </summary>
        public const float INV_DEFAULT_SCREEN_WIDTH = 1f / DEFAULT_SCREEN_WIDTH;
        
        /// <summary>
        /// 게임 윈도우 기본높이 역수
        /// </summary>
        public const float INV_DEFAULT_SCREEN_HEIGHT = 1f / DEFAULT_SCREEN_HEIGHT;
        
        public static Vector2 ScreenScaleVector2 = new Vector2(DEFAULT_SCREEN_WIDTH, DEFAULT_SCREEN_HEIGHT);
        public static Vector3 ScreenScaleVector3 = new Vector3(DEFAULT_SCREEN_WIDTH, DEFAULT_SCREEN_HEIGHT);
        public static Vector2 ScreenHalfScaleVector2 = 0.5f * ScreenScaleVector2;
        public static Vector3 ScreenHalfScaleVector3 = 0.5f * ScreenScaleVector3;
        public static Vector2 ScreenReverseScaleVector2 = new Vector2(INV_DEFAULT_SCREEN_WIDTH, INV_DEFAULT_SCREEN_HEIGHT);
        public static Vector3 ScreenReverseScaleVector3 = new Vector3(INV_DEFAULT_SCREEN_WIDTH, INV_DEFAULT_SCREEN_HEIGHT);
        public static Vector3 GetCurrentScreenScaleVector3 => new Vector3(Screen.width, Screen.height);
    }
}