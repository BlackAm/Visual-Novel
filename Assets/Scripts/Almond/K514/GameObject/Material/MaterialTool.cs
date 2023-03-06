using UnityEngine;

namespace k514
{
    public static class MaterialTool
    {
        /// <summary>
        /// 지정한 게임 오브젝트의 랜더러를 전부 탐색하여, 각 머티리얼의 셰이더를 리로드하는 메서드
        /// 모바일에서 에셋번들로부터 로드한 셰이더는 리로드를 해야 정상 표시되는데 그 때 사용한다.
        /// </summary>
        public static void ReloadMaterial(this GameObject p_TargetGameObject)
        {
            
#if UNITY_EDITOR
            var RendererGroup = p_TargetGameObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in RendererGroup)
            {
                if (renderer is ParticleSystemRenderer particleSystemRenderer)
                {
                    var materialGroup = particleSystemRenderer.sharedMaterials;
                    foreach (var material in materialGroup)
                    {
                        if (material != null)
                        {
                            material.shader = Shader.Find(material.shader.name);
                        }
                    }

                    var monoMaterial = particleSystemRenderer.sharedMaterial;
                    if (monoMaterial != null)
                    {
                        monoMaterial.shader = Shader.Find(monoMaterial.shader.name);
                    }
                    var trailMaterial = particleSystemRenderer.trailMaterial;
                    if (trailMaterial != null)
                    {
                        trailMaterial.shader = Shader.Find(trailMaterial.shader.name);
                    }
                }
                else
                {
                    var materialGroup = renderer.sharedMaterials;
                    foreach (var material in materialGroup)
                    {
                        if (material != null)
                        {
                            material.shader = Shader.Find(material.shader.name);
                        }
                    }
                    var monoMaterial = renderer.sharedMaterial;
                    if (monoMaterial != null)
                    {
                        monoMaterial.shader = Shader.Find(monoMaterial.shader.name);
                    }
                }
            }
#endif
        }
       

    }
}