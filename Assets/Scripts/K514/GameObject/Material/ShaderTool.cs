#if !SERVER_DRIVE
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BlackAm
{
    public static class ShaderTool
    {
        #region <TurnProperty>
        
        public static void SetFlag<T>(List<T> p_RenderGroup, string p_PropertyName, bool p_Flag) where T : Renderer
        {
            SetFloat(p_RenderGroup, p_PropertyName, p_Flag ? 1f : 0f);
        }
        
        public static void SetFloat<T>(List<T> p_RenderGroup, string p_PropertyName, float p_Float) where T : Renderer
        {
            foreach (var renderer in p_RenderGroup)
            {
                var materials = renderer.sharedMaterials;
                foreach (var material in materials)
                {
                    if(ReferenceEquals(null, material)) continue;
                    material.SetFloat(p_PropertyName, p_Float);
                }
            }
        }
        
        public static void SetColor<T>(List<T> p_RenderGroup, string p_PropertyName, Color p_Color) where T : Renderer
        {
            foreach (var renderer in p_RenderGroup)
            {
                var materials = renderer.sharedMaterials;
                foreach (var material in materials)
                {
                    if(ReferenceEquals(null, material)) continue;
                    material.SetColor(p_PropertyName, p_Color);
                }
            }
        }
        
        public static void SetTexture<T>(List<T> p_RenderGroup, string p_PropertyName, int p_TextureType) where T : Renderer
        {
            var texture = ImageNameTableData.GetInstanceUnSafe.GetTexture(p_TextureType, ResourceType.Material,
                ResourceLifeCycleType.Free_Condition).Item2;
            
            foreach (var renderer in p_RenderGroup)
            {
                var materials = renderer.sharedMaterials;
                foreach (var material in materials)
                {
                    if(ReferenceEquals(null, material)) continue;
                    material.SetTexture(p_PropertyName, texture);
                }
            }
        }
        
        #endregion
        
        #region <TurnRenderQueue>

        public static void SetRenderQueue<T>(List<T> p_RenderGroup, RenderQueue p_QueueType) where T : Renderer
        {
            SetRenderQueue(p_RenderGroup, (int)p_QueueType);
        }
        
        public static void SetRenderQueue<T>(List<T> p_RenderGroup, int p_QueueDepth) where T : Renderer
        {
            foreach (var renderer in p_RenderGroup)
            {
                var materials = renderer.sharedMaterials;
                foreach (var material in materials)
                {
                    if(ReferenceEquals(null, material)) continue;
                    material.renderQueue = p_QueueDepth;
                }
            }
        }

        #endregion
        
        #region <TurnShadow>

        public static void SetRenderShadow<T>(List<T> p_RenderGroup, ShadowCastingMode p_Type) where T : Renderer
        {
            foreach (var renderer in p_RenderGroup)
            {
                renderer.shadowCastingMode = p_Type;
            }
        }

        #endregion

        #region <TurnShader>

        public static void SetShader(Material[] p_Materials, Shader p_Shader)
        {
            foreach (var material in p_Materials)
            {
                if(ReferenceEquals(null, material)) continue;
                material.shader = p_Shader;
                
                var tryMainTexture = material.mainTexture;
                if (!ReferenceEquals(null, tryMainTexture))
                {
                    material.mainTexture = tryMainTexture;
                }
            }
        }

        #endregion
    }
}
#endif