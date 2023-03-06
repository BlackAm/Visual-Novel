using System;
using System.Collections.Generic;
using UnityEngine;
 
namespace k514
{
    public static class RenderableTool
    {
        #region <Enums>

#if !SERVER_DRIVE
        [Flags]
        public enum RenderGroupType
        {
            None = 0,
            OriginGroup = 1 << 0,
            AddedGroup = 1 << 1,
            Whole = OriginGroup | AddedGroup,
        }
 
        public static readonly RenderGroupType[] RenderGroupTypeEnumerator = SystemTool.GetEnumEnumerator<RenderGroupType>(SystemTool.GetEnumeratorType.ExceptMaskNone);
#endif
        
        /// <summary>
        /// 셰이더 타입
        /// </summary>
        [Flags]
        public enum ShaderControlType
        {
            /* 셰이더 컨트롤 타입 */
            /// <summary>
            /// 원본 랜더러
            /// </summary>
            None = 0,
             
            /// <summary>
            /// 외곽선 적용
            /// </summary>
            Outline = 1 << 0,
             
            /// <summary>
            /// 실루엣 적용(다른 오브젝트에 가려지는 경우 실루엣 출력)
            /// </summary>
            Silhouette = 1 << 1,
             
            /// <summary>
            /// 밝기 조정
            /// </summary>
            Intensity = 1 << 2,
             
            /// <summary>
            /// 텍스처 소각 애니메이션
            /// </summary>
            Dissolve = 1 << 3,
             
            /* 셰이더 에셋 이름 */
            MatCap_TextureMult = Outline | Intensity | Dissolve,
            MatCap_TextureMult_Silhouette = MatCap_TextureMult | Silhouette,
        }
 
        public static readonly ShaderControlType[] ShaderControlTypeEnumerator = SystemTool.GetEnumEnumerator<ShaderControlType>(SystemTool.GetEnumeratorType.ExceptMaskNone);

        #endregion
                 
#if !SERVER_DRIVE
        #region <Classess>

        public class RendererControlPreset
        {
            #region <Fields>
 
            public List<SkinnedMeshRenderer> RendererGroup;
            public Dictionary<SkinnedMeshRenderer, (SkinnedMeshRendererControlPreset t_RendererPreset, MaterialControlPreset t_MaterialPreset)> RendererControlGroups;
 
            #endregion
 
            #region <Enums>
 
            public enum MaterialControlSwitchType
            {
                ToShared,
                ToCopied,
                MakeNewCopiedSetAndUse,
            }
 
            #endregion
             
            #region <Constructors>
             
            public RendererControlPreset()
            {
                RendererGroup = new List<SkinnedMeshRenderer>();
                RendererControlGroups =
                    new Dictionary<SkinnedMeshRenderer, (SkinnedMeshRendererControlPreset t_RendererPreset,
                        MaterialControlPreset t_MaterialPreset)>();
                 
            }
             
            public RendererControlPreset(GameObject p_GameObject) : this()
            {
                SetRendererGroup(p_GameObject);
            }
 
            #endregion
 
            #region <Operator>
 
            public static implicit operator List<SkinnedMeshRenderer>(RendererControlPreset c_RendererControlPreset) =>
                c_RendererControlPreset.RendererGroup;
             
            #endregion
             
            #region <Methods>
 
            public void SetRendererGroup(GameObject p_GameObject)
            {
                p_GameObject.GetComponentsInChildren(RendererGroup);
 
                foreach (var renderer in RendererGroup)
                {
                    RendererControlGroups.Add(renderer, (new SkinnedMeshRendererControlPreset(renderer), new MaterialControlPreset(renderer)));
                }
            }
             
            public void SetRendererGroup(SkinnedMeshRenderer p_Renderer)
            {
                if (RendererGroup.Contains(p_Renderer))
                {
                    RendererControlGroups[p_Renderer].t_RendererPreset.UpdatePreset();
                    RendererControlGroups[p_Renderer].t_MaterialPreset.UpdatePreset();
                }
                else
                {
                    RendererGroup.Add(p_Renderer);
                    RendererControlGroups.Add(p_Renderer, (new SkinnedMeshRendererControlPreset(p_Renderer), new MaterialControlPreset(p_Renderer)));
                }
            }
 
            public void SwitchMaterialSet(MaterialControlSwitchType p_SwitchType)
            {
                foreach (var renderer in RendererGroup)
                {
                    RendererControlGroups[renderer].t_MaterialPreset.SwitchMaterialSet(p_SwitchType);
                }
            }
             
            public void SwitchShaderSet(ShaderControlType p_ShaderType)
            {
                foreach (var renderer in RendererGroup)
                {
                    RendererControlGroups[renderer].t_MaterialPreset.SwitchShader(p_ShaderType);
                }
            }
            #endregion
        }

        #endregion
         
        #region <Structs>
 
        public struct SkinnedMeshRendererControlPreset
        {
            #region <Fields>
 
            private SkinnedMeshRenderer _Renderer;
 
            #endregion
             
            #region <Constructor>
 
            public SkinnedMeshRendererControlPreset(SkinnedMeshRenderer p_Renderer)
            {
                _Renderer = p_Renderer;
            }
 
            #endregion
 
            #region <Methods>
 
            public void UpdatePreset()
            {
            }
 
            #endregion
        }
 
        public struct MaterialControlPreset
        {
            #region <Fields>
             
            private SkinnedMeshRenderer _Renderer;
            private int _MaterialCount;
            private Material[] _OriginMaterialList;
            private Material[] _CopiedMaterialList;
            private ShaderControlType _CurrentShaderType;
            private (AssetPreset, Shader) _AssetPresetTuple;
             
            #endregion
             
            #region <Constructor>
 
            public MaterialControlPreset(SkinnedMeshRenderer p_Renderer)
            {
                _Renderer = p_Renderer;
                _CurrentShaderType = default;
                _AssetPresetTuple = default;
                 
                _OriginMaterialList = p_Renderer.sharedMaterials;
                _MaterialCount = _OriginMaterialList.Length;
                _CopiedMaterialList = new Material[_MaterialCount];
                 
                // 머티리얼 복제본을 가져온다.
                UpdateCopiedMaterialSet();
            }
 
            #endregion
             
            #region <Methods>
 
            private void UpdateCopiedMaterialSet()
            {
                switch (_CurrentShaderType)
                {
                    case ShaderControlType.MatCap_TextureMult:
                    case ShaderControlType.MatCap_TextureMult_Silhouette:
                    {
                        for (int i = 0; i < _MaterialCount; i++)
                        {
                            if (ReferenceEquals(null, _OriginMaterialList[i])) continue;
                            _CopiedMaterialList[i] = new Material(_OriginMaterialList[i]);
                        }
                        ShaderTool.SetShader(_CopiedMaterialList, _AssetPresetTuple.Item2);
                    }
                        break;
                    default:
                    {
                        for (int i = 0; i < _MaterialCount; i++)
                        {
                            if (ReferenceEquals(null, _OriginMaterialList[i])) continue;
                            _CopiedMaterialList[i] = new Material(_OriginMaterialList[i]);
                        }
                    }
                        break;
                }
            }
 
            public void UpdatePreset()
            {
            }
             
            public void SwitchMaterialSet(RendererControlPreset.MaterialControlSwitchType p_SwitchType)
            {
                switch (p_SwitchType)
                {
                    case RendererControlPreset.MaterialControlSwitchType.ToShared:
                    {
                        _Renderer.sharedMaterials = _OriginMaterialList;
                    }
                        break;
                    case RendererControlPreset.MaterialControlSwitchType.ToCopied:
                    {
                        _Renderer.sharedMaterials = _CopiedMaterialList;
                    }
                        break;
                    case RendererControlPreset.MaterialControlSwitchType.MakeNewCopiedSetAndUse:
                    {
                        UpdateCopiedMaterialSet();
                        _Renderer.sharedMaterials = _CopiedMaterialList;
                    }
                        break;
                }
            }
 
            public void SwitchShader(ShaderControlType p_ShaderType)
            {
                
                if (_AssetPresetTuple.Item1.IsValid)
                {
                    LoadAssetManager.GetInstanceUnSafe.UnloadAsset(_AssetPresetTuple.Item1);
                }
 
                _CurrentShaderType = p_ShaderType;
                _AssetPresetTuple = ShaderNameTableData.GetInstanceUnSafe.GetResource(_CurrentShaderType, ResourceType.Material, ResourceLifeCycleType.Free_Condition);
                SwitchMaterialSet(RendererControlPreset.MaterialControlSwitchType.MakeNewCopiedSetAndUse);
            }
 
            #endregion
        }
        #endregion
#endif
    }
}