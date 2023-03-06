using UnityEngine;
using UnityEngine.Rendering;

namespace k514
{
    public interface IRenderable : IIncarnateUnit
    {
        UnitRenderDataRoot.UnitRenderType _RenderType { get; }
        IRenderableTableRecordBridge _RenderRecord { get; }
        IRenderable OnInitializeRender(UnitRenderDataRoot.UnitRenderType p_RenderType, Unit p_TargetUnit, IRenderableTableRecordBridge p_RenderPreset);
        
#if !SERVER_DRIVE
        RenderableTool.RendererControlPreset _DefaultModelRendererControlPreset { get; }
        RenderableTool.RendererControlPreset _AddedModelRendererControlPreset { get; }

        #region <Methods>

        void TurnRendererLayerTo(RenderableTool.RenderGroupType p_RenderGroupType,
            GameManager.GameLayerType p_LayerType);
        void ResetRendererLayer(RenderableTool.RenderGroupType p_RenderGroupType);
        void SetRenderEffect(RenderableTool.ShaderControlType p_ShaderControlType, int p_UpdateCount);

        #endregion

        #region <Method/Renderer>

        void AddRenderer(SkinnedMeshRenderer p_Renderer);

        #endregion

        #region <Method/ShaderControl/Silhouette>

        void SetSilhouetteEnable(RenderableTool.RenderGroupType p_RenderGroupType, bool p_Flag);
        void SetSilhouetteColor(RenderableTool.RenderGroupType p_RenderGroupType, Color p_Color);

        #endregion
        
        #region <Method/ShaderControl/Outline>

        void SetOutlineEnable(RenderableTool.RenderGroupType p_RenderGroupType, bool p_Flag);
        void SetOutlineColor(RenderableTool.RenderGroupType p_RenderGroupType, Color p_Color);
        void SetOutlineWidth(RenderableTool.RenderGroupType p_RenderGroupType, float p_Width);
        
        #endregion

        #region <Method/ShaderControl/Intensity>
        
        void SetIntensityEnable(RenderableTool.RenderGroupType p_RenderGroupType, bool p_Flag);
        void SetIntensityValue(RenderableTool.RenderGroupType p_RenderGroupType, float p_Value);

        #endregion

        #region <Method/ShaderControl/Texture>

        void SetWrappingTexture(RenderableTool.RenderGroupType p_RenderGroupType, int p_TextureType);

        #endregion
        
        #region <Method/ShaderControl/Dissolve>

        void SetDissolveEnable(RenderableTool.RenderGroupType p_RenderGroupType, bool p_Flag);
        void SetDissolveLerpProgress(RenderableTool.RenderGroupType p_RenderGroupType, float p_Progress01);
        void SetDissolveEdge(RenderableTool.RenderGroupType p_RenderGroupType, float p_Float);
        void SetDissolveTexture(RenderableTool.RenderGroupType p_RenderGroupType, int p_TextureType);
        void SetDissolveEdgeTexture(RenderableTool.RenderGroupType p_RenderGroupType, int p_TextureType);

        #endregion

        #region <Method/ShaderControl/RendererQueue>

        void SetRenderQueue(RenderableTool.RenderGroupType p_RenderGroupType, RenderQueue p_QueueType);
        void SetRenderQueue(RenderableTool.RenderGroupType p_RenderGroupType, int p_QueueDepth);

        #endregion

        #region <AttachVfx>
        
        #endregion
#endif
    }
}