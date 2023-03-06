#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.UI;

namespace k514
{
    public partial class UITool
    {
        public static Canvas CreateDefaultCanvas(this RectTransform p_Wrapper)
        {
            var canvasObject = new GameObject();
            var canvas = canvasObject.AddComponent<Canvas>();
            canvasObject.transform.SetParent(p_Wrapper, false);
            (canvasObject.transform as RectTransform).SetPivotPreset(XAnchorType.Stretch, YAnchorType.Stretch, Vector2.zero, Vector2.zero);
            canvasObject.layer = (int)GameManager.GameLayerType.UI;
            
            var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.InitCanvasScaler();
            
            var graphicRaycaster = canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        public static void InitCanvasScaler(this CanvasScaler p_Target)
        {
            p_Target.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            p_Target.referenceResolution = SystemMaintenance.ScreenScaleVector2;
            p_Target.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            p_Target.matchWidthOrHeight = 0f;
            p_Target.referencePixelsPerUnit = 100;
        }
    }
}
#endif