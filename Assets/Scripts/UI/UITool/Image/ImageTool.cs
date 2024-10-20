using UnityEngine;
using UnityEngine.UI;

#if !SERVER_DRIVE

namespace BlackAm
{
    public partial class UITool
    {
        public static (bool, ImagePreset) GetImagePreset(this Transform p_Root, string p_ImageName)
        {
            var (valid, tryWrapper) = p_Root.FindRecursive(p_ImageName);
            if (valid)
            {
                var tryComponent = tryWrapper.GetComponent<Image>();
                if (ReferenceEquals(null, tryComponent))
                {
                    return default;
                }
                else
                {
                    return (true, new ImagePreset(tryComponent));
                }
            }
            else
            {
                return default;
            }
        }
    }
}
#endif