using System.Collections.Generic;
using UnityEngine;

public class GlitchSwitcher : MonoBehaviour
{
    public Material glitchMaterial; // The material used for the glitch effect
    public bool glitchOnStart;
    private class RendererInfo
    {
        public Renderer renderer;
        public Material[] originalMaterials;
    }

    private List<RendererInfo> renderers = new List<RendererInfo>();

    void Awake()
    {
        CacheOriginalMaterials();
        ApplyGlitch(glitchOnStart);
    }

    private void CacheOriginalMaterials()
    {
        renderers.Clear();

        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            RendererInfo info = new RendererInfo
            {
                renderer = rend,
                originalMaterials = rend.materials
            };
            renderers.Add(info);
        }
    }

    public void ApplyGlitch(bool enable)
    {
        foreach (var info in renderers)
        {
            if (enable)
            {
                Material[] glitchMats = new Material[info.originalMaterials.Length];
                for (int i = 0; i < glitchMats.Length; i++)
                    glitchMats[i] = glitchMaterial;

                info.renderer.materials = glitchMats;
            }
            else
            {
                info.renderer.materials = info.originalMaterials;
            }
        }
    }
}
