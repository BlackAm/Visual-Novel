#if !SERVER_DRIVE
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class UITool
    {
        public static SliderPreset SetSliderPreset(this Transform p_Transform)
        {
            return new SliderPreset(p_Transform);
        }
    }
}
#endif