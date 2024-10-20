using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class SaveLoadManager
    {
        public Sprite LoadImageAsSprite(string p_Path)
        {
            if (string.IsNullOrEmpty(p_Path)) return null;
            if (File.Exists($"{SystemMaintenance.SaveDataImageDirectory}{p_Path}.png"))
            {
                var byteTexture = File.ReadAllBytes($"{SystemMaintenance.SaveDataImageDirectory}{p_Path}.png");
                var texture = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
                texture.filterMode = FilterMode.Trilinear;
                texture.LoadImage(byteTexture);
                
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1920, 1080), new Vector2(0.5f, 0.0f), 1.0f);
                return sprite;
            }

            return null;
        }
    }
}
